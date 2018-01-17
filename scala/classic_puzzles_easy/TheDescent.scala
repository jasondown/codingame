// https://www.codingame.com/training/easy/the-descent/solution

import math._
import scala.util._

object Player extends App {

    // game loop
    while(true) {
        val mountain = (0 to 7).map(_ => readInt)
                               .toList
                               .zipWithIndex
                               .maxBy(_._1)._2
                
        println(mountain) // The index of the mountain to fire on.
    }
}