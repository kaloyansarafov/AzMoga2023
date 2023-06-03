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

        private bool CanPlaceQueen(int row, int col, Grid grid) => 
            grid.Layers.Where(x => x is IPlayerLayer)
                .Cast<IPlayerLayer>()
                .All(x => !x.IsPlaceOccupied(row, col));

        public bool IsPlaceOccupied(int row, int col)
        {
            return AttackedRows.Contains(row) ||
                        AttackedColumns.Contains(col) ||
                        AttackedLeftDiagonals.Contains(col - row) ||
                        AttackedRightDiagonals.Contains(col + row);
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

        public Coordinates OriginalPosition { get; private set; }
        private void HandleConsole(Game game, ConsoleKeyInfo ki)
        {
            var selectedLayer = (SelectedLayer)game.Grid.Layers.First(l => l is SelectedLayer);

        //find the coords of the player in Data

            if (_currentTurn == 0 && PlayerName == "Player 1")
            {
                game.DrawMessage($"{PlayerName} shall choose next!", 2000);
                selectedLayer.SetCurrentPointer(new Coordinates() { Y = 0, X = 0 });
                this.Data[0,0] = true;
                _currentTurn++;
                IPlayerLayer[] playerLayers = game.Grid.Layers.Where(x => x is IPlayerLayer).Cast<IPlayerLayer>().ToArray();
                return;
            }
            else if(_currentTurn == 0 && PlayerName == "Player 2")
            {
                this.Data[game.Grid.Height - 1, game.Grid.Width - 1] = true;
                selectedLayer.SetCurrentPointer(new Coordinates() { Y = game.Grid.Height -1, X = game.Grid.Width - 1});
                _currentTurn++;
                IPlayerLayer[] playerLayers = game.Grid.Layers.Where(x => x is IPlayerLayer).Cast<IPlayerLayer>().ToArray();

                return; 

            }



            Coordinates playerPosition = null;
            for (int i = 0; i < game.Grid.Height; i++)
                for (int o = 0; o < game.Grid.Width; o++)
                    if (this.Data[i, o])
                        playerPosition = new Coordinates() { Y = i, X = o };


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

                  

                    //MarkPositions(selectedLayer.CurrentPointer.Y, selectedLayer.CurrentPointer.X);

                    //clear the Data matrix 
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

                    this.OnTurnDone();

                    if (_currentTurn <= this.RequiredTurns + 1) {

                    }
                    else _currentTurn = 0;
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