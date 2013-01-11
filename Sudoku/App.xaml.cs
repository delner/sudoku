using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Try to save the library before closing.
                XmlSerializer serializer = new XmlSerializer(typeof(SudokuLibrary));
                SudokuLibrary library = Global.Data.Library;
                using (FileStream fs = new FileStream("SudokuLibrary.xml", FileMode.Create))
                {
                    serializer.Serialize(fs, library);
                }
            }
            catch (Exception) { }

            base.OnExit(e);
        }
    }

    public static class Global
    {
        public static class Controls
        {
            public static MainWindow MainWindow { get; set; }
        }
        public static class Game
        {
            public static SudokuGrid Grid { get; set; }

            public static bool Solve(SudokuGrid grid, out List<string> solution)
            {
                int[,] g = new int[(grid.Puzzle.Rank * grid.Puzzle.Rank), (grid.Puzzle.Rank * grid.Puzzle.Rank)];

                for (int i = 0; i < (grid.Puzzle.Rank * grid.Puzzle.Rank); i++)
                {
                    for (int j = 0; j < (grid.Puzzle.Rank * grid.Puzzle.Rank); j++)
                    {
                        g[i, j] = grid.GetCell(i, j).Value.HasValue ? grid.GetCell(i, j).Value.Value : 0;
                    }
                }

                bool result = Solve(g, grid.Puzzle.Rank, 0, 0);


                if (result)
                {
                    // Set the the solution
                    solution = new List<string>();
                    for (int i = 0; i < (grid.Puzzle.Rank * grid.Puzzle.Rank); i++)
                    {
                        string line = string.Empty;
                        for (int j = 0; j < (grid.Puzzle.Rank * grid.Puzzle.Rank); j++)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                line += ("," + g[i, j].ToString());
                            }
                            else
                            {
                                line += g[i, j].ToString();
                            }
                        }
                        solution.Add(line);
                    }
                }
                else
                {
                    solution = grid.Puzzle.Format;
                }

                return result;
            }
            public static bool Solve(List<string> grid, int rank, out List<string> solution)
            {
                int[,] g = new int[(rank * rank), (rank * rank)];

                for (int i = 0; i < (rank * rank); i++)
                {
                    for (int j = 0; j < (rank * rank); j++)
                    {
                        g[i, j] = int.Parse(grid[i].Split(new char[] { ',' })[j]);
                    }
                }

                bool result = Solve(g, rank, 0, 0);

                if (result)
                {
                    // Set the the solution
                    solution = new List<string>();
                    for (int i = 0; i < (rank * rank); i++)
                    {
                        string line = string.Empty;
                        for (int j = 0; j < (rank * rank); j++)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                line += ("," + g[i, j].ToString());
                            }
                            else
                            {
                                line += g[i, j].ToString();
                            }
                        }
                        solution.Add(line);
                    }
                }
                else
                {
                    solution = grid;
                }

                return result;
            }
            private static bool Solve(int[,] grid, int rank, int row, int col)
            {
                if (IsComplete(grid, rank))
                {
                    return true;
                }

                int i, j;
                FindNextEmptyCell(grid, rank, out i, out j);
                
                List<int> availableValues = GetAvailableValues(grid, rank, i, j);
                if (availableValues.Count > 0)
                {
                    foreach (int availableValue in availableValues)
                    {
                        //int[,] tempGrid = (int[,])grid.Clone();
                        //tempGrid[i, j] = availableValue;
                        grid[i, j] = availableValue;
                        if (Solve(grid, rank, i, j))
                        {
                            return true;
                        }
                        else
                        {
                            grid[i, j] = 0;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return false;
            }
            private static List<int> GetAvailableValues(int[,] grid, int rank, int row, int col)
            {
                List<int> possibilities = new List<int>();
                int maxValue = (rank * rank);

                // Generate a list of possible values
                for (int i = 1; i <= maxValue; i++)
                {
                    possibilities.Add(i);
                }

                // Check row
                for (int i = 0; i < (rank * rank); i++)
                {
                    if (grid[row, i] > 0 && possibilities.Contains(grid[row, i]))
                    {
                        possibilities.Remove(grid[row, i]);
                    }
                }

                // Check column
                for (int i = 0; i < (rank * rank); i++)
                {
                    if (grid[i, col] > 0 && possibilities.Contains(grid[i, col]))
                    {
                        possibilities.Remove(grid[i, col]);
                    }
                }

                // Check zone
                for (int i = ((row / rank) * rank); i < (((row / rank) * rank) + rank); i++)
                {
                    for (int j = ((col / rank) * rank); j < (((col / rank) * rank) + rank); j++)
                    {
                        if (grid[i, j] > 0 && possibilities.Contains(grid[i, j]))
                        {
                            possibilities.Remove(grid[i, j]);
                        }
                    }
                }

                return possibilities;
            }
            private static bool IsComplete(int[,] grid, int rank)
            {
                for (int i = 0; i < (rank * rank); i++)
                {
                    for (int j = 0; j < (rank * rank); j++)
                    {
                        if (grid[i, j] == 0)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            private static void FindNextEmptyCell(int[,] grid, int rank, out int row, out int col)
            {
                for (int i = 0; i < (rank * rank); i++)
                {
                    for (int j = 0; j < (rank * rank); j++)
                    {
                        if (grid[i,j] == 0)
                        {
                            row = i;
                            col = j;
                            return;
                        }
                    }
                }

                row = -1;
                col = -1;
                return;
            }
        }
        public static class Settings
        {
            public static bool ShowErrors { get; set; }
            public static bool ShowPossibilitiesOnHover { get; set; }
        }
        public static class Data
        {
            static Data()
            {
                // Try to load the library from disk
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SudokuLibrary));
                    using (FileStream fs = new FileStream("SudokuLibrary.xml", FileMode.OpenOrCreate))
                    {
                        Library = (SudokuLibrary)serializer.Deserialize(fs);
                        if (Library == null)
                        {
                            Library = new SudokuLibrary();
                        }
                    }
                }
                catch(Exception)
                {
                    Library = new SudokuLibrary();
                }
            }

            public static SudokuLibrary Library { get; set; }
        }
    }
}
