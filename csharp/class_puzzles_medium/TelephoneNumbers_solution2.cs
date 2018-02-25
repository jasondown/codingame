using System;
using System.Collections.Generic;

class Solution
{
    static void Main()
    {
        int totalNums = int.Parse(Console.ReadLine());
        var list = new HashSet<string>();
        for (var i = 0; i < totalNums; i++)
        {
            var telephone = Console.ReadLine();
            for (var j = telephone.Length; j > 0; j--)
            {
                list.Add(telephone.Substring(0, j));
            }
        }
        Console.WriteLine(list.Count);
    }
}