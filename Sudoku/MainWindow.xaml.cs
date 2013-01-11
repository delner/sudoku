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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            Global.Controls.MainWindow = this;
            Global.Settings.ShowPossibilitiesOnHover = true;

            Global.Game.Grid = new SudokuGrid(new SudokuPuzzle() { Rank = 3 }, SudokuPuzzleLoadOptions.Blank, this.RemainingNumbersTextBlock, this.PossibleValuesTextBlock) { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            this.SudokuGridContainer.Child = Global.Game.Grid;
        }
        #endregion

        #region Properties
        #endregion

        #region Functions
        public void LoadPuzzle(SudokuPuzzle puzzle, SudokuPuzzleLoadOptions loadOptions)
        {
            Global.Game.Grid = new SudokuGrid(puzzle, loadOptions, this.RemainingNumbersTextBlock, this.PossibleValuesTextBlock) { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            this.SudokuGridContainer.Child = Global.Game.Grid;
        }
        #endregion

        #region Event handlers
        #region File menu
        private void PuzzleLibraryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LibraryWindow libraryWindow = new LibraryWindow();
            libraryWindow.ShowDialog();
        }
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SudokuPuzzle puzzle = Global.Game.Grid.Puzzle;
            puzzle.SaveState = Global.Game.Grid.ReadState();
        }
        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Game menu
        private void ShowPossibilitiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = this.ShowPossibilitiesMenuItem.IsChecked;
            switch(enabled)
            {
                case true:
                    this.PossibleValuesPanel.Visibility = System.Windows.Visibility.Visible;
                    break;

                case false:
                    this.PossibleValuesPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
            Global.Settings.ShowPossibilitiesOnHover = enabled;
        }
        private void ShowErrorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = this.ShowErrorsMenuItem.IsChecked;
            switch (enabled)
            {
                case true:
                    for (int i = 0; i < (Global.Game.Grid.Puzzle.Rank * Global.Game.Grid.Puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (Global.Game.Grid.Puzzle.Rank * Global.Game.Grid.Puzzle.Rank); j++)
                        {
                            SudokuCell cell = Global.Game.Grid.GetCell(i, j);
                            cell.ShowHighlightIfError();
                        }
                    }
                    break;

                case false:
                    for (int i = 0; i < (Global.Game.Grid.Puzzle.Rank * Global.Game.Grid.Puzzle.Rank); i++)
                    {
                        for (int j = 0; j < (Global.Game.Grid.Puzzle.Rank * Global.Game.Grid.Puzzle.Rank); j++)
                        {
                            SudokuCell cell = Global.Game.Grid.GetCell(i, j);
                            cell.RemoveErrorHighlight();
                        }
                    }
                    break;
            }
            Global.Settings.ShowErrors = enabled;
        }
        private void RemoveErrorsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Global.Game.Grid.RemoveErrors();
        }
        private void SolveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LoadPuzzle(Global.Game.Grid.Puzzle, SudokuPuzzleLoadOptions.LoadSolution);
        }
        #endregion

        #region Help menu
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
        #endregion
        #endregion
    }
}
