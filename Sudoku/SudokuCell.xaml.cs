using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
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
    /// Represents a single cell on the sudoku board
    /// </summary>
    public partial class SudokuCell : UserControl
    {
        #region Members
        private SudokuZone _zone;

        private int _zoneRow;
        private int _zoneCol;
        private int? _value;
        private CellState _cellState;
        private TextChangedEventHandler _textChangedHandler;
        #endregion

        #region Constructors
        public SudokuCell()
        {
            InitializeComponent();

            this._zone = null;

            this._zoneRow = 0;
            this._zoneCol = 0;
            this._value = null;
            this._cellState = SudokuCell.CellState.Editable;
            this._textChangedHandler = new TextChangedEventHandler(CellTextBox_TextChanged);
            this.DataContext = this;

            this.CellTextBox.MaxLength = (3 * 3).ToString().Length;

            this.BindTextBoxHandler();
        }
        public SudokuCell(SudokuZone zone, int zoneRow, int zoneCol)
        {
            InitializeComponent();

            this._zone = zone;

            this._zoneRow = zoneRow;
            this._zoneCol = zoneCol;
            this._value = null;
            this._cellState = SudokuCell.CellState.Editable;
            this._textChangedHandler = new TextChangedEventHandler(CellTextBox_TextChanged);
            this.DataContext = this;

            this.CellTextBox.MaxLength = (zone.Grid.Puzzle.Rank * zone.Grid.Puzzle.Rank).ToString().Length;

            this.BindTextBoxHandler();
        }
        #endregion

        #region Properties
        public SudokuZone Zone
        {
            get
            {
                return this._zone;
            }
        }

        public int Row
        {
            get
            {
                return ((this._zone.Grid.Puzzle.Rank * this._zone.Row) + this._zoneRow);
            }
        }
        public int Column
        {
            get
            {
                return ((this._zone.Grid.Puzzle.Rank * this._zone.Column) + this._zoneCol);
            }
        }
        public int? Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (!SudokuRangeRule.Validate(value, this._zone.Grid.Puzzle.Rank))
                {
                    throw new ArgumentOutOfRangeException("Invalid value '" + value + "' for cell.");
                }

                if (!SudokuDuplicateRule.Validate(this.Row, this.Column, value, this._zone.Grid))
                {
                    throw new ArgumentOutOfRangeException("Duplicate value '" + value + "' exists already.");
                }

                // Set the value in the TextBox
                this.UnbindTextBoxHandler();
                if (value.HasValue)
                {
                    this.CellTextBox.Text = value.Value.ToString();
                }
                else
                {
                    this.CellTextBox.Text = string.Empty;
                }
                this.BindTextBoxHandler();

                this._value = value;

                // Poll the grid to check it's state
                this._zone.Grid.CheckGameState();
            }
        }
        public SudokuCell.CellState State
        {
            get
            {
                return this._cellState;
            }
            set
            {
                switch (value)
                {
                    case SudokuCell.CellState.Editable:
                        this.CellTextBox.IsReadOnly = false;
                        this.CellTextBox.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255)));
                        break;

                    case SudokuCell.CellState.ReadOnly:
                        this.CellTextBox.IsReadOnly = true;
                        this.CellTextBox.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255)));
                        break;

                    case SudokuCell.CellState.Locked:
                        this.CellTextBox.IsReadOnly = true;
                        this.CellTextBox.Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(200), Convert.ToByte(200), Convert.ToByte(200)));
                        break;
                }

                this._cellState = value;
            }
        }
        #endregion

        #region Functions
        public void ShowHighlightIfError()
        {
            string solutionStrValue = this._zone.Grid.Puzzle.Solution[this.Row].Split(new char[] { ',' })[this.Column];
            if (solutionStrValue != (this.Value ?? 0).ToString())
            {
                this.CellTextBox.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0)));
            }
            else
            {
                this.CellTextBox.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0)));
            }
        }
        public void RemoveErrorHighlight()
        {
            this.CellTextBox.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0)));
        }
        private void BindTextBoxHandler()
        {
            this.CellTextBox.TextChanged += this._textChangedHandler;
        }
        private void UnbindTextBoxHandler()
        {
            this.CellTextBox.TextChanged -= this._textChangedHandler;
        }
        #endregion

        #region Event handlers
        private void CellTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i;
            int? value = Int32.TryParse(this.CellTextBox.Text, out i) ? i : (int?)null;

            if (SudokuRangeRule.Validate(this.CellTextBox.Text, this._zone.Grid.Puzzle.Rank) && SudokuDuplicateRule.Validate(this.Row, this.Column, value, this._zone.Grid))
            {
                // Permit the change
                this._value = value;

                if (Global.Settings.ShowErrors)
                {
                    this.ShowHighlightIfError();
                }

                // Poll the grid to check it's state
                this._zone.Grid.CheckGameState();
            }
            else
            {
                // Change the value back
                this.UnbindTextBoxHandler();
                if (this._value.HasValue)
                {
                    this.CellTextBox.Text = this._value.Value.ToString();
                }
                else
                {
                    this.CellTextBox.Text = string.Empty;
                }
                this.BindTextBoxHandler();
            }
        }
        private void CellTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Global.Settings.ShowPossibilitiesOnHover && this.State == CellState.Editable)
            {
                this._zone.Grid.DisplayAvailableValues(this.Row, this.Column);
            }
            return;
        }
        private void CellTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Global.Settings.ShowPossibilitiesOnHover && this.State == CellState.Editable)
            {
                this._zone.Grid.ClearDisplayOfAvailableValues();
            }
            return;
        }
        #endregion

        public enum CellState
        {
            /// <summary>
            /// Cell value can be changed
            /// </summary>
            Editable = 0,
            /// <summary>
            /// Cell appears like an editable cell, but cannot be changed. [For preview purposes]
            /// </summary>
            ReadOnly = 1,
            /// <summary>
            /// Cell appears locked. [As a given # of the puzzle]
            /// </summary>
            Locked = 2
        }
    }

    /// <summary>
    /// Defines valid input for Sudoku cell
    /// </summary>
    public class SudokuRangeRule : ValidationRule
    {
        public int Rank { get; set; }

        public static bool Validate(object value, int rank)
        {
            string message;
            return SudokuRangeRule.Validate(value, rank, out message);
        }
        public static bool Validate(object value, int rank, out string message)
        {
            int parameter = 0;

            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                message = null;
                return true;
            }

            if (int.TryParse(value.ToString(), out parameter))
            {
                if (parameter > 0 && parameter <= (rank * rank))
                {
                    message = null;
                    return true;
                }
                else
                {
                    message = "Invalid input. Out of range.";
                    return false;
                }
            }
            else
            {
                message = "Invalid input. Non-integral value";
                return false;
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string message;
            return new ValidationResult(SudokuRangeRule.Validate(value, this.Rank, out message), message);
        }
    }
    public class SudokuDuplicateRule : ValidationRule
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public SudokuGrid Grid { get; set; }

        public static bool Validate(int row, int col, int? value, SudokuGrid grid)
        {
            string message;
            return SudokuDuplicateRule.Validate(row, col, value, grid, out message);
        }
        public static bool Validate(int row, int col, int? value, SudokuGrid grid, out string message)
        {
            if (value == null)
            {
                message = null;
                return true;
            }

            if (grid.IsValueAvailable(row, col, value.Value))
            {
                message = null;
                return true;
            }
            else
            {
                message = "Invalid input. Duplicate value present.";
                return false;
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string message;

            if (SudokuRangeRule.Validate(value, this.Grid.Puzzle.Rank, out message))
            {
                int iValue = int.Parse(value.ToString());
                return new ValidationResult(SudokuDuplicateRule.Validate(this.Row, this.Column, iValue, this.Grid, out message), message);
            }
            else
            {
                return new ValidationResult(false, message);
            }
        }
    }
}
