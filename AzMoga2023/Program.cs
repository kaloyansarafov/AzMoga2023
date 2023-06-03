using AzMogaTukITam.Model;

PrintLogo();
Console.WriteLine("Welcome to MathTricks, please enter a number corresponding to one of the available options below");
Console.WriteLine("1. Single Player");
Console.WriteLine("2. Multi Player");
Console.WriteLine("3. Exit");
var selection = Console.ReadLine();
int[] gridSize;

switch (selection)
{
    case "1":
        gridSize = ReadGridSize();
        Game mainGame = new Game(gridSize[0], gridSize[1], new GameContext());
        mainGame.Grid.Layers.Add(new BaseLayer(mainGame.Grid));
        mainGame.Grid.Layers.Add(new BlockLayer(mainGame.Grid));
        mainGame.Grid.Layers.Add(new PlayerLayer(mainGame.Grid, new DisplayValue() { Value = "X", DisplayBackground = ConsoleColor.DarkBlue, DisplayForeground = ConsoleColor.White }, "Player 1"));
        mainGame.Grid.Layers.Add(new BotLayer(mainGame.Grid, new DisplayValue() { Value = "X", DisplayBackground = ConsoleColor.DarkRed, DisplayForeground = ConsoleColor.White }, "Bot 1"));
        mainGame.Grid.Layers.Add(new SelectedLayer(mainGame.Grid));
        mainGame.Start();
        break;
    case "2":
        gridSize = ReadGridSize();
        Game playerGame = new Game(gridSize[0], gridSize[1], new GameContext());
        playerGame.Grid.Layers.Add(new BaseLayer(playerGame.Grid));
        playerGame.Grid.Layers.Add(new BlockLayer(playerGame.Grid));
        playerGame.Grid.Layers.Add(new PlayerLayer(playerGame.Grid, new DisplayValue() { Value= "X", DisplayBackground = ConsoleColor.DarkBlue, DisplayForeground = ConsoleColor.White }, "Player 1"));
        playerGame.Grid.Layers.Add(new PlayerLayer(playerGame.Grid, new DisplayValue() { Value = "X", DisplayBackground = ConsoleColor.DarkRed, DisplayForeground = ConsoleColor.White }, "Player 2"));
        playerGame.Grid.Layers.Add(new SelectedLayer(playerGame.Grid));
        playerGame.Start();
        break;
    case "3":
        break;
    default:
        Console.WriteLine("Invalid selection");
        break;
}


static int[] ReadGridSize()
{
    while (true)
    {
        Console.Clear();
        PrintLogo();
        try
        {
            Console.WriteLine("Please enter the grid dimensions in the format: '[width],[height]'");
            var sizes = Console.ReadLine().Split(",").Select(int.Parse).ToArray();
            if (sizes.Length != 2 || sizes.Any(x => x > 90 || x < 4)) throw new Exception();
            return sizes;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Grid must be at least 4x4 and up to 20x20!");
            Thread.Sleep(3000);
        }
    }
}

static void PrintLogo()
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(@"
  __    __     ______     ______   __  __     ______   ______     __     ______     __  __     ______    
/\ ""-./  \   /\  __ \   /\__  _\ /\ \_\ \   /\__  _\ /\  == \   /\ \   /\  ___\   /\ \/ /    /\  ___\   
\ \ \-./\ \  \ \  __ \  \/_/\ \/ \ \  __ \  \/_/\ \/ \ \  __<   \ \ \  \ \ \____  \ \  _""-.  \ \___  \  
 \ \_\ \ \_\  \ \_\ \_\    \ \_\  \ \_\ \_\    \ \_\  \ \_\ \_\  \ \_\  \ \_____\  \ \_\ \_\  \/\_____\ 
  \/_/  \/_/   \/_/\/_/     \/_/   \/_/\/_/     \/_/   \/_/ /_/   \/_/   \/_____/   \/_/\/_/   \/_____/ 
                                                                                                        ");
    Console.ForegroundColor = ConsoleColor.White;
}