module Capstone4.Operations

open System
open Capstone4.Domain

let classifyAccount account =
    if account.Balance >= 0M then
        (InCredit(CreditAccount account))
    else
        Overdrawn account

/// Withdraws an amount of an account
let withdraw amount account =
    { account with Balance = account.Balance - amount }
    |> classifyAccount

let withdrawSafe amount ratedAccount =
    match ratedAccount  with
    | InCredit(CreditAccount account) -> account |> withdraw amount
    | Overdrawn _ ->
        printfn "Your account is overdrawn - withdrawal rejected"
        ratedAccount


/// Deposits an amount into an account
let deposit amount account =
    let account =
        match account with
        | InCredit (CreditAccount account) -> account
        | Overdrawn account -> account

    { account with Balance = account.Balance + amount }
    |> classifyAccount

/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationType audit operation amount (account: RatedAccount) =
    let updatedAccount = operation amount account

    let accountIsUnchanged = (updatedAccount = account)

    let transaction =
        { Operation = operationType
          Amount = amount
          Timestamp = DateTime.UtcNow }

    audit (account.GetField(fun a -> a.AccountId)) (account.GetField(fun a -> a.Owner).Name) transaction
    updatedAccount

/// Creates an account from a historical set of transactions
let loadAccount (owner, accountId, transactions) =
    let openingAccount =
        { AccountId = accountId
          Balance = 0M
          Owner = { Name = owner } }
        |> classifyAccount

    transactions
    |> Seq.sortBy (fun txn -> txn.Timestamp)
    |> Seq.fold
        (fun account txn ->
            match txn.Operation with
            | Withdraw -> account |> withdrawSafe txn.Amount
            | Deposit -> account |> deposit txn.Amount)
        openingAccount
