namespace GameOfLife
{
    partial class GameOfLife
    {
        struct Cell
        {
            public Cell(int x, int y)
            {
                alive = false;
                this.x = x;
                this.y = y;
            }

            public bool alive;
            public int x;
            public int y;

        }
    }
}
