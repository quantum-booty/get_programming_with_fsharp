namespace Capstone4.Domain

open System

type BankOperation =
    | Deposit
    | Withdraw

type Customer = { Name: string }

type Account =
    { AccountId: Guid
      Owner: Customer
      Balance: decimal }

type CreditAccount = CreditAccount of Account

type RatedAccount =
    | InCredit of CreditAccount
    | Overdrawn of Account
    member this.GetField getter =
        match this with
        | InCredit (CreditAccount account) -> getter account
        | Overdrawn account -> getter account

type Transaction =
    { Timestamp: DateTime
      Operation: BankOperation
      Amount: decimal }

module Transactions =
    /// Serializes a transaction
    let serialize transaction =
        sprintf "%O***%O***%M" transaction.Timestamp transaction.Operation transaction.Amount

    /// Deserializes a transaction
    let deserialize (fileContents: string) =
        let parts = fileContents.Split([| "***" |], StringSplitOptions.None)

        let op =
            match parts.[1] with
            | "Withdraw" -> Withdraw
            | "Deposit" -> Deposit
            | _ -> failwith "Unrecognised command"

        { Timestamp = DateTime.Parse parts.[0]
          Operation = op
          Amount = Decimal.Parse parts.[2] }
