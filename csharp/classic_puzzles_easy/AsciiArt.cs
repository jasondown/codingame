// https://www.codingame.com/training/easy/ascii-art

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
        int l = int.Parse(Console.ReadLine());
        int h = int.Parse(Console.ReadLine());
        string t = Console.ReadLine();

        string validLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ?";

        var letterLookup = new Dictionary<string, string[]>();
        InitLetterLookup(h, validLetters, ref letterLookup);

        for (int i = 0; i < h; i++)
        {
            string row = Console.ReadLine();
            AddLetterLookup(l, i, row, validLetters, ref letterLookup);
        }

        for (int j = 0; j < h; j++)
        {
            foreach (var c in t)
            {
                var letter = c.ToString().ToUpperInvariant();
                if (letterLookup.ContainsKey(letter))
                {
                    Console.Write(letterLookup[letter][j]);
                }
                else
                {
                    Console.Write(letterLookup["?"][j]);
                }
            }
            Console.WriteLine();
        }
    }

    static void InitLetterLookup(int numRowsPerLetter, string validLetters, ref Dictionary<string, string[]> lookup)
    {
        foreach (var c in validLetters)
        {
            lookup.Add(c.ToString(), new string[numRowsPerLetter]);
        }
    }

    static void AddLetterLookup(int letterLength, int rowNum, string row, string validLetters, ref Dictionary<string, string[]> lookup)
    {
        for (int i = 0; i < validLetters.Length; i++)
        {
            lookup[validLetters[i].ToString()][rowNum] = row.Substring(i * letterLength, letterLength);
        }
    }
}