using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager
{
    public class BoardRulesEngine : IBoardRulesEngine
    {
        public bool[][] CalculateNextState(bool[][] currentState)
        {
            var newBoardState = new bool[currentState.Length][];
            for (int i = 0; i < newBoardState.Length; i++)
            {
                newBoardState[i] = new bool[currentState[i].Length];
                for (int j = 0; j < currentState[i].Length; j++)
                {
                    var isAlive = IsAlive(currentState, i, j);
                    var neighboursAlive = CalculateLivingNeighbours(currentState, i, j);
                    newBoardState[i][j] = CalculateNextCellState(isAlive, neighboursAlive);
                }
            }
            return newBoardState;
        }
        public bool CalculateNextCellState(bool currentStateIsAlive, int neighboursAlive)
        {
            if (currentStateIsAlive && neighboursAlive < 2)
            {
                return false; //1. Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            }
            if (currentStateIsAlive && neighboursAlive > 3)
            {
                return false; //3. Any live cell with more than three live neighbours dies, as if by overpopulation.
            }
            if (currentStateIsAlive)
            {
                return true; //2. Any live cell with two or three live neighbours lives on to the next generation.
            }
            if (neighboursAlive == 3)
            {
                return true;//4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            }
            return false;
        }
        private int CalculateLivingNeighbours(bool[][] currentState, int row, int column)
        {

            int countAlive = 0;
            for (int i = row - 1; i < row + 2; i++)
            {
                for (int j = column - 1; j < column + 2; j++)
                {
                    if (!(row == i && column == j))
                    {
                        var currentCellIsAlive = IsAlive(currentState, i, j);
                        if (currentCellIsAlive)
                        {
                            countAlive++;
                        }
                    }
                }
            }
            return countAlive;
        }
        private bool IsAlive(bool[][] currentState, int row, int column)
        {
            if (row < 0)
                return false;
            if (column < 0)
                return false;
            if (row >= currentState.Length)
                return false;
            if (column >= currentState[row].Length)
                return false;
            return currentState[row][column];
        }
    }
}
