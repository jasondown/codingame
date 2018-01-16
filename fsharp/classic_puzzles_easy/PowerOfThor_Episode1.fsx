// https://www.codingame.com/training/easy/power-of-thor-episode-1
//----------------------------------------------------------------
let rec gameLoop (coords : int array) =
 let lx, ly, tx, ty = coords.[0], coords.[1], coords.[2], coords.[3]
 let x = compare lx tx
 let y = compare ly ty
 printfn "%s" (["N";"";"S"].[y+1] + ["W";"";"E"].[x+1])
 gameLoop [| lx;ly;tx+x;ty+y |]

stdin.ReadLine().Split ' ' 
|> Array.map int
|> gameLoop