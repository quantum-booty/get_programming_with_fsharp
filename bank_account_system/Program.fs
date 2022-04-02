module Capstone4.Program

open System
open Capstone4.Domain
open Capstone4.Operations

let withdrawWithAudit = auditAs Withdraw Auditing.composedLogger withdrawSafe
let depositWithAudit = auditAs Deposit Auditing.composedLogger deposit

let tryLoadAccountFromDisk =
    FileRepository.findTransactionsOnDisk
    >> Option.map Operations.loadAccount

type Command =
    | AccountCommand of BankOperation
    | Exit

let tryGetBankOperation cmd =
    match cmd with
    | AccountCommand op -> Some op
    | Exit -> None


[<AutoOpen>]
module CommandParsing =
    let tryParseCommand x =
        match x with
        | 'w' -> Some(AccountCommand Withdraw)
        | 'd' -> Some(AccountCommand Deposit)
        | 'x' -> Some Exit
        | _ -> None

    let isStopCommand command = command = Exit

[<AutoOpen>]
module UserInput =
    let commands =
        seq {
            while true do
                Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
                yield Console.ReadKey().KeyChar
                Console.WriteLine()
        }

    let getAmount command =
        Console.WriteLine()
        Console.Write "Enter Amount: "
        let amount = Console.ReadLine() |> Decimal.TryParse

        match amount with
        | true, amount -> Some(command, amount)
        | false, _ -> None


[<EntryPoint>]
let main _ =
    let openingAccount =
        Console.Write "Please enter your name: "
        let owner = Console.ReadLine()

        match (tryLoadAccountFromDisk owner) with
        | Some account -> account
        | None ->
            let account =
                { Balance = 0M
                  AccountId = Guid.NewGuid()
                  Owner = { Name = owner } }

            InCredit(CreditAccount account)

    printfn "Current balance is £%M" (openingAccount.GetField(fun a -> a.Balance))

    let processCommand account (command, amount) =
        printfn ""

        let account =
            match command with
            | Deposit -> account |> depositWithAudit amount
            | Withdraw -> account |> withdrawWithAudit amount

        printfn "Current balance is £%M" (account.GetField(fun a -> a.Balance))
        account

    let closingAccount =
        commands
        |> Seq.choose tryParseCommand
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.choose tryGetBankOperation
        |> Seq.choose getAmount
        |> Seq.fold processCommand openingAccount

    printfn ""
    printfn "Closing Balance:\r\n %A" closingAccount
    Console.ReadKey() |> ignore

    0
