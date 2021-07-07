using System.Text.RegularExpressions;

namespace GameOfLife
{
    static class Serialisation
    {
        public static string Serialise(GameOfLife.Cell[,] cells, int wWidth, int wHeight)
        {
            // file starts with some metadata
            string data = $"{serialisationFormatVersion}\n" +
                $"window w{wWidth} h{wHeight}\n" +
                $"grid w{cells.GetLength(0)} h{cells.GetLength(1)}\n";

            // ... and then the grid representation
            // I could use run length encoding here, but I think it's nice to be able to view 
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    data += cells[x, y].alive ? "X" : ".";
                }
                data += "\n";
            }
            
            return data;
        }

        public static void Deserialise(string raw, out GameOfLife.Cell[,] cells, out int wWidth, out int wHeight)
        {
            string[] lines = Regex.Split(raw, @"\n");

            // check for window size metadata
            Match windowMatch = Regex.Match(lines[1], @"^window w([0-9]+) h([0-9]+)$");
            wWidth = int.Parse(windowMatch.Groups[1].Value);
            wHeight = int.Parse(windowMatch.Groups[2].Value);

            // check for grid size metadata
            Match gridMatch = Regex.Match(lines[2], @"^grid w([0-9]+) h([0-9]+)$");
            int w = int.Parse(gridMatch.Groups[1].Value);
            int h = int.Parse(gridMatch.Groups[2].Value);

            // line index for where cell data starts after the aforementioned 'metadata'
            int metadataOffset = 3;

            cells = new GameOfLife.Cell[w, h];

            for (int y = 0; y < h; y++)
            {
                if (Regex.IsMatch(lines[y + metadataOffset], $@"^[X.]{{{w}}}$"))
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (lines[y + metadataOffset][x] == 'X')
                        {
                            cells[x, y] = new GameOfLife.Cell(x, y, true);
                        }
                        else
                        {
                            cells[x, y] = new GameOfLife.Cell(x, y, false);
                        }
                    }
                }
            }

        }


        const string serialisationFormatVersion = "0";
        public const string savePath = "saves";

    }
}
