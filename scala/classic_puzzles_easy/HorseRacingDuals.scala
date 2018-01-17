// https://www.codingame.com/training/easy/horse-racing-duals/solution

import math._
import scala.util._

object Solution extends App {
    val n = readInt
    val horses = (1 to n).map(_ => readInt).toList.sorted
    
    println(horses.tail.zip(horses).map(h => h._1-h._2).min)
}