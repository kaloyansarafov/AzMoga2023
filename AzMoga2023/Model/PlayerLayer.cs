namespace AzMogaTukITam.Model
{
    public sealed class PlayerLayer : LayerBase, IPlayerLayer
    { 

        int _currentTurn = 0;

        private PlayerLayer(Grid grid) : base(grid)
        {
            this.ConsoleAction = HandleConsole;
        }

        public PlayerLayer(Grid grid, DisplayValue dv, string pn) : this(grid)
        {
            DisplayValue = dv;
            PlayerName = pn;
        }

       
        public bool IsPlaceOccupied(int row, int col)
        {
            return BlockedCoordinates.Contains(new Coordinates(row, col));
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

        public override int ZIndex { get; protected set; } = 150;
        public override DisplayValue DisplayValue { get; protected set; } = new DisplayValue() {Value = "⚪" };
        public override bool[,] Data { get; protected set; }
        public override int ConsolePriority { get; protected set; } = 0;
        public override int RequiredTurns { get; protected set; } = 1;
        public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
        public override Action<Game> UpdateAction { get; protected set; }

        public string PlayerName { get; private set; }

        public HashSet<int> AttackedRows = new();
        public HashSet<int> AttackedColumns = new();
        public HashSet<int> AttackedLeftDiagonals = new();
        public HashSet<int> AttackedRightDiagonals = new();
        public HashSet<Coordinates> BlockedCoordinates = new();

        private void HandleConsole(Game game, ConsoleKeyInfo ki)
        {
            var selectedLayer = (SelectedLayer)game.Grid.Layers.First(l => l is SelectedLayer);

            //find the coords of the player in Data
            Coordinates playerPosition = null;
            for (int i = 0; i < game.Grid.Height; i++)
                for (int o = 0; o < game.Grid.Width; o++)
                    if (this.Data[i, o])
                        playerPosition = new Coordinates() { Y = i, X = o };



            if (_currentTurn == 0)
            {
                game.DrawMessage($"{PlayerName} shall choose next!", 2000);
                //find the true in Data
                selectedLayer.SetCurrentPointer(playerPosition);
                _currentTurn++;

            }

            
            switch (ki.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedLayer.CurrentPointer.Y - 1 < playerPosition.Y - 1)
                    {
                        break;
                    }
                    selectedLayer.MoveCurrentPointer(new Coordinates() { Y = -1 });
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedLayer.CurrentPointer.Y + 1 > playerPosition.Y + 1)
                    {
                        break;
                    }
                    selectedLayer.MoveCurrentPointer(new Coordinates() { Y = 1 });
                    break;
                case ConsoleKey.RightArrow:
                    if (selectedLayer.CurrentPointer.X + 1 > playerPosition.X + 1)
                    {
                        break;
                    }
                    selectedLayer.MoveCurrentPointer(new Coordinates() { X = 1 });
                    break;
                case ConsoleKey.LeftArrow:
                    if (selectedLayer.CurrentPointer.X - 1 < playerPosition.X - 1)
                    {
                        break;
                    }
                    selectedLayer.MoveCurrentPointer(new Coordinates() { X = -1 });
                    break;
                case ConsoleKey.Enter:


                    if (IsPlaceOccupied(selectedLayer.CurrentPointer.Y, selectedLayer.CurrentPointer.X))
                    {
                        game.DrawMessage($"{PlayerName} shall not be placing here, it's either occupied by the opponent or the player himself!", 6000);
                        break;
                    }


                    for (int i = 0; i < game.Grid.Height; i++)
                        for (int o = 0; o < game.Grid.Width; o++)
                            this.Data[i, o] = false;
                    


                    this.Data[selectedLayer.CurrentPointer.Y, selectedLayer.CurrentPointer.X] = true;

                    IPlayerLayer[] playerLayers = game.Grid.Layers.Where(x => x is IPlayerLayer).Cast<IPlayerLayer>().ToArray();
                    bool opponentCanMove = false;
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

                    var blockLayer = (BlockLayer)game.Grid.Layers.First(l => l is BlockLayer);
                    //foreach (var coord in this.GetAttackedCoords(game.Grid))
                    //    blockLayer.Block(coord);
                    blockLayer.Block(selectedLayer.CurrentPointer);
                    blockLayer.Block(new Coordinates(0, 0));
                    blockLayer.Block(new Coordinates(game.Grid.Width - 1, game.Grid.Height - 1));
                    BlockedCoordinates.Add(selectedLayer.CurrentPointer);
                    this.OnTurnDone();

                    _currentTurn = 0;
                    break;
            }
        }

        private static bool OpponentCanMove(Game game, PlayerLayer[] playerLayers)
        {
            for (int i = 0; i < game.Grid.Height; i++)
            {
                for (int o = 0; o < game.Grid.Width; o++)
                {
                    if (playerLayers.All(pl => !pl.IsPlaceOccupied(i, o)))
                        return true;
                }
            }
            return false;
        }
    }
}