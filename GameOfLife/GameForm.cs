using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }


        #region formEvents

        private void GameForm_Load(object sender, EventArgs e)
        {
            //StartNewGame();

            ForeColor = Color.LightGray;
            BackColor = Color.FromArgb(140, 128, 100);

            if (debugEnabled)
                debugLabel.Text = "debugging enabled";
            else
                debugLabel.Hide();

            gameTimer.Interval = baseTimerInterval;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            if (game == null || game.playing) return;       // ignore clicks on the form base if the game is in action

            MouseEventArgs args = e as MouseEventArgs;

            // check that click was inside the game grid area
            if (MinX < args.X && args.X < MaxX
                && MinY < args.Y && args.Y < MaxY)
            {
                FindCellIndexes(args.X, args.Y, out int cellX, out int cellY);

                ToggleCellState(cellX, cellY);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // just a solution for drawing on startup, since drawing isn't possible from the form's load event
            if (firstTick)
            {
                StartNewGame();
                gameTimer.Enabled = false;
                firstTick = false;
            }

            if (game != null) game.Update();
        }
        bool firstTick = true;

        private void PlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!gameTimer.Enabled)
            {
                gameTimer.Start();
                playToolStripMenuItem.Text = "Pause";
            }
            else
            {
                gameTimer.Stop();
                playToolStripMenuItem.Text = "Play";
            }
        }

        private void SpeedControl_Scroll(object sender, EventArgs e)
        {
            // range :  0 to 10  ->  -5 to 5  ->  0.5 to -0.5
            double multiplier = (speedControl.Value - speedControl.Maximum / 2f) * -0.1;
            gameTimer.Interval = (int)(baseTimerInterval * (1 + multiplier));
        }

        #endregion formEvents


        #region drawing

        void StartNewGame()
        {
            DrawNewGrid();

            game = new GameOfLife(this, columnCount, rowCount);
            gridSizeLabel.Text = $"Grid size : {columnCount}x{rowCount}";
        }

        /// <summary>
        /// Draws a new game grid, taking into account the size of the form.
        /// </summary>
        void DrawNewGrid()
        {
            // initialise graphics
            Graphics graphics = CreateGraphics();
            graphics.Clear(BackColor);              // remove old drawings
            Pen pen = new Pen(ForeColor);

            CalculateDimensions(out columnCount, out rowCount);

            // draw vertical lines
            for (int x = 0; x < columnCount + 1; x++)       // +1 to account for the right border line
            {
                int posX = MinX + x * cellWidth;
                graphics.DrawLine(pen, posX, MinY, posX, MaxY);
            }
            // draw horizontal lines
            for (int y = 0; y < rowCount + 1; y++)          // +1  to account for the lower border line
            {
                int posY = MinY + y * cellHeight;
                graphics.DrawLine(pen, MinX, posY, MaxX, posY);
            }

            graphics.Dispose();
            pen.Dispose();
        }

        void ToggleCellState(int x, int y)
        {
            game.ToggleCellState(x, y, out bool alive);     // update game object's internal record

            ColorCell(x, y, alive);
        }

        public void RedrawGrid()
        {
            for (int x = 0; x < columnCount; x++)
            {
                for (int y = 0; y < rowCount; y++)
                {
                    ColorCell(x, y);
                }
            }
        }

        void ColorCell(int x, int y)
        {
            game.TryGetCellState(x, y, out bool alive);
            ColorCell(x, y, alive);
        }

        void ColorCell(int x, int y, bool alive)
        {
            Graphics graphics = CreateGraphics();
            
            Color fillColor = alive ? ForeColor : BackColor;

            Brush brush = new SolidBrush(fillColor);
            Point coords = GetCellTopLeft(x, y);
            graphics.FillRectangle(brush, coords.X + 1, coords.Y + 1, cellWidth - 1, cellHeight - 1);

            graphics.Dispose();
            brush.Dispose();
        }

        #endregion drawing


        #region utilities

        /// <summary>
        /// Calculates the cell-based dimensions of the game grid based on the form dimensions.
        /// </summary>
        /// <param name="colCount">The number of cell columns.</param>
        /// <param name="rowCount">The number of cell rows.</param>
        void CalculateDimensions(out int colCount, out int rowCount)
        {
            int gridWidth = Size.Width - horizontalPadding * 2;     // account for padding on left *and* right
            int gridHeight = Size.Height - verticalPadding * 2;     // "  top *and* bottom

            colCount = gridWidth / cellWidth;
            rowCount = gridHeight / cellHeight;
        }

        /// <summary>
        /// Determines the x and y indexes of the cell in the grid that was clicked.
        /// </summary>
        void FindCellIndexes(int rawX, int rawY, out int cellX, out int cellY)
        {
            int relX = rawX - horizontalPadding;
            int relY = rawY - verticalPadding;

            cellX = relX / cellWidth;
            cellY = relY / cellHeight;
        }

        /// <summary>
        /// Gets the Point of the top-left corner of the cell of the given indexes.
        /// </summary>
        Point GetCellTopLeft(int x, int y)
        {
            int px = MinX + x * cellWidth;
            int py = MinY + y * cellHeight;
            return new Point(px, py);
        }

        #endregion utilities


        GameOfLife game;
        const bool debugEnabled = true;
        public Label DebugLabel => debugLabel;

        const int baseTimerInterval = 500;

        // dimensions of a single cell in pixels
        const int cellWidth = 12;
        const int cellHeight = cellWidth;

        // edge padding of the game grid in pixels
        const int horizontalPadding = 40;
        const int verticalPadding = horizontalPadding;

        // dimensions of the game grid in cells
        int columnCount;
        int rowCount;

        // basic pixel positions of the game grid
        int MinX => horizontalPadding;
        int MaxX => MinX + columnCount * cellWidth;
        int MinY => verticalPadding;
        int MaxY => MinY + rowCount * cellHeight;

    }
}
