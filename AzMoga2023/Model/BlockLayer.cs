using System;

namespace AzMogaTukITam.Model;

public class BlockLayer : LayerBase
{
    public BlockLayer(Grid grid) : base(grid)
    {
    }

    public override int ZIndex { get; protected set; } = 100;
    public override DisplayValue DisplayValue { get; protected set; } = new() {Value = ' ', DisplayBackground = ConsoleColor.DarkGray};
    public override bool[,] Data { get; protected set; }
    public override int ConsolePriority { get; protected set; } = 0;
    public override int RequiredTurns { get; protected set; } = 0;
    public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
    public override Action<Game> UpdateAction { get; protected set; }

    // Override props

    public void Block(Coordinates cords) => Data[cords.Y, cords.X] = true;
}