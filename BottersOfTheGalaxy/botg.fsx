open System

type RoundType = SelectHero | CommandHeroes of int

type EntityType = Bush | Spawn

type Point = { X : float; Y : float }

type Entity = 
    { EntityType : EntityType
      Point      : Point
      Radius     : int }

type Item =
    { Name      : string
      Cost      : int
      Damage    : int
      Health    : int
      MaxHealth : int
      Mana      : int
      MaxMana   : int
      MoveSpeed : int
      ManaRegen : int
      IsPotion  : bool }

type UnitType = 
    Unit | Hero | Tower | Groot
    override x.ToString() =
        match x with
        | Unit -> "UNIT"
        | Hero -> "HERO"
        | Tower -> "TOWER"
        | Groot -> "GROOT"

type HeroType = 
    Deadpool | DoctorStrange | Hulk | Ironman | Valkyrie | NotHero
    override x.ToString() =
        match x with
        | Deadpool -> "DEADPOOL"
        | DoctorStrange -> "DOCTOR_STRANGE"
        | Hulk -> "HULK"
        | Ironman -> "IRONMAN"
        | Valkyrie -> "VALKYRIE"
        | NotHero -> ""

type Unit =
    { UnitId : int
      Team          : int
      UnitType      : UnitType
      Point         : Point
      AttackRange   : float
      Health        : int
      MaxHealth     : int
      Shield        : int
      AttackDamage  : int
      MovementSpeed : float
      StunDuration  : int
      GoldValue     : int
      CountDown1    : int
      CountDown2    : int
      CountDown3    : int
      Mana          : int
      MaxMana       : int
      ManaRegen     : int
      HeroType      : HeroType
      IsVisible     : bool
      ItemsOwned    : int }

type Move = 
    | Move of Point 
    | MoveAttack of Point * int
    | Attack of int
    | AttackNearest of UnitType
    | Buy of Item
    | Wait 
    | PickHero of HeroType
    override x.ToString() =
        match x with
        | Move p -> sprintf "MOVE %f %f" p.X p.Y
        | MoveAttack (p, id) -> sprintf "MOVE_ATTACK %f %f %i" p.X p.Y id
        | Attack id -> sprintf "ATTACK %i" id
        | AttackNearest ut -> sprintf "ATTACK_NEAREST %s" (ut.ToString())
        | Buy i -> sprintf "BUY %s" i.Name
        | Wait -> "WAIT"
        | PickHero h -> h |> string

let mutable gold = 0

//----------Helper Functions
let readNLines n = List.init n (fun _ -> Console.ReadLine())

let getUnitType s =
    match s with
    | "UNIT" -> Unit
    | "HERO" -> Hero
    | "TOWER" -> Tower
    | "GROOT" -> Groot
    | s -> failwithf "Unknown unit type %s" s

let getHeroType s =
        match s with
        | "DEADPOOL" -> Deadpool
        | "DOCTOR_STRANGE" -> DoctorStrange
        | "HULK" -> Hulk
        | "IRONMAN" -> Ironman
        | "VALKYRIE" -> Valkyrie
        | _ -> NotHero

let getDist p1 p2 =
    let x = (p1.X - p2.X) * (p1.X - p2.X)
    let y = (p1.Y - p2.Y) * (p1.Y - p2.Y)
    let dist = (x + y) |> sqrt
    dist

let isSafeDistance (enemyTower : Unit) (hero : Unit) (target : Point) =
    let distFromTower = getDist target enemyTower.Point
    let rangeAndSpeed =  enemyTower.AttackRange + hero.MovementSpeed
    distFromTower > rangeAndSpeed

let backup myTeam (x:float) (x1:float) =
    if myTeam = 0 then x-x1 else x+x1

let inFrontOf myTeam x1 x2 dist =
    if myTeam = 0 
    then x1 > x2-dist
    else x1 < x2+dist

let buyItem (items : Item list) =
    let item = items |> List.maxBy (fun i -> i.Damage * 7 + i.Health * 4 + i.Mana * 2)
    gold <- gold - item.Cost
    Move.Buy item
//----------

let myTeam = Console.ReadLine() |> int
let entities =
    Console.ReadLine() |> int |> readNLines
    |> List.map (fun l -> 
        let token = l.Split ' '
        let entityType = 
            match token.[0] with
            | "BUSH" -> Bush
            | "SPAWN" -> Spawn
            | x -> failwithf "Unknown span type %s" x
        { EntityType = entityType
          Point      = { X = float token.[1]; Y = float token.[2] }
          Radius     = int token.[3] })
    
let items =
    Console.ReadLine() |> int |> readNLines
    |> List.map (fun l ->
        let token = l.Split ' '
        { Name      = token.[0]
          Cost      = int token.[1]
          Damage    = int token.[2]
          Health    = int token.[3]
          MaxHealth = int token.[4]
          Mana      = int token.[5]
          MaxMana   = int token.[6]
          MoveSpeed = int token.[7]
          ManaRegen = int token.[8]
          IsPotion  = (int token.[9]) = 1 })

(* game loop *)
while true do
    gold <- Console.ReadLine() |> int
    let enemyGold = Console.ReadLine() |> int
    let roundType = 
        match Console.ReadLine() |> int with
        | n when n < 0 -> SelectHero
        | n            -> CommandHeroes n

    let units =
        Console.ReadLine() |> int |> readNLines
        |> List.map (fun l -> 
            let token = l.Split ' '
            { UnitId        = int token.[0]
              Team          = int token.[1]
              UnitType      = getUnitType token.[2]
              Point         = { X = float token.[3]; Y = float token.[4] }
              AttackRange   = float token.[5]
              Health        = int token.[6]
              MaxHealth     = int token.[7]
              Shield        = int token.[8]
              AttackDamage  = int token.[9]
              MovementSpeed = float token.[10]
              StunDuration  = int token.[11]
              GoldValue     = int token.[12]
              CountDown1    = int token.[13]
              CountDown2    = int token.[14]
              CountDown3    = int token.[15]
              Mana          = int token.[16]
              MaxMana       = int token.[17]
              ManaRegen     = int token.[18]
              HeroType      = getHeroType token.[19]
              IsVisible     = int token.[20] <> 0
              ItemsOwned    = int token.[21] })        

    let getMove =
        let myHero = units |> List.tryFind (fun u -> u.Team = myTeam && u.UnitType = UnitType.Hero)
        let enemyHero = units |> List.tryFind (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Hero)
        let myTower = units |> List.find (fun u -> u.Team = myTeam && u.UnitType = UnitType.Tower)
        let enemyTower = units |> List.find (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Tower)
        let myUnits = units |> List.filter (fun u -> u.Team = myTeam && u.UnitType = UnitType.Unit)
        let enemyUnits = units |> List.filter (fun u -> u.Team <> myTeam && u.UnitType = UnitType.Unit)
        let affordableItems = items |> List.filter (fun i -> i.Cost < gold && not i.IsPotion)
        let isSafeDist = isSafeDistance enemyTower
        Console.Error.WriteLine(sprintf "gold: %i" gold)
        Console.Error.WriteLine(sprintf "affordable items: %i" affordableItems.Length)
        match myHero, enemyHero, myUnits.Length, enemyUnits.Length, affordableItems.Length with
        | None, _, _, _, _ -> Move.Wait
        | Some hero, _, _, _, ai when ai > 0 && hero.ItemsOwned < 4 ->
            buyItem affordableItems
        | Some hero, _, mu, _, _ when mu > 1 ->
            let heroRange = hero.AttackRange + hero.MovementSpeed * 0.3
            let myFrontUnit = myUnits |> List.maxBy (fun u -> if hero.Team = 0 then u.Point.X else -u.Point.X)
            let potentialMove = { X = backup hero.Team myFrontUnit.Point.X 25.; Y = hero.Point.Y }
            let lowestEnemy = enemyUnits |> List.tryFind (fun eu -> eu.Health < hero.AttackDamage && getDist potentialMove eu.Point < heroRange)
            let closestEnemy = enemyUnits |> List.tryFind (fun eu -> getDist potentialMove eu.Point < heroRange)
            if isSafeDist hero potentialMove then
                match lowestEnemy, closestEnemy with
                | Some le, _ -> 
                    Move.MoveAttack (potentialMove, le.UnitId)
                | _, Some ce -> 
                    Move.MoveAttack (potentialMove, ce.UnitId)
                | _ -> Move.Move potentialMove
            else 
                Move.Move myTower.Point
        | _ -> Move.Move myTower.Point

    let move =
        match roundType with
        | SelectHero -> Move.PickHero HeroType.Valkyrie
        | CommandHeroes _ -> getMove

    printfn "%s" (move |> string)
    ()