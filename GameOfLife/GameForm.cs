using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();

            ForeColor = Color.LightGray;
            BackColor = Color.FromArgb(140, 128, 100);
        }

        const bool debugEnabled = true;


        #region formEvents

        private void GameForm_Load(object sender, EventArgs e)
        {
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
            if (game == null) return;

            MouseEventArgs args = e as MouseEventArgs;

            // check that click was inside the game grid area
            if (MinX < args.X && args.X < MaxX
                && MinY < args.Y && args.Y < MaxY)
            {
                FindCellIndexes(args.X, args.Y, out int cellX, out int cellY);

                ToggleCellState(cellX, cellY);
            }
        }

        private void GameForm_ResizeEnd(object sender, EventArgs e)
        {
            SetSpeedControlPosition();
            ResizeGameGrid();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // just a solution for drawing on startup, since drawing isn't possible from the form's load event
            if (firstTick)
            {
                StartNewGame();
                SetSpeedControlPosition();
                gameTimer.Enabled = false;
                gameTimer.Interval = baseTimerInterval;
                firstTick = false;
            }
            else if (game != null) game.Update();
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
            // range :  0 to 10  ->  -5 to 5  ->  0.95 to -0.95
            double multiplier = (speedControl.Value - speedControl.Maximum / 2f) * -0.19;
            multiplier += 1;        // range: 0.05 to 1.95
            gameTimer.Interval = (int)(baseTimerInterval * multiplier);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveData = Serialisation.Serialise(game.Cells, Size.Width, Size.Height);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save your game state",
                Filter = "Text file|*.txt",
                InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), Serialisation.savePath),
                FileName = "life-save",
                AddExtension = true
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                Stream fs = saveFileDialog.OpenFile();
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(saveData);
                }
                saveFileDialog.Dispose();
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Load a game state",
                Filter = "Text file|*.txt",
                InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), Serialisation.savePath)
            };
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                string raw;

                Stream fs = openFileDialog.OpenFile();
                using (StreamReader sr = new StreamReader(fs))
                {
                    raw = sr.ReadToEnd();
                }
                openFileDialog.Dispose();

                Serialisation.Deserialise(raw, out GameOfLife.Cell[,] cells, out int wWidth, out int wHeight);

                StartNewGame();
                game.Cells = cells;
                Size = new Size(wWidth, wHeight);

                RedrawCellStates();
            }
            
        }

        private void NewSoup_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
            game.Soupify();
            RedrawCellStates();
        }

        private void ExportSoupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get save data without metadata
            string saveData = Serialisation.Serialise(game.StartingSoup, Size.Width, Size.Height);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Export your starting Soup",
                Filter = "Text file|*.txt",
                InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), Serialisation.savePath),
                FileName = "life-soup",
                AddExtension = true
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                Stream fs = saveFileDialog.OpenFile();
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(saveData);
                }
                saveFileDialog.Dispose();
            }
        }

        #endregion formEvents


        #region drawing

        void StartNewGame()
        {
            DrawNewGrid();

            game = new GameOfLife(this, columnCount, rowCount);
            UpdateGameDataLabel();
        }

        /// <summary>
        /// Handles dynamic resizing of the game grid to the game window.
        /// </summary>
        void ResizeGameGrid()
        {
            DrawNewGrid();

            game.ResizeGrid(columnCount, rowCount);
            UpdateGameDataLabel();

            RedrawCellStates();
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

        public void RedrawCellStates()
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

        void SetSpeedControlPosition()
        {
            speedControl.Location = new Point(Size.Width - speedControl.Width - 20, 30);
        }

        public void UpdateGameDataLabel()
        {
            gridSizeLabel.Text = $"Grid size : {columnCount}x{rowCount}\t  Generation : {game.Generation}";
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
