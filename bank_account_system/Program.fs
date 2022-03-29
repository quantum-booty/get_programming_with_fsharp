open System
open Bank.Auditing
open Bank.Domain
open Bank.Operations
open Bank.FileRepository

let withdrawWithAudit = auditAs "w" composedLogger withdraw
let depositWithAudit = auditAs "d" composedLogger deposit
let loadAccountFromDisk = findTransactionsOnDisk >> loadAccount

[<AutoOpen>]
module CommandParsing =
    let isValidCommand command =
        [ "d"; "w"; "x" ] |> List.contains command

    let isStopCommand command =
        match command with
        | "x" -> true
        | _ -> false

[<AutoOpen>]
module UserInput =
    let getAmount command =
        Console.Write "Enter Amount: "
        (command, Console.ReadLine() |> Decimal.Parse)

    let commands =
        seq {
            while true do
                Console.WriteLine()
                Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
                yield Console.ReadLine()

        }

let accountBalanceLogger account =
    printfn "Account %O: current balance %M" account.Id account.Balance

let processCommand account (command, amount) =
    accountBalanceLogger account

    match command with
    | "d" -> depositWithAudit amount account
    | "w" -> withdrawWithAudit amount account
    | _ -> account

[<EntryPoint>]
let main argv =
    let openingAccount =
        Console.Write "What is your name?"
        Console.ReadLine() |> loadAccountFromDisk

    accountBalanceLogger openingAccount

    commands
    |> Seq.filter isValidCommand
    |> Seq.takeWhile (not << isStopCommand)
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount
    |> ignore

    0
