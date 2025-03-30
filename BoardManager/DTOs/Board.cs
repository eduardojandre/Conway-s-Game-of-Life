using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager.DTOs
{
    public class Board
    {
        public String Id { get; set; }
        public string Name { get; set; }
        public bool[][] InitialState { get; set; }
    }
}
