// https://www.codingame.com/training/easy/onboarding

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {

        // game loop
        while (true)
        {
            string enemy1 = Console.ReadLine(); 
            int dist1 = int.Parse(Console.ReadLine());
            string enemy2 = Console.ReadLine();
            int dist2 = int.Parse(Console.ReadLine());

            Console.WriteLine(dist1 < dist2 ? enemy1 : enemy2);
        }
    }
}