using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager
{
    public interface IBoardRulesEngine
    {
        bool[][] CalculateNextState(bool[][] currentState);
        public bool CalculateNextCellState(bool currentStateIsAlive, int neighboursAlive);
    }
}
