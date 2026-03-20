
using System.Drawing;
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

public class GameLoop
{
    public static bool fountainActive = false;
    static bool loop = true;
    public static Movement? movement;
    static readonly Obstacles Obstacles = new();
    public static readonly UserInterface _interface = new();
    public static readonly UserInterface.ChangeUserOptions changeUserOptions = new();
    public static int size;
    public GameLoop(int _size)
    {
        size = _size;
        movement = new Movement(size);
        changeUserOptions.AddShooting(size);
        Obstacles.RandomizeObstacles(size, ref movement.worldGrid);
        int arrowsLeft = movement.arrows;
        while (loop)
        {
            //Obstacles.PrintGrid(movement.worldGrid, size, movement.location);
            _interface.PrintMessages();
            Obstacles.SenseObstacles(movement.worldGrid, movement.location, size);
            if (arrowsLeft > movement.arrows) 
            {
                arrowsLeft = movement.arrows;
                Console.WriteLine($"You have {arrowsLeft} arrows left.");
            }
            movement.Move(this, ref loop, size, fountainActive, _interface, changeUserOptions);
        }
    }
}

public class UserInterface
{
    public static Dictionary<string, List<string>> movementOptions = new()
    {
        {"East", new List<string>{"move east","Move East", "Move east", "move East", "East", "east"}},
        {"West", new List<string>{"move west", "Move West", "Move west", "move West", "West", "west"}},
        {"North", new List<string>{"move north", "Move North", "Move north", "move North", "North", "north"}},
        {"South", new List<string>{"move south", "Move South", "Move south", "move South", "South", "south"}},
    };

    public class ChangeUserOptions
    {
        public void AddShooting(int size)
        {
            if (size != 4)
            {
                Dictionary<string, List<string>> shootingOptions = new()
                {
                    {"Shoot South", new List<string>{"shoot south", "Shoot South", "Shoot south", "shoot South"}},
                    {"Shoot North", new List<string>{"shoot north", "Shoot North", "Shoot north", "shoot North"}},
                    {"Shoot East", new List<string>{"shoot east", "Shoot East", "Shoot east", "shoot East"}},
                    {"Shoot West", new List<string>{"shoot west", "Shoot West", "Shoot west", "shoot West"}}
                };
                foreach(KeyValuePair<string, List<string>> items in shootingOptions) movementOptions.Add(items.Key, items.Value);
            }
        }
        
        public void AddFountain()
        {
            movementOptions.Add("Activate Fountain", new List<string> {"Activate Fountain", "Activate fountain", "activate Fountain", "activate fountain"});
        }
    }
    public void PrintMessages()
    {
        for (int length = 0; length < Console.WindowWidth; length ++)
        {
            Console.Write("-");
        }
        Console.WriteLine($"You are in the room at {GameLoop.movement.location.x}, {GameLoop.movement.location.y}");
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
            {"Amaroks", 0},
            {"Fountain", 1}
        };
        switch (size)
        {
            case 4:
                obstacles["Fountain"] = 1;
                obstacles["Pit"] = 1;
                break;
            case 6:
                obstacles["Fountain"] = 1;
                obstacles["Pit"] = 2;
                obstacles["Maelstrom"] = 1;
                obstacles["Amaroks"] = 2;
                break;
            case 8:
                obstacles["Fountain"] = 1;
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
        if (worldGrid[player] == "Fountain" && !GameLoop.fountainActive) Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!"); 

        void CheckForObstacles((int, int) locationToCheck)
        {
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
    public int arrows = 5;

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
    
    public void Move(GameLoop game, ref bool loop, int size, bool fountainActive, UserInterface _interface, UserInterface.ChangeUserOptions changeUserOptions)
    {
        string? movement;
        string message = $"You are in a room at {location.x}, {location.y}\rWhat do you want to do? ";
        if (size == 4) message += "(move: east, west, north, ";
        else  message += "(move or shoot: east, west, north, ";

        if (!fountainActive && UserInterface.movementOptions.ContainsKey("Activate Fountain")) message += "south, or activate fountain) ";
        else message += "or south) ";

        movement = _interface.ReadInput(message, UserInterface.movementOptions);
        
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
            case "Shoot East":
                Shoot((location.x + 1, location.y));
                break; 
            case "Shoot West":
                Shoot((location.x - 1, location.y));
                break; 
            case "Shoot North":
                Shoot((location.x, location.y - 1));
                break; 
            case "Shoot South":
                Shoot((location.x, location.y + 1));
                break;
            case "Activate Fountain":
                GameLoop.fountainActive = true;
                UserInterface.movementOptions.Remove("Activate Fountain");
                break;
        }
        loop = HitObstacles(UserInterface.movementOptions, changeUserOptions);
        //if (location == (size,size)) loop = false;
    }

    public void Shoot((int, int) gridSquare)
    {
        if (arrows == 0) Console.WriteLine("You cannot shoot, you are out of arrows. "); 
        else if (worldGrid[gridSquare] == "Amaroks")
        {
            worldGrid[gridSquare] = "";
        }
        arrows --;
    }

    public bool HitObstacles(Dictionary<string, List<string>> movementOptions, UserInterface.ChangeUserOptions changeUserOptions)
    {
        string item = worldGrid[location];
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
                case "Fountain":
                    if (!GameLoop.fountainActive) changeUserOptions.AddFountain();
                    else 
                    {
                        movementOptions.Remove("Activate Fountain");
                        Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
                    }
                    return true;
                case "entrance":
                    if (GameLoop.fountainActive) 
                    {
                        Console.WriteLine("You have succesfully activated the fountain and exited the cave! ");
                        return false;
                    }
                    else Console.WriteLine("You see light in this room coming from outside the cavern. This is the entrance. ");
                    return true;
                default:
                return true;
            }
    }

}


