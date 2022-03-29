namespace Bank

open Bank.Domain
open Bank.FileRepository

module Auditing =
    /// Logs to the console
    let printTransaction accountId _ (transaction: Transaction) =
        printfn "Account %O: %s" accountId (serialized transaction)

    // Logs to both console and file system
    let composedLogger =
        let loggers =
            [ FileRepository.writeTransaction
              printTransaction ]

        fun accountId owner message ->
            loggers
            |> List.iter (fun logger -> logger accountId owner message)
