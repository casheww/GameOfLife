namespace GameOfLife
{
    partial class GameOfLife
    {
        public struct Cell
        {
            public Cell(int x, int y, bool alive = false)
            {
                this.x = x;
                this.y = y;
                this.alive = alive;
            }

            public int x;
            public int y;
            public bool alive;

        }
    }
}
