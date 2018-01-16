// https://www.codingame.com/multiplayer/codegolf/power-of-thor

let rec g lx ly tx ty=
 let x,y=compare lx tx,compare ly ty
 printfn"%s"(["N";"";"S"].[y+1]+["W";"";"E"].[x+1])
 g lx ly (tx+x) (ty+y)
let t=(stdin.ReadLine()).Split ' '|>Array.map int
g t.[0] t.[1] t.[2] t.[3]