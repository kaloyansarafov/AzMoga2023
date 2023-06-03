namespace AzMogaTukITam.Model
{
    public class Grid
    {
        public Grid(int h, int w)
        {
            this.Height = h;
            this.Width = w;
            this.PreviousState = this.ConstructGrid();
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public DisplayValue[,] PreviousState { get; set; }

        public List<LayerBase> Layers { get; set; } = new List<LayerBase>();

        public DisplayValue[,] ConstructGrid()
        {
            DisplayValue[,] finalGrid = new DisplayValue[this.Height, this.Width];
            foreach (LayerBase layer in this.Layers.OrderBy(x => x.ZIndex).ThenBy(x => x.LayerID))
            {
                for (int y = 0; y < layer.Data.GetLength(0); y++)
                {
                    for (int x = 0; x < layer.Data.GetLength(1); x++)
                    {
                        if (layer.Data[y, x] && (layer is BaseLayer))
                        {
                            finalGrid[y, x] = ((BaseLayer)layer).DisplayData[y, x];
                        }
                        else if (layer.Data[y, x] && !(layer is BaseLayer))
                        {
                            var previousCell = finalGrid[y, x];
                            finalGrid[y, x] = new DisplayValue();
                            finalGrid[y, x].DisplayBackground = layer.DisplayValue.DisplayBackground;
                            finalGrid[y, x].DisplayForeground = layer.DisplayValue.DisplayForeground;
                            finalGrid[y, x].Value = previousCell.Value;
                        }       
                    }
                }
            }

            this.PreviousState = finalGrid;
            return finalGrid;
        }
    }
}