using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sudoku
{
    [Serializable]
    public class SudokuLibrary
    {
        #region Members
        private List<SudokuPuzzle> _puzzles;
        #endregion

        #region Constructors
        public SudokuLibrary()
        {
            this._puzzles = new List<SudokuPuzzle>();
        }
        #endregion

        #region Properties
        public List<SudokuPuzzle> Puzzles
        {
            get
            {
                return this._puzzles;
            }
            set
            {
                this._puzzles = value;
            }
        }
        #endregion

        #region Functions
        #endregion
    }

    [Serializable]
    public class SudokuPuzzle
    {
        #region Members
        List<string> _format;
        List<string> _solution;
        List<string> _saveState;
        #endregion

        #region Constructors
        public SudokuPuzzle()
        {
            this.Id = Guid.NewGuid().ToString();

            this._format = new List<string>();
            this._solution = new List<string>();
            this._saveState = new List<string>();
        }
        #endregion

        #region Properties
        [XmlIgnore()]
        public string Id { get; set; }

        public string Name { get; set; }
        public int Rank { get; set; }
        public List<string> Format { get; set; }
        public List<string> Solution { get; set; }
        public List<string> SaveState { get; set; }
        #endregion

        #region Functions
        #endregion
    }
}
