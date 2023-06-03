namespace AzMogaTukITam.Model
{
    public class BaseLayer : LayerBase
    {
        public BaseLayer(Grid grid) : base(grid)
        {
            for (int y = 0; y < this.Data.GetLength(0); y++)
            for (int x = 0; x < this.Data.GetLength(1); x++)
                this.Data[y, x] = true;

            NumbersData = CreateNumbersData(this.Data.GetLength(1), this.Data.GetLength(0));
            DisplayData = CreateDisplayData(NumbersData);
        }

        public override int ZIndex { get; protected set; } = 0;
        public override DisplayValue DisplayValue { get; protected set; } = new DisplayValue() {Value = ""};
        public override bool[,] Data { get; protected set; }
        public override int ConsolePriority { get; protected set; } = 0;
        public override int RequiredTurns { get; protected set; } = 0;
        public override Action<Game, ConsoleKeyInfo> ConsoleAction { get; protected set; }
        public override Action<Game> UpdateAction { get; protected set; }
        
        public DisplayValue[,] DisplayData { get; protected set; }

        public string[,] NumbersData { get; protected set; }

        private string[,] CreateNumbersData(int rows, int cols)
        {
            var random = new Random();
            var numbersData = new string[rows, cols];
            var operations = new List<string> {"+1 ", "-1 ", "x2 ", "x0 ", "/2 "};
            var operationTypes = new List<string> {"+", "-", "*", "/"};

            // Add one of each operation to the grid
            foreach (var operation in operations)
            {
                var row = random.Next(rows);
                var col = random.Next(cols / 2);
                numbersData[row, col] = operation;
            }

            numbersData[0, 0] = "0";
            numbersData[rows - 1, cols - 1] = "0";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < (cols % 2 == 0 ? cols / 2 : (cols / 2) + 1); j++)
                {
                    if (numbersData[i, j] == null)
                    {
                        int randomNumber = random.Next(0, 1 * Math.Max(rows, cols) + 1);
                        string randomOperation = operationTypes[random.Next(operationTypes.Count)];
                        string numberString = randomOperation;

                        numberString += Math.Abs(randomNumber).ToString();

                        numbersData[i, j] = numberString;
                    }
                }
            }

            numbersData = CopyAndRotate(numbersData);

            return numbersData;
        }

        private string[,] CopyAndRotate(string[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            string[,] resultArray = new string[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    resultArray[i, cols - j - 1] = array[rows - i - 1, j];
                }
            }
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (resultArray[i, j] == null)
                    {
                        resultArray[i, j] = array[i, j];
                    }
                }
            }

            return resultArray;
        }
        
        private DisplayValue[,] CreateDisplayData(string[,] numbersData)
        {
            var displayData = new DisplayValue[numbersData.GetLength(0), numbersData.GetLength(1)];
            
            for (int i = 0; i < numbersData.GetLength(0); i++)
            {
                for (int j = 0; j < numbersData.GetLength(1); j++)
                {
                    if (numbersData[i, j] != null)
                    {
                        displayData[i, j] = new DisplayValue() {Value = numbersData[i, j]};
                    }
                }
            }

            return displayData;
        }
    }
}