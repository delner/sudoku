using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for SudokuZone.xaml
    /// </summary>
    public partial class SudokuZone : UserControl
    {
        #region Members
        private SudokuGrid _grid;
        private SudokuCell[,] _cells;

        private int _row;
        private int _column;
        #endregion

        #region Constructors
        public SudokuZone()
        {
            InitializeComponent();

            this._grid = null;
            this._cells = new SudokuCell[3, 3];
            this._row = 0;
            this._column = 0;

            StackPanel zoneStackPanel = new StackPanel() { Orientation = Orientation.Vertical };

            for (int i = 0; i < 3; i++)
            {
                StackPanel rowStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                for (int j = 0; j < 3; j++)
                {
                    SudokuCell cell = new SudokuCell();
                    this._cells[i, j] = cell;
                    rowStackPanel.Children.Add(cell);
                }

                zoneStackPanel.Children.Add(rowStackPanel);
            }

            this.SudokuCellContainer.Child = zoneStackPanel;
        }
        public SudokuZone(SudokuGrid grid, int row, int col)
        {
            InitializeComponent();

            this._grid = grid;
            this._cells = new SudokuCell[grid.Puzzle.Rank, grid.Puzzle.Rank];
            this._row = row;
            this._column = col;

            StackPanel zoneStackPanel = new StackPanel() { Orientation = Orientation.Vertical };

            for (int i = 0; i < grid.Puzzle.Rank; i++)
            {
                StackPanel rowStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                for (int j = 0; j < grid.Puzzle.Rank; j++)
                {
                    SudokuCell cell = new SudokuCell(this, i, j);
                    this._cells[i, j] = cell;
                    rowStackPanel.Children.Add(cell);
                }

                zoneStackPanel.Children.Add(rowStackPanel);
            }

            this.SudokuCellContainer.Child = zoneStackPanel;
        }
        #endregion

        #region Properties
        public SudokuGrid Grid
        {
            get
            {
                return this._grid;
            }
        }
        public SudokuCell[,] Cells
        {
            get
            {
                return this._cells;
            }
        }

        public int Row
        {
            get
            {
                return this._row;
            }
        }
        public int Column
        {
            get
            {
                return this._column;
            }
        }

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < this._grid.Puzzle.Rank; i++ )
                {
                    for (int j = 0; j < this._grid.Puzzle.Rank; j++)
                    {
                        if (!this._cells[i, j].Value.HasValue)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
        #endregion

        #region Functions
        public List<int> GetUsedValues()
        {
            List<int> result = new List<int>();

            for (int i = 0; i < this._grid.Puzzle.Rank; i++)
            {
                for (int j = 0; j < this._grid.Puzzle.Rank; j++)
                {
                    if (this._cells[i, j].Value.HasValue)
                    {
                        result.Add(this._cells[i, j].Value.Value);
                    }
                }
            }

            return result;
        }
        public List<int> GetAvaliableValues()
        {
            List<int> result = new List<int>();

            // Generate a list of possible values
            for (int i = 1; i <= (this._grid.Puzzle.Rank * this._grid.Puzzle.Rank); i++)
            {
                result.Add(i);
            }

            foreach (int usedValue in GetUsedValues())
            {
                if (result.Contains(usedValue))
                {
                    result.Remove(usedValue);
                }
            }

            return result;
        }
        public bool IsValueAvailable(int value)
        {
            return !(GetUsedValues().Contains(value));
        }
        #endregion
    }
}
