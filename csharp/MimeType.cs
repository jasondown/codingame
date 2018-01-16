// https://www.codingame.com/training/easy/mime-type

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
        int q = int.Parse(Console.ReadLine());
        var lookup = new Dictionary<string, string>();

        for (int i = 0; i < n; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            string ext = inputs[0];
            string mt = inputs[1];

            lookup.Add(ext.ToLowerInvariant(), mt);
        }
        for (int i = 0; i < q; i++)
        {
            string fName = Console.ReadLine(); // One file name per line.
            var extPos = fNAME.LastIndexOf(".");
            if (extPos >= 0)
            {
                var ext = fname.Substring(extPos + 1).ToLowerInvariant();
                if (lookup.ContainsKey(ext))
                {
                    Console.WriteLine(lookup[ext]);
                    continue;
                }
            }
            Console.WriteLine("UNKNOWN");
        }
    }
}