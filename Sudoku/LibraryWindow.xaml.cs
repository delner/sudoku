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
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for LibraryWindow.xaml
    /// </summary>
    public partial class LibraryWindow : Window
    {
        #region Members
        private string _currentPuzzleId = string.Empty;

        private RoutedPropertyChangedEventHandler<double> _sliderChangedHandler;
        #endregion

        #region Constructors
        public LibraryWindow()
        {
            InitializeComponent();

            this._sliderChangedHandler = new RoutedPropertyChangedEventHandler<double>(this.RankSlider_ValueChanged);

            this.LoadBlankPuzzle(3);

            this.PuzzleListBox.Items.Clear();
            this.PuzzleListBox.ItemsSource = Global.Data.Library.Puzzles;

            this.RankSlider.ValueChanged += this._sliderChangedHandler;
        }
        #endregion

        #region Properties
        #endregion

        #region Functions
        public void LoadBlankPuzzle(int rank)
        {
            SudokuGrid grid = new SudokuGrid(new SudokuPuzzle() { Rank = rank }, SudokuPuzzleLoadOptions.Blank) { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            this._currentPuzzleId = grid.Puzzle.Id;
            this.PuzzlePreviewContainer.Child = grid;
        }
        public SudokuPuzzle SaveCurrentPuzzle()
        {
            SudokuGrid grid = (SudokuGrid)this.PuzzlePreviewContainer.Child;

            // If its a new puzzle
            if (this.PuzzleListBox.SelectedItem == null)
            {
                grid.Puzzle.Format = grid.ReadState();
                grid.Puzzle.SaveState = grid.Puzzle.Format;
                grid.Puzzle.Name = !string.IsNullOrEmpty(this.PuzzleNameTextBox.Text.Trim()) ? this.PuzzleNameTextBox.Text.Trim() : ("Puzzle " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                grid.Puzzle.Rank = Convert.ToInt32(this.RankSlider.Value);

                // Compute solution
                List<string> solution;
                if (Global.Game.Solve(grid.Puzzle.Format, Convert.ToInt32(this.RankSlider.Value), out solution))
                {
                    grid.Puzzle.Solution = solution;
                }
                else
                {
                    grid.Puzzle.Solution = grid.Puzzle.Format;
                }

                // Add it to the library
                Global.Data.Library.Puzzles.Add(grid.Puzzle);

                return grid.Puzzle;
            }
            else
            {
                // Save it over the existing puzzle
                SudokuPuzzle targetPuzzle = (SudokuPuzzle)this.PuzzleListBox.SelectedItem;

                List<string> format = grid.ReadState();
                if (!this.IsFormatEqual(format, targetPuzzle.Format))
                {
                    // Format changed: write fresh data
                    targetPuzzle.Format = format;
                    targetPuzzle.SaveState = format;
                    targetPuzzle.Rank = Convert.ToInt32(this.RankSlider.Value);

                    // Compute solution
                    List<string> solution;
                    if (Global.Game.Solve(format, Convert.ToInt32(this.RankSlider.Value), out solution))
                    {
                        targetPuzzle.Solution = solution;
                    }
                    else
                    {
                        targetPuzzle.Solution = format;
                    }
                }
                targetPuzzle.Name = this.PuzzleNameTextBox.Text;

                return targetPuzzle;
            }
        }
        private bool IsFormatEqual(List<string> formatA, List<string> formatB)
        {
            bool result = true;

            if (formatA.Count != formatB.Count)
            {
                return false;
            }
            for (int i = 0; i < formatA.Count; i++)
            {
                if (!formatA[i].Equals(formatB[i]))
                {
                    return false;
                }
            }

            return result;
        }
        #endregion

        #region Event handlers
        private void PuzzleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PuzzleListBox.SelectedItem != null)
            {
                SudokuPuzzle puzzle = (SudokuPuzzle)this.PuzzleListBox.SelectedItem;
                this._currentPuzzleId = puzzle.Id;
                this.PuzzleNameTextBox.Text = puzzle.Name;
                this.RankSlider.Value = puzzle.Rank;
                this.PuzzlePreviewContainer.Child = new SudokuGrid(puzzle, SudokuPuzzleLoadOptions.EditMode) { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };

                this.DeleteButton.IsEnabled = true;
                //this.ResumeButton.IsEnabled = true;
                //this.StartNewButton.IsEnabled = true;
            }
            else
            {
                // Clear the preview pane
                this.PuzzleNameTextBox.Text = string.Empty;
                this.LoadBlankPuzzle(Convert.ToInt32(this.RankSlider.Value));
                this.DeleteButton.IsEnabled = false;
                //this.ResumeButton.IsEnabled = false;
                //this.StartNewButton.IsEnabled = false;
            }
        }

        private void RankSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newRank = Convert.ToInt32(this.RankSlider.Value);
            this.RankSliderValueTextBlock.Text = newRank.ToString();

            this.LoadBlankPuzzle(newRank);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PuzzleListBox.SelectedItem != null)
            {
                this.PuzzleListBox.SelectedItem = null;
            }
            else
            {
                this.PuzzleNameTextBox.Text = string.Empty;
                this.LoadBlankPuzzle(Convert.ToInt32(this.RankSlider.Value));
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.SaveCurrentPuzzle();

            // Refresh the list box
            this.PuzzleListBox.ItemsSource = null;
            this.PuzzleListBox.ItemsSource = Global.Data.Library.Puzzles;
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PuzzleListBox.SelectedItem != null)
            {
                SudokuPuzzle puzzle = (SudokuPuzzle)this.PuzzleListBox.SelectedItem;
                Global.Data.Library.Puzzles.Remove(puzzle);

                // Refresh the list box
                this.PuzzleListBox.ItemsSource = null;
                this.PuzzleListBox.ItemsSource = Global.Data.Library.Puzzles;
            }
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            SudokuPuzzle puzzle = this.SaveCurrentPuzzle();
            Global.Controls.MainWindow.LoadPuzzle(puzzle, SudokuPuzzleLoadOptions.LoadSavedGame);
            this.Close();
        }
        private void StartNewButton_Click(object sender, RoutedEventArgs e)
        {
            SudokuPuzzle puzzle = this.SaveCurrentPuzzle();
            Global.Controls.MainWindow.LoadPuzzle(puzzle, SudokuPuzzleLoadOptions.StartNew);
            this.Close();
        }
        #endregion
    }
}
