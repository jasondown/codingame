// https://www.codingame.com/training/easy/chuck-norris

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
        string message = Console.ReadLine();

        var bytes = Encoding.ASCII.GetBytes(message);
        var bString = String.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(7, '0')));

        var output = new StringBuilder();

        string lastNumber = String.Empty;
        foreach (var c in bString)
        {
            var currentNumber = c.ToString();
            if (lastNumber != currentNumber)
            {
                output.Append(" ");
                output.Append(currentNumber == "1" ? "0 " : "00 ");
                lastNumber = currentNumber;
            }
            output.Append("0");
        }

        Console.WriteLine(output.ToString().TrimStart());
    }
}