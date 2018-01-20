using System;
using System.Linq;
using System.Collections.Generic;

public class Player
{
    static void Main(string[] args)
    {
        // game loop
        while (true)
        {
            var mountains = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                mountains.Add(int.Parse(Console.ReadLine()));
            }
            Console.WriteLine(mountains.IndexOf(mountains.Max()));
        }
    }
}