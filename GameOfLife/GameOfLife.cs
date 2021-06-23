using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    partial class GameOfLife
    {
        public GameOfLife(GameForm form, int cols, int rows)
        {
            this.form = form;
            playing = false;

            cells = new Cell[cols, rows];
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }

        }

        /// <summary>
        /// Executes one time step in the game of life, following the game's rules.<br/>
        /// This is called from <see cref="GameForm.GameTimer_Tick(object, EventArgs)"/>,
        /// so the speed of the game is defined by the <see cref="GameForm.gameTimer"/>'s tick interval.
        /// </summary>
        public void Update()
        {
            int cols = cells.GetLength(0);
            int rows = cells.GetLength(1);

            // copy the cell grid to a new array so as to not overwrite the contents of the current generation
            Cell[,] nextGen = new Cell[cols, rows];
            Array.Copy(cells, nextGen, cols * rows);

            // iterate through all cells
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    RunRulesOnCell(x, y, nextGen[x, y].alive, out bool aliveAfter);
                    nextGen[x, y].alive = aliveAfter;
                }
            }

            // replace current generation with the new generation of cells
            cells = nextGen;

            form.RedrawGrid();
        }

        void RunRulesOnCell(int x, int y, bool aliveBefore, out bool aliveAfter)
        {
            aliveAfter = false;     // default

            int livingNeighbourCount = CountLivingNeigbours(x, y);

            if (aliveBefore)
            {
                if (livingNeighbourCount < 2)
                {
                    aliveAfter = false;
                }
                else if (livingNeighbourCount == 2 || livingNeighbourCount == 3)
                {
                    aliveAfter = true;
                }
                else if (livingNeighbourCount > 3)
                {
                    aliveAfter = false;
                }
            }
            else
            {
                if (livingNeighbourCount == 3)
                {
                    aliveAfter = true;
                }
            }

            /*
            // "Any live cell with fewer than two live neighbours dies, as if by underpopulation."
            // "Any live cell with more than three live neighbours dies, as if by overpopulation."
            if (aliveBefore)
            {
                if (livingNeighbourCount < 2 || livingNeighbourCount > 3)
                    aliveAfter = false;
            }
            // "Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction."
            else
            {
                if (livingNeighbourCount == 3) aliveAfter = true;
            }*/
        }


        #region cellStatus

        /// <returns>The number of cells in the neighouring eight that are alive.</returns>
        int CountLivingNeigbours(int x, int y)
        {
            int livingCount = 0;

            // check the eight neighbours of the given cell's `alive` status
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y) continue;

                    TryGetCellState(i, j, out bool neighbourIsAlive);
                    if (neighbourIsAlive) livingCount++;
                }
            }

            return livingCount;
        }

        /// <summary>
        /// Toggle whether or not the cell at the given indexes is alive.
        /// </summary>
        public void ToggleCellState(int x, int y, out bool newAlive)
        {
            newAlive = !cells[x, y].alive;
            cells[x, y].alive = newAlive;
            
        }

        public bool TryGetCellState(int x, int y, out bool alive)
        {
            if (x < 0 || x >= cells.GetLength(0) || y < 0 || y >= cells.GetLength(1))
            {
                alive = false;
            }
            else
            {
                alive = cells[x, y].alive;
            }

            return true;
        }

        #endregion cellStatus

        readonly GameForm form;
        Cell[,] cells;

        public bool playing;

    }
}
