using System;

class Player
{
    static void Main()
    {
        var inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);

        // Max turns
        var ignored = int.Parse(Console.ReadLine());

        inputs = Console.ReadLine().Split(' ');
        var x = int.Parse(inputs[0]);
        var y = int.Parse(inputs[1]);

        var batman = new Batman(x, y, width, height);

        // game loop
        while (true)
        {
            var bombdir = Console.ReadLine();
            batman.Move(bombdir);
            Console.WriteLine(batman.Position);
        }
    }
}

class Batman
{
    private int XMin { get; set; }
    private int XMax { get; set; }
    private int YMin { get; set; }
    private int YMax { get; set; }
    private int X { get; set; }
    private int Y { get; set; }

    public string Position => $"{X} {Y}";

    public Batman(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        XMin = 0;
        XMax = width - 1;
        YMin = 0;
        YMax = height - 1;
    }

    public void Move(string bombDir)
    {
        switch (bombDir)
        {
            case "U":
                MoveUp();
                break;

            case "UR":
                MoveUp(); MoveRight();
                break;

            case "R":
                MoveRight();
                break;

            case "DR":
                MoveDown(); MoveRight();
                break;

            case "D":
                MoveDown();
                break;

            case "DL":
                MoveDown(); MoveLeft();
                break;

            case "L":
                MoveLeft();
                break;

            case "UL":
                MoveUp(); MoveLeft();
                break;
        }
    }

    private void MoveUp()
    {
        YMax = Y;
        Y -= (int)Math.Ceiling((double)(Y - YMin) / 2);
    }

    private void MoveRight()
    {
        XMin = X;
        X += (int)Math.Ceiling((double)(XMax - X) / 2);
    }

    private void MoveDown()
    {
        YMin = Y;
        Y += (int)Math.Ceiling((double)(YMax - Y) / 2);
    }

    private void MoveLeft()
    {
        XMax = X;
        X -= (int)Math.Ceiling((double)(X - XMin) / 2);
    }
}