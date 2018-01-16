// https://www.codingame.com/multiplayer/optimization/code-vs-zombies

(* Save humans, destroy zombies! *)
open System


(* game loop *)
while true do
    let token = (Console.In.ReadLine()).Split [|' '|]
    let x = int(token.[0])
    let y = int(token.[1])
    let humanCount = int(Console.In.ReadLine())
    for i in 0 .. humanCount - 1 do
        let token1 = (Console.In.ReadLine()).Split [|' '|]
        let humanId = int(token1.[0])
        let humanX = int(token1.[1])
        let humanY = int(token1.[2])
        ()

    let zombieCount = int(Console.In.ReadLine())
    for i in 0 .. zombieCount - 1 do
        let token2 = (Console.In.ReadLine()).Split [|' '|]
        let zombieId = int(token2.[0])
        let zombieX = int(token2.[1])
        let zombieY = int(token2.[2])
        let zombieXNext = int(token2.[3])
        let zombieYNext = int(token2.[4])
        ()

    
    (* Write an action using printfn *)
    (* To debug: Console.Error.WriteLine("Debug message") *)
    
    printfn "0 0" (* Your destination coordinates *)
    ()