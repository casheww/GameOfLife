using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GameOfLife
{
    static class Serialisation
    {
        public static string Serialise(int generation, GameOfLife.Cell[,] cells, int wWidth, int wHeight)
        {
            string data = $"{serialisationFormatVersion}\n" +
                $"generation {generation}\n" +
                $"window w{wWidth} h{wHeight}\n" +
                $"grid w{cells.GetLength(0)} h{cells.GetLength(1)}\n";

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

        public static void Deserialise(string raw, out int gen, out GameOfLife.Cell[,] cells, out int wWidth, out int wHeight)
        {
            string[] lines = Regex.Split(raw, @"\n");

            Match genMatch = Regex.Match(lines[1], @"generation ([0-9]+)");
            gen = int.Parse(genMatch.Groups[1].Value);

            Match windowMatch = Regex.Match(lines[2], @"^window w([0-9]+) h([0-9]+)$");
            wWidth = int.Parse(windowMatch.Groups[1].Value);
            wHeight = int.Parse(windowMatch.Groups[2].Value);

            Match gridMatch = Regex.Match(lines[3], @"^grid w([0-9]+) h([0-9]+)$");
            int w = int.Parse(gridMatch.Groups[1].Value);
            int h = int.Parse(gridMatch.Groups[2].Value);

            cells = new GameOfLife.Cell[w, h];

            int gridYOffset = 4;

            for (int y = gridYOffset; y < h + gridYOffset; y++)
            {
                if (Regex.IsMatch(lines[y], $@"^[X.]{{{w}}}$"))
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (lines[y][x] == 'X')
                        {
                            cells[x, y - gridYOffset] = new GameOfLife.Cell(x, y, true);
                        }
                        else
                        {
                            cells[x, y - gridYOffset] = new GameOfLife.Cell(x, y, false);
                        }
                    }
                }
                else
                {
                    throw new FormatException("Invalid save file format.");
                }
            }

        }


        const string serialisationFormatVersion = "0";
        public const string savePath = "saves";

    }
}
