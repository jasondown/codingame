// https://www.codingame.com/multiplayer/codegolf/temperature-code-golf

let f = stdin.ReadLine
f()|>function|"0"->printf"0"|_->f().Split ' '|>Seq.map int|>Seq.sortBy(fun x->abs x,-x)|>Seq.head|>printf"%d"