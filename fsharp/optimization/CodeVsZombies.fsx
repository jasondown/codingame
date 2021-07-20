// https://www.codingame.com/multiplayer/optimization/code-vs-zombies
//-------------------------------------------------------------------
open System

module Types =

    type Point = 
        {
            X: float
            Y: float
        }
        member this.ToInts() =
            (Convert.ToInt32(this.X), Convert.ToInt32(this.Y))

    type Player =
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

    type GameState =
        {
            Player: Player
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

    let getMove (gs: GameState) =
        
        let me = gs.Player

        let distHumansToMe = 
            gs.Humans
            |> List.map (fun h -> (getDist h.Point me.Point), h)
            |> List.sortBy fst
        
        let distHumansToZombiesAndHumansToMe =
            gs.Humans
            |> List.map (fun h ->
                gs.Zombies
                |> List.map (fun z -> (getDist h.Point z.Point), (getDist h.Point me.Point), h, z))
            |> List.concat
            |> List.sortBy (fun (_, distToMe, _, _) -> distToMe)
        
        // // move to closest human where I am closer than the closest zombie
        // let candidates =
        //     distHumansToZombiesAndHumansToMe
        //     |> List.filter (fun (d2z, d2m, h, z) -> d2z >= d2m)
        //     |> List.map (fun (_, _, h, _) -> h)
            
        // move to human where the human to me distance vs human to zombie distance is the least
        let candidates =
            distHumansToZombiesAndHumansToMe
            |> List.sortBy (fun (d2z, d2m, h, z) -> 
                let distdiff = abs(d2z - d2m)
                eprintfn "%.0f %i %i" distdiff h.Id z.Id
                distdiff)
            |> List.map (fun (_, _, h, _) -> h)

        candidates |> List.iter (fun c -> eprintfn "%i" c.Id)

        candidates.Head.Point


module Main =

    open Types
    open Helpers
    open Move

    let mutable previousMove: Point option = None

    (* game loop *)
    while true do
    
        let player  = 1 |> create Player.Create |> List.head
        let humans  = readInt() |> create Human.Create
        let zombies = readInt() |> create Zombie.Create

        let gameState =
            {
                Player = player
                Humans = humans
                Zombies = zombies
            }
                
        let move =
            match previousMove with
            | Some m -> 
                let humans = gameState.Humans
                let targetHumanDied =
                    humans
                    |> List.filter (fun h -> h.Point = m)
                    |> List.isEmpty

                if targetHumanDied then
                    let move' = getMove gameState
                    previousMove <- Some(move')
                    move'
                else
                    m

            | None -> 
                let move' = getMove gameState
                previousMove <- Some(move')
                move'

        let command = move.ToInts()

        printfn "%i %i" (fst command) (snd command)