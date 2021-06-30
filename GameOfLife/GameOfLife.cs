using System;

namespace GameOfLife
{
    partial class GameOfLife
    {
        public GameOfLife(GameForm form, int cols, int rows)
        {
            this.form = form;
            playing = false;

            // create new cell array where all cells are dead
            Cells = new Cell[cols, rows];
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Cells[x, y] = new Cell(x, y);
                }
            }

            StartingSoup = new Cell[0, 0];

            Generation = 0;
            rand = new Random();
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

            // iterate through all cells to run the game rules on them
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    RunRulesOnCell(x, y, nextGen[x, y].alive, out bool aliveAfter);
                    nextGen[x, y].alive = aliveAfter;
                }
            }

            // replace current generation with the new generation of cells
            Cells = nextGen;
            Generation++;

            form.RedrawCellStates();
            form.UpdateGameDataLabel();
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

        /// <summary>
        /// Creates a new 2D <see cref="Cell"/> array based on the given width and height
        /// and copies cell states from the old to the new (0,0 anchored).
        /// </summary>
        public void ResizeGrid(int width, int height)
        {
            Cell[,] newGrid = new Cell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y < Cells.GetLength(1) && x < Cells.GetLength(0))
                    {
                        newGrid[x, y] = Cells[x, y];
                    }
                    else
                    {
                        // new cells that default to being dead
                        newGrid[x, y] = new Cell(x, y);
                    }
                }
            }

            Cells = newGrid;
        }

        /// <summary>
        /// Generate a new soup, where each cell's alive/dead status is random.
        /// </summary>
        public void Soupify()
        {
            int cols = Cells.GetLength(0);
            int rows = Cells.GetLength(1);

            StartingSoup = new Cell[cols, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    bool alive = rand.Next(0, 2) == 1;       // randomises Cell.alive for each cell
                    Cells[x, y].alive = alive;
                    
                    // store the starting soup in memory so we can export it later if it makes a pretty pattern
                    StartingSoup[x, y] = new Cell(x, y, alive);
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
        readonly Random rand;

        public Cell[,] Cells { get; set; }
        public Cell[,] StartingSoup { get; private set; }
        public int Generation { get; private set; }

        public bool playing;

    }
}
