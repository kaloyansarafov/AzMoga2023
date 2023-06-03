namespace AzMogaTukITam.Model;

public class SelectedLayer : LayerBase
{
    private Coordinates _currentPointer = new Coordinates();

    public SelectedLayer(Grid grid)
        : base(grid)
    {
    }

    public override int ZIndex { get; protected set; } = 200;

    public override DisplayValue DisplayValue { get; protected set; } = new DisplayValue()
        {Value = "+", DisplayBackground = ConsoleColor.DarkCyan, DisplayForeground = ConsoleColor.White};

    public override bool[,] Data { get; protected set; }
    public override int ConsolePriority { get; protected set; }
    public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
    public override Action<Game> UpdateAction { get; protected set; }

    public Coordinates CurrentPointer => _currentPointer;

    public override int RequiredTurns { get; protected set; } = 0;

    public Coordinates SetCurrentPointer(Coordinates cord)
    {
        if (cord.X < 0 || cord.X > Data.GetLength(1) || cord.Y < 0 || cord.Y > Data.GetLength(0))
            return _currentPointer;
        ClearCurrentPointer();
        _currentPointer = cord;
        Data[_currentPointer.Y, _currentPointer.X] = true;
        return _currentPointer;
    }

    public Coordinates MoveCurrentPointer(Coordinates rel)
    {
        int newX = _currentPointer.X + rel.X;
        int newY = _currentPointer.Y + rel.Y;

        // Calculate the absolute difference in X and Y coordinates
        int deltaX = Math.Abs(newX - _currentPointer.X);
        int deltaY = Math.Abs(newY - _currentPointer.Y);

        // Check if the movement is within the allowed range and original boundaries
        if ((deltaX == 0 || deltaX == 1) && (deltaY == 0 || deltaY == 1) &&
            deltaX <= 1 && deltaY <= 1 &&
            newX >= 0 && newX < Data.GetLength(1) && newY >= 0 && newY < Data.GetLength(0))
        {
            ClearCurrentPointer();
            _currentPointer.X = newX;
            _currentPointer.Y = newY;
            Data[_currentPointer.Y, _currentPointer.X] = true;
        }

        return _currentPointer;
    }


    public void ClearCurrentPointer()
    {
        for (int y = 0; y < Data.GetLength(0); y++)
        for (int x = 0; x < Data.GetLength(1); x++)
            Data[y, x] = false;
    }
}