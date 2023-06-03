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

        private void HandleConsole(Game game, ConsoleKeyInfo ki)
        {
            var selectedLayer = (SelectedLayer)game.Grid.Layers.First(l => l is SelectedLayer);
            
            if (_currentTurn == 0)
            {
                game.DrawMessage($"{PlayerName} shall choose next!", 2000);
                selectedLayer.SetCurrentPointer(new Coordinates() { Y = game.Grid.Height / 2, X = game.Grid.Width/2 });
                _currentTurn++;
            }
            
            switch (ki.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedLayer.MoveCurrentPointer(new Coordinates() { Y = -1 });
                    break;
                case ConsoleKey.DownArrow:
                    selectedLayer.MoveCurrentPointer(new Coordinates() { Y = 1 });
                    break;
                case ConsoleKey.RightArrow:
                    selectedLayer.MoveCurrentPointer(new Coordinates() { X = 1 });
                    break;
                case ConsoleKey.LeftArrow:
                    selectedLayer.MoveCurrentPointer(new Coordinates() { X = -1 });
                    break;
                case ConsoleKey.Enter:

                    if (!CanPlaceQueen(selectedLayer.CurrentPointer.Y, selectedLayer.CurrentPointer.X, game.Grid))
                    {
                        game.DrawMessage($"{PlayerName} shall not be placing here, it's either occupied by the opponent or the player himself!", 6000);
                        break;
                    }

                    MarkPositions(selectedLayer.CurrentPointer.Y, selectedLayer.CurrentPointer.X);
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
                    foreach (var coord in this.GetAttackedCoords(game.Grid))
                        blockLayer.Block(coord);

                    this.OnTurnDone();

                    if (_currentTurn <= this.RequiredTurns + 1)
                        selectedLayer.SetCurrentPointer(new Coordinates() { Y = game.Grid.Height / 2, X = game.Grid.Width / 2 });
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