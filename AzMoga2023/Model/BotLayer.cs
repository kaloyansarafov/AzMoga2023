namespace AzMogaTukITam.Model
{
    public sealed class BotLayer : LayerBase, IPlayerLayer
    {

        private BotLayer(Grid grid) : base(grid)
        {
            UpdateAction = HandleUpdate;
        }

        public BotLayer(Grid grid, DisplayValue dv, string pn) : this(grid)
        {
            DisplayValue = dv;
            PlayerName = pn;
            Data[grid.Height - 1, grid.Width - 1] = true;
        }

        public override int ZIndex { get; protected set; } = 150;
        public override DisplayValue DisplayValue { get; protected set; } = new DisplayValue() { Value = "X" };
        public override bool[,] Data { get; protected set; }
        public override int ConsolePriority { get; protected set; } = 0;
        public override int RequiredTurns { get; protected set; } = 0;
        public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
        public override Action<Game> UpdateAction { get; protected set; }

        public string PlayerName { get; private set; }
        
        public int Score = 0;
        public HashSet<Coordinates> BlockedCoordinates = new();


        private void HandleUpdate(Game game)
        {
            game.DrawMessage("Bot's thinking about it...", 1000);
            Coordinates playerPosition = null;
            for (int i = 0; i < game.Grid.Height; i++)
            for (int o = 0; o < game.Grid.Width; o++)
                if (this.Data[i, o])
                    playerPosition = new Coordinates() { Y = i, X = o };
            
            var botChoice = FindBestPlaceFromCoordinates(game.Grid, playerPosition);

            if (botChoice == null)
            {
                return;
            }
            for (int i = 0; i < game.Grid.Height; i++)
                for (int o = 0; o < game.Grid.Width; o++)
                    this.Data[i, o] = false;
            Data[botChoice.Y, botChoice.X] = true;
            BlockedCoordinates.Add(new Coordinates(botChoice.Y, botChoice.X));

            
            var blockLayer = (BlockLayer)game.Grid.Layers.First(l => l is BlockLayer);
            blockLayer.Block(botChoice);

          
            
            IPlayerLayer[] playerLayers = game.Grid.Layers
                .Where(x => x is IPlayerLayer)
                .Cast<IPlayerLayer>()
                .ToArray();
            
            var opponentCanMove = false;
            for (int i = 0; i < game.Grid.Height; i++)
            {
                for (int o = 0; o < game.Grid.Width; o++)
                {
                    if (playerLayers.All(pl => !pl.IsPlaceOccupied(i, o)))
                        opponentCanMove = true;
                }
            }
            if (!opponentCanMove)
                game.EndGame(() => game.DrawMessage($"{PlayerName} Won!", 5000));

        }

        public Coordinates? FindBestPlaceFromCoordinates(Grid grid, Coordinates place)
        {
            var baseLayer = grid.Layers.OfType<BaseLayer>().First();
            var blockLayer = grid.Layers.OfType<BlockLayer>().First();
            var numbersData = baseLayer.NumbersData;
            var x = place.X;
            var y = place.Y;
            Coordinates bestPlace = null;
            int bestScore = int.MinValue;

            for (int i = y - 1; i <= y + 1; i++)
            {
                if (i < 0 || i >= grid.Height) 
                {
                    continue;
                }

                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (j < 0 || j >= grid.Width || blockLayer.Data[i, j] || IsPlaceOccupied(i, j) || (i == y && j == x))
                    {
                        continue;
                    }

                    var operation = numbersData[i, j][0];
                    var operand = int.Parse(numbersData[i, j].Substring(1));

                    switch (operation)
                    {
                        case '*':
                            if (Score * operand > bestScore)
                            {
                                bestScore = Score * operand;
                                bestPlace = new Coordinates(i, j);
                            }
                            break;
                        case '/':
                            if (Score / operand > bestScore)
                            {
                                bestScore = Score / operand;
                                bestPlace = new Coordinates(i, j);
                            }
                            break;
                        case '+':
                            if (Score + operand > bestScore)
                            {
                                bestScore = Score + operand;
                                bestPlace = new Coordinates(i, j);
                            }
                            break;
                        case '-':
                            if (Score - operand > bestScore)
                            {
                                bestScore = Score - operand;
                                bestPlace = new Coordinates(i, j);
                            }
                            break;
                    }
                }
            }

            return bestPlace;
        }

        public bool IsPlaceOccupied(int row, int col)
        {
            return BlockedCoordinates.Contains(new Coordinates(row, col));
        }

        //returns the score of a place, where it blocks the most open spaces in the 4 directions and diagonals
        private int GetScore(int row, int column, Grid grid, HashSet<int> set)
        {
            int score = 0;
            for (int i = 0; i < grid.Height; i++)
            {
                if (set.Contains(i))
                    score++;
            }
            return score;
        }


        private bool CanPlaceQueen(int row, int col, Grid grid) => 
            grid.Layers.Where(x => x is IPlayerLayer)
                .Cast<IPlayerLayer>()
                .All(x => !x.IsPlaceOccupied(row, col));

    }
}