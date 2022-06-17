open System

type Token = string array

type GamePhase =
    | Move
    | Release
    static member Create =
        function
        | "MOVE" -> Move
        | "RELEASE" -> Release
        | x -> failwithf "Unknown Game Phase: %s" x

type Skill =
    | Training
    | Coding
    | DailyRoutine
    | TaskPrioritization
    | ArchitectureStudy
    | ContinuousIntegration
    | CodeReview
    | Refactoring
    static member Create =
        function
        | 0 -> Training
        | 1 -> Coding
        | 2 -> DailyRoutine
        | 3 -> TaskPrioritization
        | 4 -> ArchitectureStudy
        | 5 -> ContinuousIntegration
        | 6 -> CodeReview
        | 7 -> Refactoring
        | x -> failwithf "Unknown Skill: %i" x

type DeskLocation = 
    | None
    | Skill of Skill
    static member Create =
        function
        | -1 -> None
        | x when x >= 0 && x <= 7 -> Skill (Skill.Create x)
        | x -> failwithf "Unknown Desk Location: %i" x

type CardType =
    | Skill of Skill
    | Bonus
    | TechnicalDebt

type CardLocation =
    | Hand
    | Draw
    | Discard
    | OpponentCards
    static member Create = 
        function 
        | "HAND" -> Hand
        | "DRAW" -> Draw
        | "DISCARD" -> Discard
        | "OPPONENT_CARDS" -> OpponentCards
        | x -> failwithf "Unknown Card Location: %s" x

type Card =
    {
        Location: CardLocation
        Type: CardType
    }

type Application =
    {
        Id: int
        RequiredSkills: Map<Skill, int>
    }
    static member Create (token: Token) =
        {
            Id = int token.[1]
            RequiredSkills = 
                Map.empty
                    .Add(Training, int token.[2])
                    .Add(Coding, int token.[3])
                    .Add(DailyRoutine, int token.[4])
                    .Add(TaskPrioritization, int token.[5])
                    .Add(ArchitectureStudy, int token.[6])
                    .Add(ContinuousIntegration, int token.[7])
                    .Add(CodeReview, int token.[8])
                    .Add(Refactoring, int token.[9])
        }

type Player =
    {
        Location: DeskLocation
        ReleasedApps: int
        PermanentDailyRoutineCards: int // ignore for Wood 2
        PermanentArchitectureStudyCards: int // ignore for Wood 2
    }
    static member Create (token: Token) =
        {
            Location = DeskLocation.Create (int token.[0])
            ReleasedApps = int token.[1]
            PermanentDailyRoutineCards = int token.[2]
            PermanentArchitectureStudyCards = int token.[3]
        }

type Move =
    | Move of int
    | Release of int
    | Random
    | Wait
    override this.ToString() =
        match this with
        | Move i -> sprintf "MOVE %i" i
        | Release i -> sprintf "RELEASE %i" i
        | Random -> sprintf "RANDOM"
        | Wait -> sprintf "WAIT"
    static member Create (s: string) =
        match s.Split ' ' |> Array.toList with
        | "MOVE" :: i :: _ -> Move (int i)
        | "RELEASE" :: i :: _ -> Release (int i)
        | "WAIT" :: _ -> Wait
        | "RANDOM" :: _ -> Random
        | x -> failwithf "Unrecognized Move: %A" x

type Gamestate = 
    {
        Phase: GamePhase
        Me: Player
        Opponent: Player
        Applications: Application list
        Cards: (Card * int) list
        AvailMoves: Move list
    }

let readInput () = Console.In.ReadLine()
let readInt = readInput >> int
let tokenize (line : string) : Token = line.Split ' '
let tokenizeInput = readInput >> tokenize
let readNLines n = List.init n (fun _ -> readInput())

let createCards (token: Token) =
    let location = CardLocation.Create token.[0]
    let skillCards =
        token.[1..8]
        |> Array.mapi (fun i n -> CardType.Skill (Skill.Create i), int n)
        |> Array.map (fun (s, n) -> { Card.Location = location; Card.Type = s }, n)
        |> List.ofArray
    let bonusCards = { Card.Location = location; Card.Type = Bonus}, (int token.[9])
    let techDebtCards = { Card.Location = location; Card.Type = TechnicalDebt}, (int token.[10]    )
    techDebtCards :: bonusCards :: skillCards |> List.rev

let getMove (gs: Gamestate) : Move =
    gs.AvailMoves.Head

    // let myCards =
    //     cards |> List.filter (fun c -> 
    //         let card = fst c
    //         card.Location = Hand)

    // let drawCards = 
    //     cards |> List.filter (fun c ->
    //         let card = fst c
    //         card.Location = Draw)

    // let discardCards =
    //     cards |> List.filter (fun c ->
    //         let card = fst c
    //         card.Location = Discard)

    // let oppCards =
    //     cards |> List.filter (fun c ->
    //         let card = fst c
    //         card.Location = OpponentCards)

while true do
    let gamePhase = readInput() |> GamePhase.Create

    let apps =
        readInt()
        |> readNLines
        |> List.map (tokenize >> Application.Create)

    let myInfo = tokenizeInput() |> Player.Create

    let oppInfo = tokenizeInput() |> Player.Create

    let cards =
        readInt()
        |> readNLines
        |> List.collect (tokenize >> createCards)

    let possibleMoves =
        readInt()
        |> readNLines
        |> List.map Move.Create

    let gameState = 
        {
            Phase = gamePhase
            Me = myInfo
            Opponent = oppInfo
            Applications = apps
            Cards = cards
            AvailMoves = possibleMoves
        }

    gameState
    |> getMove
    |> string
    |> printfn "%s" 
