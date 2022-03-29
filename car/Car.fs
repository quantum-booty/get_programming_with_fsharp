module Car

open System

type Vehical = { petrol: int; position: string }

let getDistance destination =
    match destination with
    | "Home" -> 25
    | "Office" -> 50
    | "Stadium" -> 25
    | "Gas station" -> 10
    | _ -> failwith "Destination not found"

let calculateRemainingPetrol distance car =
    if car.petrol < distance then
        failwith "Not enough petrol"
    else
        { car with petrol = car.petrol - distance }

let addFuel destination car =
    match destination with
    | "Gas station" -> { car with petrol = car.petrol + 50 }
    | _ -> car

let updateDestination destination car = { car with position = destination }

let driveTo destination car =
    if car.position = destination then
        car
    else
        car
        |> calculateRemainingPetrol (getDistance destination)
        |> addFuel destination
        |> updateDestination destination
