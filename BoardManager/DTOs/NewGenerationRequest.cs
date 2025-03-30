using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager.DTOs
{
    public class NewGenerationRequest
    {
        public int Number { get; set; }
        public string BoardId { get; set; }
        public bool[][] State { get; set; }
        public bool IsFinalState { get; set; }
    }
}
