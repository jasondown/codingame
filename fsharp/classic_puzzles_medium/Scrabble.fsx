// https://www.codingame.com/training/medium/scrabble

open System

let pointValues = 
    dict ['e',1; 'a',1; 'i',1; 'o',1; 'n',1; 'r',1; 't',1; 'l',1; 's',1; 'u',1;
          'd',2; 'g',2;
          'b',3; 'c',3; 'm',3; 'p',3;
          'f',4; 'h',4; 'v',4; 'w',4; 'y',4;
          'k',5;
          'j',8; 'x',8;
          'q',10; 'z',10; 
    ]

let readNLines n = Array.init n (fun _ -> Console.ReadLine())

let getWordsAndLetters wordCount =
    let words = readNLines (wordCount - 1)
    let letters = readNLines 1 |> Array.head
    let validWords =
        words
        |> Array.filter (fun w -> w.Length <= 7)
    (validWords, letters)

let words, letters = 
    Console.ReadLine()
    |> int
    |> getWordsAndLetters
