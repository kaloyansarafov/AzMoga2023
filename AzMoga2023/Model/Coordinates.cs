using System.Threading.Tasks;

namespace AzMogaTukITam.Model
{
    public class Coordinates
    {
        public int Y { get; set; }
        public int X { get; set; }
        
        public Coordinates(){}
        public Coordinates(int y, int x)
        {
            Y = y;
            X = x;
        }
    }
}