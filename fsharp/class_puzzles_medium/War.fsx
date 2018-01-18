// https://www.codingame.com/training/medium/winamax-battle

open System
open System.Collections.Generic

let readNLines n = Array.init n (fun _ -> Console.In.ReadLine())

type Rank = 
    | Two
    | Three
    | Four
    | Five 
    | Six
    | Seven 
    | Eight 
    | Nine 
    | Ten
    | Jack 
    | Queen 
    | King 
    | Ace

type Suit = Clubs | Diamonds | Spades | Hearts    
type Card =  Rank * Suit
type Deck = Card Queue
type WarResult = | PlayerOneWins | PlayerTwoWins | Tie
  
let getDigits s = 
   match Int32.TryParse s with
   | (true, x) -> Some(x)
   | (false, _) -> None
    
let (|IsNumber|IsFaceCard|) s =
    match getDigits s with
    | Some _ -> IsNumber
    | None -> IsFaceCard
      
let createCard (s : string) =
    let rankOnly = s.Substring(0, s.Length - 1)
    let rank = 
        match rankOnly with
        | IsNumber -> 
            match int rankOnly with 
            | 2 -> Two
            | 3 -> Three
            | 4 -> Four
            | 5 -> Five
            | 6 -> Six
            | 7 -> Seven
            | 8 -> Eight
            | 9 -> Nine
            | 10 -> Ten
            | _ -> failwith "Rank number out of range."
        | IsFaceCard ->
            match rankOnly with
            | "J" -> Jack
            | "Q" -> Queen
            | "K" -> King
            | "A" -> Ace
            | _ -> failwithf "Rank not recognized: %s" rankOnly
    let suit =
        match s.[s.Length - 1] with
        | 'C' -> Clubs
        | 'D' -> Diamonds
        | 'S' -> Spades
        | 'H' -> Hearts
        | _ -> failwith "String format of card is incorrect."

    (rank, suit)

let getCardValue card =
    match card with
    | (Two, _) -> 2
    | (Three, _) -> 3
    | (Four, _) -> 4
    | (Five, _) -> 5
    | (Six, _) -> 6
    | (Seven, _) -> 7
    | (Eight, _) -> 8
    | (Nine, _) -> 9
    | (Ten, _) -> 10
    | (Jack, _) -> 11
    | (Queen, _) -> 12
    | (King, _) -> 13
    | (Ace, _) -> 14

let createDeck numCards (deck : Deck) = 
    numCards 
    |> readNLines 
    |> Array.map (fun c -> 
        let card = createCard c
        deck.Enqueue card)
    |> ignore
    deck

let getWinner p1Card p2Card =
    match getCardValue p1Card, getCardValue p2Card with
    | p1, p2 when p1 > p2 -> WarResult.PlayerOneWins
    | p1, p2 when p2 > p1 -> WarResult.PlayerTwoWins
    | _ -> WarResult.Tie

let matchUp (p1 : Deck) (p2 : Deck) = 
    let p1CardsPlayed = new Deck()
    let p2CardsPlayed = new Deck()
    let rec matchup' (p1Deck : Deck) (p2Deck : Deck) (acc1 : Deck) (acc2 : Deck) =
        let p1Card = p1Deck.Dequeue()
        let p2Card = p2Deck.Dequeue()
        acc1.Enqueue(p1Card)
        acc2.Enqueue(p2Card)
        match getWinner p1Card p2Card with
        | PlayerOneWins -> while acc1.Count > 0 do p1Deck.Enqueue(acc1.Dequeue())
                           while acc2.Count > 0 do p1Deck.Enqueue(acc2.Dequeue())
                           PlayerOneWins
        | PlayerTwoWins -> while acc1.Count > 0 do p2Deck.Enqueue(acc1.Dequeue())
                           while acc2.Count > 0 do p2Deck.Enqueue(acc2.Dequeue())
                           PlayerTwoWins 
        | Tie -> if p1Deck.Count <= 3 || p2Deck.Count <= 3 then
                    Tie
                 else
                    for i in 1 .. 3 do 
                        acc1.Enqueue(p1Deck.Dequeue())
                        acc2.Enqueue(p2Deck.Dequeue())
                    matchup' p1Deck p2Deck acc1 acc2
        
    matchup' p1 p2 p1CardsPlayed p2CardsPlayed
    
let playGame (p1 : Deck) (p2 : Deck) =
    let mutable tied = false
    let mutable round = 0
    while p1.Count > 0 && p2.Count > 0 && tied |> not do
        match matchUp p1 p2 with
        | PlayerOneWins | PlayerTwoWins -> round <- round + 1
        | Tie -> tied <- true
        
    if tied then printf "PAT"
    elif p1.Count > 0 then printf "1 %i" round
    elif p2.Count > 0 then printf "2 %i" round
    else failwith "Gamed state unknown."

// Main logic starts here
let numP1Cards = int(Console.In.ReadLine())
let p1Deck = new Deck(numP1Cards) |> createDeck numP1Cards
let numP2Cards = int(Console.In.ReadLine())
let p2Deck = new Deck(numP2Cards) |> createDeck numP2Cards

playGame p1Deck p2Deck