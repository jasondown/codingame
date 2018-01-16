// https://www.codingame.com/training/easy/horse-racing-duals

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine());
        var horseStrengths = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            horseStrengths.Add(int.Parse(Console.ReadLine()));
        }

        horseStrengths.Sort();

        Console.WriteLine(ClosestHorsesDelta(horseStrengths));
    }

    static int ClosestHorsesDelta(IList<int> horses)
    {
        var closestDiff = Int32.MaxValue;
        var lastHorse = Int32.MaxValue;

        for (int i = 0; i < horses.Count; i++)
        {
            var currHorse = horses[i];
            var diff = Math.Abs(currHorse - lastHorse);
            if (diff < closestDiff)
            {
                closestDiff = diff;
            }

            lastHorse = currHorse;
        }

        return closestDiff;
    }
}