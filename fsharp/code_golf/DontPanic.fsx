// https://www.codingame.com/multiplayer/codegolf/don't-panic

let r=stdin.ReadLine
let t=r().Split ' '
let l=
 Seq.init (int t.[7]) (fun _ ->
  let t1=r().Split ' '
  (int t1.[0],int t1.[1]))|>Seq.append[(int t.[3],int t.[4])]|>Map.ofSeq
while true do
 let i=r().Split ' '
 let f,p,d=int i.[0],int i.[1],i.[2]
 let l=match Map.tryFind f l with|None->0|Some x->x
 if f<0||(d="LEFT"&&l<=p)||(d="RIGHT"&&l>=p) then printfn"WAIT" else printfn"BLOCK"