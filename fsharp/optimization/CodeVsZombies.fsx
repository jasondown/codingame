// https://www.codingame.com/multiplayer/optimization/code-vs-zombies
//-------------------------------------------------------------------
open System

module Types =

    type Point = 
        {
            X: float
            Y: float
        }
        static member ToInts point =
            (Convert.ToInt32(point.X), Convert.ToInt32(point.Y))

    type Ash =
        {
            Point: Point
        }
        static member Create (token: string array) =
            { Point =
                { 
                    X = token.[0] |> float
                    Y = token.[1] |> float
                } 
            }

    type Human =
        {
            Id: int
            Point: Point
        }
        static member Create (token: string array) =
            {
                Id = token.[0] |> int
                Point = 
                    {
                        X =  token.[1] |> float
                        Y =  token.[2] |> float
                    }
            }

    type Zombie =
        {
            Id: int
            Point: Point
            NextPoint: Point
        }
        static member Create (token: string array) =
            {
                Id    = token.[0] |> int
                Point =
                    {
                        X     = token.[1] |> float
                        Y     = token.[2] |> float
                    }
                NextPoint =
                    {
                        X = token.[3] |> float
                        Y = token.[4] |> float
                    }
            }

    type Player = 
        | Ash of Ash
        | Human of Human
        | Zombie of Zombie

    type GameState =
        {
            Ash: Ash
            Humans: Human list
            Zombies: Zombie list
        }

module Helpers =

    let readInput () = Console.ReadLine()
    let readInt = readInput >> int
    let tokenize (line : string) = line.Split ' '
    let tokenizeInput = readInput >> tokenize
    let readNLines n = Array.init n (fun _ -> readInput())

    let create (create: (string array -> 'a)) (num: int)=
            num
            |> readNLines
            |> Array.map (tokenize >> create)
            |> Array.toList

module Move =

    open Types

    let getDist p1 p2 =
        let x = (p1.X - p2.X) * (p1.X - p2.X)
        let y = (p1.Y - p2.Y) * (p1.Y - p2.Y)
        let dist = (x + y) |> sqrt
        dist

    let timeOnTarget (target: Player) (source: Player) =
        match target, source with 
        | Human h, Zombie z ->
            (getDist z.Point h.Point ) / 400.
        | Zombie z, Ash a ->
            ((getDist a.Point z.Point) - 2000.) / 1000.
        | x, y -> failwithf "ToT not implemented for %A %A" x y

    let getSaveableHuman (me: Ash) (humans: Human list) (zombies: Zombie list) =
        humans
        |> List.filter (fun h ->
            let zombieWinsRace = 
                zombies
                |> List.tryFind (fun z ->
                    let ashTimeToZombie = timeOnTarget (Zombie z) (Ash me)
                    let zombieTimeToHuman = timeOnTarget (Human h) (Zombie z)
                    ashTimeToZombie > zombieTimeToHuman)
            match zombieWinsRace with
            | Some _ -> false
            | None -> true)
        |> List.tryHead

    let getClosestZombie (me: Ash) (zombies: Zombie list) =
        zombies
        |> List.sortBy (fun z -> getDist me.Point z.Point)
        |> List.tryHead

    let getMove (gs: GameState) =
        
        let me = gs.Ash
        let humans = gs.Humans
        let zombies = gs.Zombies

        let saveableHuman = getSaveableHuman me humans zombies
        let closestZombie = getClosestZombie me zombies
        let stillSaveableHuman =
            match closestZombie with
            | Some cz ->
                let saveableHumanIfGoForZombie =
                    let zombiesNextMove =
                        zombies
                        |> List.map (fun z -> {z with Point = z.NextPoint; })
                    let ashNextMove =
                        let D = sqrt (Math.Pow(cz.Point.X - me.Point.X, 2.) + (Math.Pow(cz.Point.Y - me.Point.Y, 2.)))
                        let d = 1000.
                        let newX = me.Point.X + (d/D) * (cz.Point.X - me.Point.X)
                        let newY = me.Point.Y + (d/D) * (cz.Point.Y - me.Point.Y)
                        {me with Point = {X = newX; Y = newY}}
                    getSaveableHuman ashNextMove humans zombiesNextMove

                match saveableHumanIfGoForZombie with
                | Some _ -> true
                | None -> false
            | None -> true

        match stillSaveableHuman, closestZombie, saveableHuman with
        | true, Some cz, _ -> cz.Point
        | false, _, Some sh -> sh.Point
        | _, None, Some sh -> sh.Point
        | _ -> { X=0.; Y=0.}

module Main =

    open Types
    open Helpers
    open Move

    (* game loop *)
    while true do
    
        let ash  = 1 |> create Ash.Create |> List.head
        let humans  = readInt() |> create Human.Create
        let zombies = readInt() |> create Zombie.Create

        let gameState =
            {
                Ash = ash
                Humans = humans
                Zombies = zombies
            }
                
        gameState |> getMove |> Point.ToInts||> printfn "%i %i"
