
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
    public static Movement? movement;
    static readonly Obstacles Obstacles = new();
    static readonly UserInterface _interface = new();
    public GameLoop(int size)
    {
        movement = new Movement(size);
        Obstacles.RandomizeObstacles(size, ref movement.worldGrid);
        while (loop)
        {
            Obstacles.PrintGrid(movement.worldGrid, size, movement.location);
            _interface.PrintMessages();
            Obstacles.SenseObstacles(movement.worldGrid, movement.location, size);
            movement.Move(ref loop, size, _interface);
        }
    }
}

public class UserInterface
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

public class Obstacles
{
    public static Dictionary<(int, int), string> RandomizeObstacles(int size, ref Dictionary<(int x, int y), string> worldGrid)
    {
        Random rand = new();
        Dictionary<string, int> obstacles = new Dictionary<string, int>
        {
            {"Pit", 0},
            {"Maelstroms", 0},
            {"Amaroks", 0}
        };
        switch (size)
        {
            case 4:
                obstacles["Pit"] = 1;
                break;
            case 6:
                obstacles["Pit"] = 2;
                obstacles["Maelstrom"] = 1;
                obstacles["Amaroks"] = 2;
                break;
            case 8:
                obstacles["Pit"] = 4;
                obstacles["Maelstrom"] = 2;
                obstacles["Amaroks"] = 3;
                break;
        }
        
        List<(int x, int y)> usedSpaces = new List<(int x, int y)>();
        foreach (KeyValuePair<string, int> pair in obstacles)
        {
            while (obstacles[pair.Key] > 0)
            {
                (int x, int y) location = Randomize(usedSpaces);
                while (worldGrid[location] != "") 
                {
                    location = Randomize(usedSpaces);
                }
                worldGrid[location] = pair.Key;
                obstacles[pair.Key] --;
            } 
        }
        
        return worldGrid;


        (int, int) Randomize(List<(int x, int y)> spaces)
        {
            int x = rand.Next(1,size+1);
            int y = rand.Next(1,size+1);
            
            foreach ((int x, int y) item in spaces)
            {
                Console.Write($"{item.x}, {item.y}");
            }
            if (spaces.Contains((x, y))) return Randomize(spaces);
            return (x, y);
        }


    }
    public void PrintGrid(Dictionary<(int, int), string> worldGrid, int size, (int, int) location)
    {
        for (int i = 1; i <= size; i ++)
        {
            for (int j = 1; j <= size; j++)
            {
                if (worldGrid[(j,i)] != "") Console.Write(worldGrid[(j,i)]+ " ");
                else if ((j,i) == location) Console.Write("Player ");
                else Console.Write("Empty ");
            }
            Console.WriteLine();
        }
    }
    public static void SenseObstacles(Dictionary<(int, int), string> worldGrid, (int x, int y) player, int size)
    {
        (int x, int y) location = player;
        for (int x = -1; x < 2; x++)
        {
            location.x = player.x + x;
            if (location.x > 0 && location.x <= size) 
            {
                for (int y = -1; y < 2; y++)
                {
                    location.y = player.y + y;
                    if (location.y > 0 && location.y <= size) 
                    {
                        CheckForObstacles(location);
                    }
                }
            }
        }

        void loop((int x, int y) player)
        {
            
        }

        void CheckForObstacles((int, int) locationToCheck)
        {
            List<string> obstacles = [];
            if (worldGrid[locationToCheck] == "Pit") Console.WriteLine("You feel a draft. There is a pit in a nearby room.");
            if (worldGrid[locationToCheck] == "Maelstroms") Console.WriteLine("You hear the growling and groaning of a maelstrom nearby."); 
            if (worldGrid[locationToCheck] == "Amaroks") Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room."); 
        }
    }
}

public class Movement
{
    public (int x, int y) location = (1, 1);
    public Dictionary<(int x, int y), string> worldGrid = new();

    public Movement(int size)
    {
        //Create world upon class initialization
        for (int i= 1; i<=size; i++)
        {
            for (int j = 1; j<=size; j++)
            {
                worldGrid.Add((j, i), "");
                switch (j,i)
                {
                    case (1,1):
                        worldGrid[(j, i)] = "entrance";
                        break;
                }
            }
        }
    }


    
    public void Move(ref bool loop, int size, UserInterface _interface)
    {
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
        loop = HitObstacles();
        if (location == (size,size)) loop = false;
    }

    public bool HitObstacles()
    {
        string item = worldGrid[location];
            Console.WriteLine(item);
            switch (item)
            {
                case "Pit":
                    Console.WriteLine("You fell in a pit and died. ");
                    return false;
                case "Amaroks":
                    Console.WriteLine("You got eaten by an Amarok. ");
                    return false;
                case "Maelstrom":
                    return true;
                case "Entrance":
                    Console.WriteLine("You are at the entrance. ");
                    return true;
                default:
                    return true;
            }
    }

}


