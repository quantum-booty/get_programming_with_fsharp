open System
open Car

let getDestination () =
    Console.Write("Enter destination: ")
    Console.ReadLine()

let mutable car = { petrol = 100; position = "Home" }

[<EntryPoint>]
let main argv =
    while true do
        try
            let destination = getDestination ()
            printfn "Trying to drive to %s" destination
            car <- car |> driveTo destination
            printfn "Made it to %s! You have %d petrol left" car.position car.petrol
        with
        | ex -> printfn "ERROR: %s" ex.Message

    0
