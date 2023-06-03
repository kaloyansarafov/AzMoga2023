namespace AzMogaTukITam.Model
{
    public abstract class LayerBase
    {
        public LayerBase(Grid grid)
        {
            this.Data = new bool[grid.Height, grid.Width];
        }

        public readonly string LayerID = Guid.NewGuid().ToString();

        public abstract int ZIndex { get; protected set; }

        public abstract DisplayValue DisplayValue { get; protected set; }

        public abstract bool[,] Data { get; protected set; }

        public abstract int ConsolePriority { get; protected set; }

        public abstract int RequiredTurns { get; protected set; }

        public event EventHandler TurnDone;

        protected virtual void OnTurnDone()
        {
            TurnDone?.Invoke(this, new EventArgs());
        }

        public abstract Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }

        public abstract Action<Game> UpdateAction { get; protected set; }
    }
}