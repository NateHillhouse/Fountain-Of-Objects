
using System.Security.Cryptography.X509Certificates;

class Program
{
    static void Main()
    {
        UserInterface userInterface = new();
        int size = userInterface.WorldSize("How big would you like the world to be? (4x4, 6x6, 8x8) ");
        GameLoop game = new(size);
    }
}

class GameLoop
{
    bool loop = true;
    static Movement? movement;
    public GameLoop(int size)
    {
        movement = new Movement(size);
        while (loop)
        {
            movement.Move(ref loop, size);
        }
    }
}

class UserInterface
{
    public static readonly Dictionary<string, List<string>> movementOptions = new()
    {
        {"East", new List<string>{"move east","Move East", "Move east", "move East", "East", "east"}},
        {"West", new List<string>{"move west", "Move West", "Move west", "move West", "West", "west"}},
        {"North", new List<string>{"move north", "Move North", "Move north", "move North", "North", "north"}},
        {"South", new List<string>{"move south", "Move South", "Move south", "move South", "South", "south"}}
    };

        public void PrintMessages()
    {
        for (int length = 0; length < Console.WindowWidth; length ++)
        {
            Console.Write("-");
        }
        Console.WriteLine();
    }

    public string? ReadInput(string message, Dictionary<string, List<string>> dict)
    {
        Console.Write(message);
        string? input;
        bool match = false;
        //Get correct input
        input = Console.ReadLine();
        (input, match) = CheckInput(input, ReadInput, false, dict);

        return input;
    }

    public static (string?, bool) CheckInput(string? input, Func<string, Dictionary<string, List<string>>, string?> operation, bool match, Dictionary<string, List<string>> options)
    {
        string? key = input;
        //loop through all the possible options to make sure the input is valid
        foreach (KeyValuePair<string, List<string>> item in options)
        {
            foreach (string val in item.Value) if (val == input) 
            {
                return (item.Key, true);
            }
        }
        if (match == false || input == null) 
        {
            //Console.Write(errormessage)
            input = operation("Please enter a valid input. ", options);
            return (input, false);
        }
        else return (key, true);
    }

    public int WorldSize(string message)
    {
        Dictionary<string, List<string>> validInputs = new Dictionary<string, List<string>>
        {
            {"4x4", new List<string>{"4x4", "4"}},
            {"6x6", new List<string>{"6x6", "6"}},
            {"8x8", new List<string>{"8x8", "8"}}
        };
        string? input = ReadInput(message, validInputs);
        switch (input)
        {
            case "4x4":
                return 4;
            case "6x6":
                return 6;
            case "8x8":
                return 8;
            default: 
                return WorldSize("Please enter a valid input: (4, 6, 8) ");      
        }
    }
}

class Movement
{
    List<(int row, int column, List<string> contains)> grid = [];
    (int x, int y) location = (1, 1);
    UserInterface _interface = new UserInterface();

    public Movement(int size)
    {
        for (int i= 1; i<=size; i++)
        {
            for (int j = 1; j<=size; j++)
            {
                grid.Add((i, j, new List<string>()));
            }
        }
    }
    
    public void Move(ref bool loop, int size)
    {
        _interface.PrintMessages();
        Console.WriteLine($"You are in a room at {location.x}, {location.y}");
        string? movement = _interface.ReadInput("What do you want to do? (move east, move west, move north, move south) ", UserInterface.movementOptions);
        string bounds = "You hit the wall. ";

        switch (movement)
        {
            case "East":
                if (location.x == size) Console.WriteLine(bounds);
                else location.x += 1;
                break;
            case "West":
                if (location.x == 1) Console.WriteLine(bounds);
                else location.x -= 1;
                break;
            case "North":
                if (location.y == 1) Console.WriteLine(bounds);
                else location.y -= 1;
                break;
            case "South":
                if (location.y == size) Console.WriteLine(bounds);
                else location.y += 1;
                break; 
        }
        if (location == (4,4)) loop = false;
    }
}


