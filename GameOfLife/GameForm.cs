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

            DoubleBuffered = true;
        }

        const bool debugEnabled = true;


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

            SetSpeedControlPosition();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void GameForm_Click(object sender, EventArgs e)
        {
            if (game == null || gameTimer.Enabled) return;       // ignore clicks on the form base if the game is in action

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
            if (game != null && !firstTick) game.Update();

            // just a solution for drawing on startup, since drawing isn't possible from the form's load event
            if (firstTick)
            {
                StartNewGame();
                gameTimer.Stop();
                firstTick = false;
            }
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

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveData = Serialisation.Serialise(game.Generation, game.Cells, Size.Width, Size.Height);

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

                try
                {
                    Serialisation.Deserialise(raw, out int gen, out GameOfLife.Cell[,] cells, out int wWidth, out int wHeight);
                    Size = new Size(wWidth, wHeight);
                    StartNewGame(cells, gen);
                }
                catch (FormatException)
                {
                    Form dlg = new Form();
                    dlg.ShowDialog();
                }
            }

        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            SetSpeedControlPosition();
        }

        #endregion formEvents


        #region drawing

        void StartNewGame(GameOfLife.Cell[,] cells = null, int generation = 0)
        {
            gameTimer.Stop();
            DrawNewGrid();

            game = new GameOfLife(this, columnCount, rowCount, cells, generation);

            UpdateGameDataLabel();

            RedrawCells();
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

        /// <summary>
        /// Redraws the grid so that cells are up to date. Notable usage: <see cref="GameOfLife.Update"/>.
        /// </summary>
        /// <param name="coordsChanged">If this is not null, only coord arrays in the array will be redrawn.</param>
        public void RedrawCells(int[][] coordsChanged = null)
        {
            if (coordsChanged == null)
            {
                for (int x = 0; x < columnCount; x++)
                {
                    for (int y = 0; y < rowCount; y++)
                    {
                        ColorCell(x, y);
                    }
                }
            }
            else
            {
                // here we can redraw only the cells whose state has changed
                // this gives a visible performance increase - scan lines are far less visible
                for (int i = 0; i < coordsChanged.Length; i++)
                {
                    ColorCell(coordsChanged[i][0], coordsChanged[i][1]);
                }
            }

            UpdateGameDataLabel();
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

        void UpdateGameDataLabel()
        {
            gameDataLabel.Text = $"Grid size : {columnCount}x{rowCount}  \t" +
                $"Generation : {game.Generation}";
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

        void SetSpeedControlPosition()
        {
            int x = Size.Width - 25 - speedControl.Width;
            int y = speedControl.Height - 15;
            speedControl.Location = new Point(x, y);
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
