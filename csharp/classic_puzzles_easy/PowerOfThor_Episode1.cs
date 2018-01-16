// https://www.codingame.com/training/easy/power-of-thor-episode-1

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    [Flags]
    public enum MoveDirection
    {
        None = 0x0,
        North = 0x1,
        South = 0x2,
        East = 0x4,
        West = 0x8
    }

    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int LX = int.Parse(inputs[0]); // the X position of the light of power
        int LY = int.Parse(inputs[1]); // the Y position of the light of power
        int initialTX = int.Parse(inputs[2]); // Thor's starting X position
        int initialTY = int.Parse(inputs[3]); // Thor's starting Y position

        int TX = initialTX;
        int TY = initialTY;
        // game loop
        while (true)
        {
            MoveDirection moveDir = MoveDirection.None;
            int E = int.Parse(Console.ReadLine()); // The level of Thor's remaining energy, representing the number of moves he can still make.

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            var newTY = TY;
            if (LY < TY)
            {
                moveDir |= MoveDirection.North;
                newTY--;
            }
            else if (LY > TY)
            {
                moveDir |= MoveDirection.South;
                newTY++;
            }
            TY = newTY;

            var newTX = TX;
            if (LX > TX)
            {
                moveDir |= MoveDirection.East;
                newTX++;
            }
            else if (LX < TX)
            {
                moveDir |= MoveDirection.West;
                newTX--;
            }
            TX = newTX;

            Console.WriteLine(GetDirection(moveDir)); // A single line providing the move to be made: N NE E SE S SW W or NW
        }
    }

    static string GetDirection(MoveDirection moveDir)
    {
        switch (moveDir)
        {
            case MoveDirection.North:
                return "N";

            case MoveDirection.North | MoveDirection.East:
                return "NE";

            case MoveDirection.East:
                return "E";

            case MoveDirection.South | MoveDirection.East:
                return "SE";

            case MoveDirection.South:
                return "S";

            case MoveDirection.South | MoveDirection.West:
                return "SW";

            case MoveDirection.West:
                return "W";

            case MoveDirection.North | MoveDirection.West:
                return "NW";

            default:
                throw new ApplicationException("Unknown direction!");
                break;
        }
    }
}