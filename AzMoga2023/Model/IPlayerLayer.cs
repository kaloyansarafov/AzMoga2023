using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzMogaTukITam.Model
{
    internal interface IPlayerLayer
    {
        public string PlayerName { get; set; }
        public double Score { get; set; }
        public bool IsPlaceOccupied(int y, int x);
    }
}
