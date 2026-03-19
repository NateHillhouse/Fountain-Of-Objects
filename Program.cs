
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        GameLoop game = new();
    }
}

class GameLoop
{
    bool loop = true;
    static Movement movement = new();
    public GameLoop()
    {
        
        while (loop)
        {
            movement.Move(ref loop);
        }
    }
}

class UserInterface
{
    public static readonly Dictionary<string, List<string>> movementOptions = new()
    {
        {"East", new List<string>{"move east","Move East", "Move east", "move East"}},
        {"West", new List<string>{"move west", "Move West", "Move west", "move West"}},
        {"North", new List<string>{"move north", "Move North", "Move north", "move North"}},
        {"South", new List<string>{"move south", "Move South", "Move south", "move South"}}
    };

        public void PrintMessages()
    {
        for (int length = 0; length < Console.WindowWidth; length ++)
        {
            Console.Write("-");
        }
        Console.WriteLine();
    }

    public string? ReadInput(string message)
    {
        Console.Write(message);
        string? input;
        bool match = false;
        //Get correct input
        input = Console.ReadLine();
        (input, match) = CheckInput(input, ReadInput, false);

        return input;
    }

    public static (string?, bool) CheckInput(string? input, Func<string, string?> operation, bool match)
    {
        //loop through all the possible options to make sure the input is valid
        foreach (KeyValuePair<string, List<string>> item in movementOptions)
        {
            foreach (string val in item.Value) if (val == input) match = true;
        }
        if (match == false || input == null) 
        {
            //Console.Write(errormessage)
            input = operation("Please enter a valid input. ");
            return (input, false);
        }
        else return (input, true);
    }
}

class Movement
{
    List<(int row, int column, List<string> contains)> grid = [];
    (int x, int y) location = (1, 1);
    UserInterface _interface = new UserInterface();

    public Movement()
    {
        for (int i= 1; i<=4; i++)
        {
            for (int j = 1; j<=4; j++)
            {
                grid.Add((i, j, new List<string>()));
            }
        }
    }
    
    public void Move(ref bool loop)
    {
        _interface.PrintMessages();
        Console.WriteLine($"You are in a room at {location.x}, {location.y}");
        string? movement = _interface.ReadInput("What do you want to do? (move east, move west, move north, move south) ");
        string bounds = "You hit the wall. ";
        switch (movement)
        {
            case "move east":
            case "Move East":
            case "Move east":
            case "move East":
                if (location.x == 4) Console.WriteLine(bounds);
                else location.x += 1;
                break;
            case "move west":
            case "Move West":
            case "Move west":
            case "move West":
                if (location.x == 1) Console.WriteLine(bounds);
                else location.x -= 1;
                break;
            case "move north":
            case "Move North":
            case "Move north":
            case "move North":
                if (location.y == 1) Console.WriteLine(bounds);
                else location.y -= 1;
                break;
            case "move south":
            case "Move South":
            case "Move south":
            case "move South":
                if (location.y == 4) Console.WriteLine(bounds);
                else location.y += 1;
                break; 
        }
        if (location == (4,4)) loop = false;
    }
}


