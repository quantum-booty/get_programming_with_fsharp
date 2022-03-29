namespace Bank

open System
open Bank.Domain

module Operations =

    let deposit amount account =
        { account with Account.Balance = account.Balance + amount }

    let withdraw amount account =
        if account.Balance < amount then
            account
        else
            { account with Account.Balance = account.Balance - amount }

    let auditAs operationName audit operation amount account =
        let createTransaction accepted =
            { Operation = operationName
              Timestamp = DateTime.UtcNow
              Amount = amount
              Accepted = accepted }

        let updatedAccount = operation amount account

        let accountIsUnchanged = (updatedAccount = account)

        if accountIsUnchanged then
            audit account.Id account.Owner.Name (createTransaction false)
        else
            audit account.Id account.Owner.Name (createTransaction true)

        updatedAccount

    let loadAccount (accountId, owner, transactions) =
        let openingAccount =
            { Id = accountId
              Owner = { Name = owner }
              Balance = 0M }

        let action account transaction =
            match transaction.Operation with
            | "d" -> deposit transaction.Amount account
            | "w" -> withdraw transaction.Amount account
            | _ -> account

        transactions
        |> Seq.sortBy (fun txn -> txn.Timestamp)
        |> Seq.fold action openingAccount
