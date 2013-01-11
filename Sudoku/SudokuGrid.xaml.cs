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
    /// Interaction logic for SudokuGrid.xaml
    /// </summary>
    public partial class SudokuGrid : UserControl
    {
        #region Members
        private SudokuPuzzle _puzzle;
        private SudokuZone[,] _zones;

        private TextBlock _availableValuesTextBlock;
        private TextBlock _possibleValuesTextBlock;
        #endregion

        #region Constructors
        public SudokuGrid()
        {
            InitializeComponent();

            this._puzzle = new SudokuPuzzle() { Rank = 3 };
            this._zones = new SudokuZone[this._puzzle.Rank, this._puzzle.Rank];

            // Load the UI
            StackPanel gridStackPanel = new StackPanel() { Orientation = Orientation.Vertical };

            for (int i = 0; i < this._puzzle.Rank; i++)
            {
                StackPanel rowStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                for (int j = 0; j < this._puzzle.Rank; j++)
                {
                    SudokuZone zone = new SudokuZone();
                    this._zones[i, j] = zone;
                    rowStackPanel.Children.Add(zone);
                }

                gridStackPanel.Children.Add(rowStackPanel);
            }

            this.SudokuZoneContainer.Child = gridStackPanel;
        }
        public SudokuGrid(SudokuPuzzle puzzle, SudokuPuzzleLoadOptions loadOptions)
        {
            InitializeComponent();

            this._puzzle = puzzle;
            this._zones = new SudokuZone[puzzle.Rank, puzzle.Rank];

            // Load the UI
            StackPanel gridStackPanel = new StackPanel() { Orientation = Orientation.Vertical };

            for (int i = 0; i < puzzle.Rank; i++)
            {
                StackPanel rowStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                for (int j = 0; j < puzzle.Rank; j++)
                {
                    SudokuZone zone = new SudokuZone(this, i, j);
                    this._zones[i, j] = zone;
                    rowStackPanel.Children.Add(zone);
                }

                gridStackPanel.Children.Add(rowStackPanel);
            }

            this.SudokuZoneContainer.Child = gridStackPanel;

            // Load the puzzle into the UI
            switch (loadOptions)
            {
                case SudokuPuzzleLoadOptions.StartNew:
                    for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                        {
                            int value = int.Parse(this._puzzle.Format[i].Split(new char[] { ',' })[j].ToString());

                            SudokuCell cell = this.GetCell(i, j);
                            if (value != 0)
                            {
                                cell.Value = value;
                                cell.State = SudokuCell.CellState.Locked;
                            }
                        }
                    }
                    break;

                case SudokuPuzzleLoadOptions.LoadSavedGame:
                    for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                        {
                            int value = int.Parse(this._puzzle.SaveState[i].Split(new char[] { ',' })[j].ToString());
                            int formatValue = int.Parse(this._puzzle.Format[i].Split(new char[] { ',' })[j].ToString());

                            SudokuCell cell = this.GetCell(i, j);
                            if (value != 0)
                            {
                                if (value != formatValue)
                                {
                                
                                        cell.Value = value;
                                        cell.State = SudokuCell.CellState.Editable;
                                }
                                else
                                {
                                        cell.Value = value;
                                        cell.State = SudokuCell.CellState.Locked;
                                }
                            }
                        }
                    }
                    break;

                case SudokuPuzzleLoadOptions.LoadSolution:
                    for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                        {
                            int value = int.Parse(this._puzzle.Solution[i].Split(new char[] { ',' })[j].ToString());
                            int formatValue = int.Parse(this._puzzle.Format[i].Split(new char[] { ',' })[j].ToString());

                            SudokuCell cell = this.GetCell(i, j);
                            if (value != 0)
                            {
                                if (value != formatValue)
                                {
                                    cell.Value = value;
                                    cell.State = SudokuCell.CellState.Editable;
                                }
                                else
                                {
                                    cell.Value = value;
                                    cell.State = SudokuCell.CellState.Locked;
                                }
                            }
                        }
                    }
                    break;

                case SudokuPuzzleLoadOptions.EditMode:
                    for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                        {
                            int value = int.Parse(this._puzzle.Format[i].Split(new char[] { ',' })[j].ToString());

                            SudokuCell cell = this.GetCell(i, j);
                            if (value != 0)
                            {
                                cell.Value = value;
                            }
                        }
                    }
                    break;

                default:
                    // Do not load anything
                    break;
            }
        }
        public SudokuGrid(SudokuPuzzle puzzle, SudokuPuzzleLoadOptions loadOptions, TextBlock availableValuesTextBlock, TextBlock possibleValuesTextBlock)
            : this(puzzle, loadOptions)
        {
            this._availableValuesTextBlock = availableValuesTextBlock;
            this._possibleValuesTextBlock = possibleValuesTextBlock;

            this.CheckAvailableValues();
        }
        #endregion

        #region Properties
        public SudokuPuzzle Puzzle
        {
            get
            {
                return this._puzzle;
            }
        }
        public SudokuZone[,] Zones
        {
            get
            {
                return this._zones;
            }
        }

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < this._puzzle.Rank; i++)
                {
                    for (int j = 0; j < this._puzzle.Rank; j++)
                    {
                        if (!this._zones[i, j].IsComplete)
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
        public void CheckGameState()
        {
            // Check available values
            this.CheckAvailableValues();

            // Check win condition
            if (this.IsComplete)
            {
                // Show win
            }
        }
        private void CheckAvailableValues()
        {
            List<int> availableValues = new List<int>();

            for (int i = 0; i < this._puzzle.Rank; i++)
            {
                for (int j = 0; j < this._puzzle.Rank; j++)
                {
                    List<int> zoneAvailableValues = this._zones[i, j].GetAvaliableValues();
                    foreach(int availableValue in zoneAvailableValues)
                    {
                        if (!availableValues.Contains(availableValue))
                        {
                            if (availableValues.Count > 0)
                            {
                                // Do insertion sort
                                for (int k = 0; k < availableValues.Count; k++)
                                {
                                    if (availableValue < availableValues[k])
                                    {
                                        availableValues.Insert(k, availableValue);
                                        break;
                                    }
                                    // If last item
                                    else if(k == availableValues.Count-1)
                                    {
                                        availableValues.Add(availableValue);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                availableValues.Add(availableValue);
                            }
                        }
                    }
                }
            }

            // Generate a string show which values are left
            if (this._availableValuesTextBlock != null)
            {
                string availableValuesString = string.Empty;
                foreach (int value in availableValues)
                {
                    if (!string.IsNullOrEmpty(availableValuesString))
                    {
                        availableValuesString += (", " + value.ToString());
                    }
                    else
                    {
                        availableValuesString = value.ToString();
                    }
                }
                this._availableValuesTextBlock.Text = availableValuesString;
            }
        }
        public bool IsValueAvailable(int row, int col, int value)
        {
            int zoneRow = row / this._puzzle.Rank;
            int zoneCol = col / this._puzzle.Rank;
            int zoneCellRow = row % this._puzzle.Rank;
            int zoneCellCol = col % this._puzzle.Rank;

            // Check zone
            if(!this._zones[zoneRow,zoneCol].IsValueAvailable(value))
            {
                return false;
            }

            // Check rows
            for (int i = 0; i < this._puzzle.Rank; i++)
            {
                SudokuZone zone = this._zones[zoneRow, i];
                for (int j = 0; j < this._puzzle.Rank; j++)
                {
                    int gridCol = ((this._puzzle.Rank * i) + j);

                    if (gridCol != col && zone.Cells[zoneCellRow, j].Value.HasValue)
                    {
                        if(zone.Cells[zoneCellRow,j].Value.Value == value)
                        {
                            return false;
                        }
                    }
                }
            }

            // Check cols
            for (int i = 0; i < this._puzzle.Rank; i++)
            {
                SudokuZone zone = this._zones[i, zoneCol];
                for (int j = 0; j < this._puzzle.Rank; j++)
                {
                    int gridRow = ((this._puzzle.Rank * i) + j);

                    // If it is not the parameter cell, and the target has a value set
                    if (gridRow != row && zone.Cells[j, zoneCellCol].Value.HasValue)
                    {
                        if (zone.Cells[j, zoneCellCol].Value.Value == value)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        public List<int> GetAvailableValues(int row, int col)
        {
            // Calculate positions
            int zoneRow = row / this._puzzle.Rank;
            int zoneCol = col / this._puzzle.Rank;
            int zoneCellRow = row % this._puzzle.Rank;
            int zoneCellCol = col % this._puzzle.Rank;

            List<int> availableZoneValues = this._zones[zoneRow, zoneCol].GetAvaliableValues();

            return new List<int>(availableZoneValues.Intersect<int>(this.GetAvailableRowValues(row)).Intersect<int>(this.GetAvailableColumnValues(col)));
        }
        public void DisplayAvailableValues(int row, int col)
        {
            List<int> availableValues = this.GetAvailableValues(row, col);

            // Generate a string show which values are left
            if (this._possibleValuesTextBlock != null)
            {
                string availableValuesString = string.Empty;
                foreach (int value in availableValues)
                {
                    if (!string.IsNullOrEmpty(availableValuesString))
                    {
                        availableValuesString += (", " + value.ToString());
                    }
                    else
                    {
                        availableValuesString = value.ToString();
                    }
                }
                this._possibleValuesTextBlock.Text = availableValuesString;
            }
        }
        public void ClearDisplayOfAvailableValues()
        {
            if (this._possibleValuesTextBlock != null)
            {
                this._possibleValuesTextBlock.Text = string.Empty;
            }
        }
        private List<int> GetAvailableRowValues(int row)
        {
            List<int> result = new List<int>();
            int maxValue = (this.Puzzle.Rank * this.Puzzle.Rank);

            // Generate a list of possible values
            for (int i = 1; i <= maxValue; i++)
            {
                result.Add(i);
            }

            for (int j = 0; j < maxValue; j++)
            {
                SudokuCell cell = GetCell(row, j);
                if(cell.Value.HasValue && result.Contains(cell.Value.Value))
                {
                    result.Remove(cell.Value.Value);
                }
            }

            return result;
        }
        private List<int> GetAvailableColumnValues(int col)
        {
            List<int> result = new List<int>();
            int maxValue = (this.Puzzle.Rank * this.Puzzle.Rank);

            // Generate a list of possible values
            for (int i = 1; i <= maxValue; i++)
            {
                result.Add(i);
            }

            for (int j = 0; j < maxValue; j++)
            {
                SudokuCell cell = GetCell(j, col);
                if (cell.Value.HasValue && result.Contains(cell.Value.Value))
                {
                    result.Remove(cell.Value.Value);
                }
            }

            return result;
        }

        public SudokuCell GetCell(int row, int col)
        {
            int zoneRow = row / this._puzzle.Rank;
            int zoneCol = col / this._puzzle.Rank;
            int zoneCellRow = row % this._puzzle.Rank;
            int zoneCellCol = col % this._puzzle.Rank;

            return this._zones[zoneRow, zoneCol].Cells[zoneCellRow, zoneCellCol];
        }
        public List<string> ReadState()
        {
            List<string> result = new List<string>();

            for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
            {
                string row = string.Empty;
                for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                {
                    SudokuCell cell = this.GetCell(i, j);
                    if (cell.Value.HasValue)
                    {
                        if (string.IsNullOrEmpty(row))
                        {
                            row += cell.Value.Value.ToString();
                        }
                        else
                        {
                            row += ("," + cell.Value.Value.ToString());
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(row))
                        {
                            row += "0";
                        }
                        else
                        {
                            row += (",0");
                        }
                    }
                }
                result.Add(row);
            }

            return result;
        }
        public void RemoveErrors()
        {
            for (int i = 0; i < (this._puzzle.Rank * this._puzzle.Rank); i++)
            {
                string[] solutionValues = this._puzzle.Solution[i].Split(new char[] { ',' });
                for (int j = 0; j < (this._puzzle.Rank * this._puzzle.Rank); j++)
                {
                    SudokuCell cell = this.GetCell(i,j);
                    int currentCellValue = cell.Value.HasValue ? cell.Value.Value : 0;

                    // If the cell doesn't match the solution...
                    if (currentCellValue.ToString() != solutionValues[j])
                    {
                        // Clear it
                        cell.Value = null;
                    }
                }
            }
        }
        #endregion
    }

    public enum SudokuPuzzleLoadOptions
    {
        Blank = 0,
        StartNew = 1,
        LoadSavedGame = 2,
        LoadSolution = 3,
        EditMode = 4
    }
}
