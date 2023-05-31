using System;
using ReactiveUI;

namespace Minesweeper.Models
{
    public class Global : ReactiveObject
    {
        private int _amountBombs = 25;
        private int _currentAmountBombs = 25; // only changes when the board changes
        private int _currentRowsAndColumns = 15; // only changes when the board changes


        private int _flagsSet;

        private bool _gameRunning;


        private string _infoText = "";
        private MainButtonStatuses _mainButtonStatus = MainButtonStatuses.Start;

        private string _mainButtonText = "Start";
        private int _maxBombs = 112;

        private int _rowsAndColumns = 15;
        private bool _solverIsSolving;

        public bool BombsArePlaced;
        public bool SolverActive = false;

        public DateTime StartTime;

        public bool SolverIsSolving
        {
            get => _solverIsSolving;
            set => this.RaiseAndSetIfChanged(ref _solverIsSolving, value);
        }

        public bool NeedsReset { get; set; }

        public int CurrentRowsAndColumns
        {
            get => _currentRowsAndColumns;
            set => this.RaiseAndSetIfChanged(ref _currentRowsAndColumns, value);
        }

        public int CurrentAmountBombs
        {
            get => _currentAmountBombs;
            set => this.RaiseAndSetIfChanged(ref _currentAmountBombs, value);
        }

        public string MainButtonText
        {
            get => _mainButtonText;
            set => this.RaiseAndSetIfChanged(ref _mainButtonText, value);
        }

        public int AmountBombs
        {
            get => _amountBombs;
            set => this.RaiseAndSetIfChanged(ref _amountBombs, value);
        }

        public bool GameRunning
        {
            get => _gameRunning;
            private set => this.RaiseAndSetIfChanged(ref _gameRunning, value);
        }


        public int RowsAndColumns
        {
            get => _rowsAndColumns;
            set
            {
                this.RaiseAndSetIfChanged(ref _rowsAndColumns, value);
                MaxBombs = (int)(Math.Pow(value, 2) / 2);
            }
        }

        public int MaxBombs
        {
            get => _maxBombs;
            set => this.RaiseAndSetIfChanged(ref _maxBombs, value);
        }


        public MainButtonStatuses MainButtonStatus
        {
            get => _mainButtonStatus;
            set
            {
                this.RaiseAndSetIfChanged(ref _mainButtonStatus, value);
                switch (value)
                {
                    case MainButtonStatuses.Start:
                        MainButtonText = "Start";
                        return;
                    case MainButtonStatuses.Reset:
                        MainButtonText = "Reset";
                        return;
                    case MainButtonStatuses.Cancel:
                        MainButtonText = "Cancel";
                        return;
                }
            }
        }

        public string InfoText
        {
            get => _infoText;
            set => this.RaiseAndSetIfChanged(ref _infoText, value);
        }


        public int FlagsSet
        {
            get => _flagsSet;
            set => this.RaiseAndSetIfChanged(ref _flagsSet, value);
        }

        public void Reset()
        {
            Stop();
            InfoText = "";
            FlagsSet = 0;
            BombsArePlaced = false;
            NeedsReset = false;
        }

        public void Start()
        {
            GameRunning = true;
            NeedsReset = true;
        }

        public void Stop()
        {
            GameRunning = false;
        }
    }

    public enum MainButtonStatuses
    {
        Start,
        Reset,
        Cancel
    }
}