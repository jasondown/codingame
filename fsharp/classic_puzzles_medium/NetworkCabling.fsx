// https://www.codingame.com/training/medium/network-cabling

open System

type Building = {X : int64; Y: int64}

let getMainLineY buildings = 
    let medianYBuilding = 
        buildings 
        |> Array.sortBy (fun b -> b.Y)
        |> Array.item ((buildings.Length - 1) / 2)
    medianYBuilding.Y    
    
let getTotalYLengths buildings =
    let mainLine = buildings |> getMainLineY
    buildings
    |> Array.fold (fun total b -> 
        total + 
        (match b.Y with
         | y when y > mainLine -> Math.Abs(mainLine - y)
         | y when y < mainLine -> Math.Abs(y - mainLine)
         | _ -> 0L)) 0L

let getMainLineLength buildings =
    let minX = buildings |> Array.minBy (fun b -> b.X) 
    let maxX = buildings |> Array.maxBy (fun b -> b.X)
    Math.Abs (minX.X - maxX.X)

let getTotalLength buildings =
    (buildings |> getMainLineLength) + (buildings |> getTotalYLengths)

let getBuildings numBuildings = 
    Array.init numBuildings (fun _ -> Console.ReadLine())
    |> Array.map (fun l ->
        let token = l.Split[|' '|]
        { X = int64 <| token.[0]
          Y = int64 <| token.[1] })
    |> Array.sortByDescending (fun b -> b.X)

let getRequiredCableLength = getBuildings >> getTotalLength

// Main Logic
Console.ReadLine() |> int |> getRequiredCableLength |> printfn "%i"