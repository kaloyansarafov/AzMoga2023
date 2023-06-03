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
        }

        public override int ZIndex { get; protected set; } = 150;
        public override DisplayValue DisplayValue { get; protected set; } = new DisplayValue() { Value = "X" };
        public override bool[,] Data { get; protected set; }
        public override int ConsolePriority { get; protected set; } = 0;
        public override int RequiredTurns { get; protected set; } = 0;
        public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
        public override Action<Game> UpdateAction { get; protected set; }

        public string PlayerName { get; set; }
        public double Score { get; set; }

        public HashSet<int> AttackedRows = new();
        public HashSet<int> AttackedColumns = new();
        public HashSet<int> AttackedLeftDiagonals = new();
        public HashSet<int> AttackedRightDiagonals = new();

        // Override props

        private void HandleUpdate(Game game)
        {
            game.DrawMessage("Bot's thinking about it...", 1000);   
            var botChoice = FindBestPlace(game.Grid);

            if (botChoice == null)
            {
                return;
            }
            MarkPositions(botChoice.Y, botChoice.X);
            Data[botChoice.Y, botChoice.X] = true;
            
            var blockLayer = (BlockLayer)game.Grid.Layers.First(l => l is BlockLayer);

            foreach (var Coord in this.GetAttackedCoords(game.Grid))
            {
                blockLayer.Block(Coord);
            }
            
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
                game.EndGame(() => {
                    double highestScore = -1;
                    string winnerName = "";

                    foreach (var playerLayer in playerLayers)
                    {
                        if (playerLayer.Score > highestScore)
                        {
                            highestScore = playerLayer.Score;
                            winnerName = playerLayer.PlayerName;
                        }
                    }

                    game.DrawMessage($"{winnerName} Won with a score of {highestScore}!", 5000);
                });


        }

        //algorithm to find the best place to put a queen, where it blocks the most open spaces in the 4 directions and diagonals
        public Coordinates? FindBestPlace(Grid grid)
        {
            Coordinates bestPlace = null;
            int bestScore = -1;
            for (int i = 0; i < grid.Height; i++)
            {
                for (int j = 0; j < grid.Width; j++)
                {
                    if (CanPlaceQueen(i, j, grid))
                    {
                        int score = 0;
                        score += GetScore(i, j, grid, AttackedRows);
                        score += GetScore(i, j, grid, AttackedColumns);
                        score += GetScore(i, j, grid, AttackedLeftDiagonals);
                        score += GetScore(i, j, grid, AttackedRightDiagonals);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestPlace = new Coordinates(i, j);
                        }
                    }
                }
            }
            return bestPlace;
        }

        public bool IsPlaceOccupied(int row, int col)
        {
            return (AttackedRows.Contains(row) ||
                        AttackedColumns.Contains(col) ||
                        AttackedLeftDiagonals.Contains(col - row) ||
                        AttackedRightDiagonals.Contains(col + row));
        }

        private void MarkPositions(int row, int col)
        {
            AttackedRows.Add(row);
            AttackedColumns.Add(col);
            AttackedLeftDiagonals.Add(col - row);
            AttackedRightDiagonals.Add(col + row);
        }

        private IEnumerable<Coordinates> GetAttackedCoords(Grid grid)
        {
            for (int i = 0; i < grid.Height; i++)
                for (int o = 0; o < grid.Width; o++)
                    if (IsPlaceOccupied(i, o)) 
                        yield return new Coordinates(i, o);
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