// https://www.codingame.com/training/easy/mars-lander-episode-1

open System

let surfaceN = int(Console.In.ReadLine())
for i in 0 .. surfaceN - 1 do
    let token = (Console.In.ReadLine()).Split [|' '|]
    let landX = int(token.[0])
    let landY = int(token.[1])
    ()

let reqPower speed =
    match speed with
    | s when s <= -40 -> 4
    | _ -> 0

(* game loop *)
while true do
    let token1 = (Console.In.ReadLine()).Split [|' '|]
    let X = int(token1.[0])
    let Y = int(token1.[1])
    let hSpeed = int(token1.[2])
    let vSpeed = int(token1.[3])
    let fuel = int(token1.[4])
    let rotate = int(token1.[5])
    let power = int(token1.[6])
    
    printfn "0 %i" (reqPower vSpeed)