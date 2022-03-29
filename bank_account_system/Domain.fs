namespace Bank.Domain

open System

type Customer = { Name: string }

type Account =
    { Id: System.Guid
      Owner: Customer
      Balance: decimal }

[<AutoOpen>]
module Transactions =
    type Transaction =
        { Operation: string
          Timestamp: DateTime
          Amount: decimal
          Accepted: bool }

    let serialized (transaction: Transaction) =
        sprintf "%O***%s***%M***%b" transaction.Timestamp transaction.Operation transaction.Amount transaction.Accepted

    let deserialize (transactionStr: string) =
        match transactionStr.Split("***", StringSplitOptions.None) with
        | [| a; b; c; d |] ->
            { Timestamp = DateTime.Parse a
              Operation = b
              Amount = Decimal.Parse c
              Accepted = bool.Parse d }
        | _ -> failwith $"unrecognised transaction format {transactionStr}"
