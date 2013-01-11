# Sudoku
###### By David Elner
======

A Sudoku game hacked together for a 24-hour coding competition, with several cool features like a puzzle library, error-checker, hints, and solver.

### Installation

#### Pre-requisites

This game was built on the *.NET 4.0 & Windows Presentation Foundation (WPF)* platform. As such, it is required that you:

* Are running Windows (sorry, fellow Linux geeks!)
* Have the [.NET 4.0 Framework](http://www.microsoft.com/en-us/download/details.aspx?id=17851) or later installed

#### How to install

You may either install Sudoku or run it as a Standalone:

* **Install**:		Double-click `..\install\Sudoku.Install.msi` and follow the on screen instructions.
* **Standalone**:	Double-click `..\install\Sudoku.exe` and start playing.


### Usage
#### Creating/Opening/Managing Puzzles

The Puzzle Library allows organization of all of your Sudoku puzzles!
Go to `File -> Puzzle Library` to open the Puzzle Library interface.

From this screen you can:

1. Create a new Puzzle:
   * Select the puzzle rank (i.e. 3 = 9x9)
   * Enter the name in the text box.
   * Place the given numbers on the grid.
   * Click the Save button.
   * If a puzzle already appears on the screen and you’d like to create one from scratch, click the Clear button.
2. Edit a puzzle:
   * Select your puzzle from the left hand menu.
   * Make any changes you wish.
   * Press the Save button.
3. Delete a puzzle:
   * Select your puzzle from the left hand menu.
   * Press the Delete button.
4. Start a puzzle:
   * Select your puzzle from the left hand menu.
   * Press the Start New button.
5. Resume a puzzle:
   * Select the puzzle which you’d like to resume.
   * This requires having previously saved the puzzle via `File -> Save` on the main window.
   * Press the Resume Puzzle button.

#### Gameplay Features

This version of Sudoku has a number of useful features, all of which are available under the Game menu.

These include:

1. Show Possibilities on Hover
   * You may check/uncheck this option on the Game menu to enable this feature.
   * When you hover over a cell, the possible values you may enter into that cell are listed below the puzzle.
2. Show Errors
   * You may check/uncheck this option on the Game menu to enable this feature.
   * If an error is made in the puzzle (i.e. the value for a cell does not match the value for the known solution), the number will appear highlighted in red.
3. Remove Errors
   * Selecting this from the Game menu will clear all values from the puzzle that does not match the known solution.
4. Solve
   * Selecting this from the Game menu will finish the puzzle with a pre-computed solution.
