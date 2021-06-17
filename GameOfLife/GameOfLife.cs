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

        public void Update()
        {
            // game of life stuff here
        }

        /// <summary>
        /// Toggle whether or not the cell at the given indexes is alive.
        /// </summary>
        public void ToggleCellState(int x, int y, out bool newAlive)
        {
            try
            {
                newAlive = !cells[x, y].alive;
                cells[x, y].alive = newAlive;
            }
            catch (IndexOutOfRangeException)
            {
                newAlive = false;
                form.DebugLabel.Text = $"SetCellState() : index exception : {x},{y}";
            }
        }

        readonly GameForm form;
        readonly Cell[,] cells;

        public bool playing;
    }
}
