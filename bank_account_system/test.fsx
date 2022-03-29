open System

#load "Domain.fs"
#load "Operations.fs"
#load "Auditing.fs"

open Bank.Domain
open Bank.Operations
open Bank.Auditing

let validCommands = Set.ofList ([ "d"; "w"; "x" ])

let isValidCommand command = validCommands.Contains command

let isStopCommand command =
    match command with
    | "x" -> true
    | _ -> false

let getAmount command =
    Console.Write "Amount: "
    let ammount = Console.ReadLine() |> Decimal.Parse

    (command, ammount)

let withdrawWithAudit = auditAs "withdraw" consoleAudit withdraw
let depositWithAudit = auditAs "deposit" consoleAudit deposit

let processCommand account (command, amount) =
    match command with
    | "d" -> depositWithAudit amount account
    | "w" -> withdrawWithAudit amount account
    | _ -> account

let account =
    let openingAccount =
        { Id = Guid.Empty
          Owner = { Name = "Henry" }
          Balance = 69M }

    seq {
        while true do
            Console.WriteLine()
            Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
            yield Console.ReadLine()

    }
    |> Seq.filter isValidCommand
    |> Seq.takeWhile (not << isStopCommand)
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount
