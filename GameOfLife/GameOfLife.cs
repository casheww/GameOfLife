using System;
using System.Collections.Generic;

namespace GameOfLife
{
    partial class GameOfLife
    {
        public GameOfLife(GameForm form, int cols, int rows, Cell[,] cells, int generation = 0)
        {
            this.form = form;

            if (cells == null)
            {
                Cells = new Cell[cols, rows];

                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        Cells[x, y] = new Cell(x, y);
                    }
                }
            }
            else Cells = cells;

            Generation = generation;
        }

        /// <summary>
        /// Executes one time step in the game of life, following the game's rules.<br/>
        /// This is called from <see cref="GameForm.GameTimer_Tick(object, EventArgs)"/>,
        /// so the speed of the game is defined by the <see cref="GameForm.gameTimer"/>'s tick interval.
        /// </summary>
        public void Update()
        {
            int cols = Cells.GetLength(0);
            int rows = Cells.GetLength(1);

            // copy the cell grid to a new array so as to not overwrite the contents of the current generation
            Cell[,] nextGen = new Cell[cols, rows];
            Array.Copy(Cells, nextGen, cols * rows);

            // list of cell coords with changed states - allows for more efficient redrawing later
            List<int[]> coordsChanged = new List<int[]>();

            // iterate through all cells
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    RunRulesOnCell(x, y, nextGen[x, y].alive, out bool aliveAfter);

                    if (nextGen[x, y].alive != aliveAfter) coordsChanged.Add(new int[] { x, y });

                    nextGen[x, y].alive = aliveAfter;
                }
            }

            // replace current generation with the new generation of cells
            Cells = nextGen;

            Generation++;
            form.RedrawCells(coordsChanged.ToArray());
        }

        void RunRulesOnCell(int x, int y, bool aliveBefore, out bool aliveAfter)
        {
            aliveAfter = false;     // default

            int livingNeighbourCount = CountLivingNeigbours(x, y);

            if (aliveBefore)
            {
                // underpopulation
                if (livingNeighbourCount < 2)
                {
                    aliveAfter = false;
                }
                // stability
                else if (livingNeighbourCount == 2 || livingNeighbourCount == 3)
                {
                    aliveAfter = true;
                }
                // overpopulation
                else if (livingNeighbourCount > 3)
                {
                    aliveAfter = false;
                }
            }
            else
            {
                // reproduction
                if (livingNeighbourCount == 3)
                {
                    aliveAfter = true;
                }
            }
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
            newAlive = !Cells[x, y].alive;
            Cells[x, y].alive = newAlive;
            
        }

        public bool TryGetCellState(int x, int y, out bool alive)
        {
            if (x < 0 || x >= Cells.GetLength(0) || y < 0 || y >= Cells.GetLength(1))
            {
                alive = false;
            }
            else
            {
                alive = Cells[x, y].alive;
            }

            return true;
        }

        #endregion cellStatus

        readonly GameForm form;
        public Cell[,] Cells { get; private set; }

        public int Generation { get; private set; }

    }
}
