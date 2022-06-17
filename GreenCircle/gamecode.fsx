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

    override this.ToString() =
        match this with
        | Training -> "0"
        | Coding -> "1"
        | DailyRoutine -> "2"
        | TaskPrioritization -> "3"
        | ArchitectureStudy -> "4"
        | ContinuousIntegration -> "5"
        | CodeReview -> "6"
        | Refactoring -> "7"

type DeskLocation =
    | None
    | Skill of Skill
    static member Create =
        function
        | -1 -> None
        | x when x >= 0 && x <= 7 -> Skill(Skill.Create x)
        | x -> failwithf "Unknown Desk Location: %i" x

    override this.ToString() =
        match this with
        | Skill s -> s.ToString()
        | None -> ""

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
    { Location: CardLocation
      Type: CardType }

type Application =
    { Id: int
      RequiredSkills: Map<Skill, int> }
    static member Create(token: Token) =
        { Id = int token.[1]
          RequiredSkills =
            Map
                .empty
                .Add(Training, int token.[2])
                .Add(Coding, int token.[3])
                .Add(DailyRoutine, int token.[4])
                .Add(TaskPrioritization, int token.[5])
                .Add(ArchitectureStudy, int token.[6])
                .Add(ContinuousIntegration, int token.[7])
                .Add(CodeReview, int token.[8])
                .Add(Refactoring, int token.[9]) }

type Player =
    { Location: DeskLocation
      ReleasedApps: int
      PermanentDailyRoutineCards: int // ignore for Wood 2
      PermanentArchitectureStudyCards: int } // ignore for Wood 2
    static member Create(token: Token) =
        { Location = DeskLocation.Create(int token.[0])
          ReleasedApps = int token.[1]
          PermanentDailyRoutineCards = int token.[2]
          PermanentArchitectureStudyCards = int token.[3] }

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

    static member Create(s: string) =
        match s.Split ' ' |> Array.toList with
        | "MOVE" :: i :: _ -> Move(int i)
        | "RELEASE" :: i :: _ -> Release(int i)
        | "WAIT" :: _ -> Wait
        | "RANDOM" :: _ -> Random
        | x -> failwithf "Unrecognized Move: %A" x

type Gamestate =
    { Phase: GamePhase
      Me: Player
      Opponent: Player
      Applications: Application list
      Cards: (Card * int) list
      AvailMoves: Move list }

let readInput () = Console.In.ReadLine()
let readInt = readInput >> int
let tokenize (line: string) : Token = line.Split ' '
let tokenizeInput = readInput >> tokenize
let readNLines n = List.init n (fun _ -> readInput ())

let createCards (token: Token) =
    let location = CardLocation.Create token.[0]

    let skillCards =
        token.[1..8]
        |> Array.mapi (fun i n -> CardType.Skill(Skill.Create i), int n)
        |> Array.map (fun (s, n) ->
            { Card.Location = location
              Card.Type = s },
            n)
        |> List.ofArray

    let bonusCards =
        { Card.Location = location
          Card.Type = Bonus },
        (int token.[9])

    let techDebtCards =
        { Card.Location = location
          Card.Type = TechnicalDebt },
        (int token.[10])

    techDebtCards :: bonusCards :: skillCards
    |> List.rev

let getTargetApp (gs: Gamestate) (myCards: (Card * int) list) =
    let apps = gs.Applications

    let requiredSkills =
        apps
        |> List.map (fun a ->
            let rs = a.RequiredSkills |> Map.filter (fun _ i -> i > 0)
            { Id = a.Id; RequiredSkills = rs })

    let getScore (rs: Map<Skill, int>) (cards: Card list) =
        let rec score (cardList: Card list) acc =
            match cardList with
            | [] -> acc
            | c :: cs ->
                match c.Type with
                | Skill s ->
                    if rs.ContainsKey(s) then
                        score cs (acc + 10)
                    else
                        score cs acc
                | Bonus -> score cs (acc + 1)
                | TechnicalDebt -> score cs acc

        score cards 0

    let targetApp =
        requiredSkills
        |> List.sortByDescending (fun rs -> getScore rs.RequiredSkills (myCards |> List.map fst))
        |> List.head

    targetApp

let getTargetDesk (gs: Gamestate) (targetApp: Application) (myCards: (Card * int) list) =
    let skillCards =
        myCards
        |> List.map (fun (c, n) -> c.Type, n)
        |> List.choose (fun (c, _) ->
            match c with
            | Skill s -> Some s
            | _ -> Option.None)

    let x =
        targetApp.RequiredSkills
        |> Map.filter (fun sk _ -> skillCards |> List.exists (fun s -> s = sk) |> not)
        |> Seq.tryHead
        |> Option.defaultValue (Collections.Generic.KeyValuePair.Create(Training, 0))

    DeskLocation.Skill x.Key


let getMove (gs: Gamestate) : Move =

    let myCards =
        gs.Cards
        |> List.filter (fun c ->
            let card = fst c
            card.Location = Hand && (snd c) > 0)

    let targetApp = getTargetApp gs myCards

    let canMake =
        gs.AvailMoves
        |> List.exists (fun am ->
            match am with
            | Release i when i = targetApp.Id -> true
            | _ -> false)

    if canMake then
        Release targetApp.Id
    else
        let targetDesk = getTargetDesk gs targetApp myCards

        if targetDesk <> gs.Me.Location then
            Move(int <| targetDesk.ToString())
        else
            Random

while true do
    let gamePhase = readInput () |> GamePhase.Create

    let apps =
        readInt ()
        |> readNLines
        |> List.map (tokenize >> Application.Create)

    let myInfo = tokenizeInput () |> Player.Create

    let oppInfo = tokenizeInput () |> Player.Create

    let cards =
        readInt ()
        |> readNLines
        |> List.collect (tokenize >> createCards)

    let possibleMoves = readInt () |> readNLines |> List.map Move.Create

    let gameState =
        { Phase = gamePhase
          Me = myInfo
          Opponent = oppInfo
          Applications = apps
          Cards = cards
          AvailMoves = possibleMoves }

    gameState |> getMove |> string |> printfn "%s"
