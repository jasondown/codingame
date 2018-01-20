// https://www.codingame.com/training/medium/stock-exchange-losses

// This is a dirty solution global state mutation.
// But I just wanted to see if I could get it to work in a map
// operation.
open System
let mutable j = -1

let setval p =
    if j < p then j <- p else j <- j |> ignore
    j

let getMaxLoss values =  
    values
    |> Array.map (Int32.Parse >> (fun p -> p - (setval p)))
    |> Array.min

let _ = int(Console.In.ReadLine()) // don't even need this value

(Console.In.ReadLine()).Split [|' '|] 
|> getMaxLoss 
|> printfn "%i"
