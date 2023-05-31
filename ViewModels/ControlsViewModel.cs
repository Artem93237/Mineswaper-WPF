using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Timers;
using Minesweeper.Models;
using ReactiveUI;
using Timer = System.Timers.Timer;

namespace Minesweeper.ViewModels
{
    public class ControlsViewModel : ViewModelBase
    {
        private readonly Timer _timer = new();
        private int _amountMoves;

        private bool _automatic;

        private int _currentMove;
        private int _delay = 300;

        private bool _forward = true;
        private Solver? _solver;

        private bool _solverActive;

        public ControlsViewModel(BoardViewModel boardViewModel, Global global)
        {
            Global = global;
            Board = boardViewModel;
            OnLoadBoardClicked = ReactiveCommand.Create(LoadBoard);
            OnStartClicked = ReactiveCommand.Create(StartOrReset);
            _timer.Elapsed += TimerTick;
        }

        public Global Global { get; }

        public List<Move> Moves { get; } = new();

        public bool SolverActive
        {
            get => _solverActive;
            set => this.RaiseAndSetIfChanged(ref _solverActive, value);
        }

        public int Delay
        {
            get => _delay;
            set
            {
                this.RaiseAndSetIfChanged(ref _delay, value);
                DelayHasChanged();
            }
        }

        public int CurrentMove
        {
            get => _currentMove;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentMove, value);
                CurrentMoveHasChanged();
            }
        }

        public bool Automatic
        {
            get => _automatic;
            set
            {
                this.RaiseAndSetIfChanged(ref _automatic, value);
                AutomaticHasChanged();
            }
        }

        public bool Forward
        {
            get => _forward;
            set => this.RaiseAndSetIfChanged(ref _forward, value);
        }

        public int AmountMoves
        {
            get => _amountMoves;
            set => this.RaiseAndSetIfChanged(ref _amountMoves, value);
        }

        private BoardViewModel Board { get; }
        public ReactiveCommand<Unit, Unit> OnLoadBoardClicked { get; }
        public ReactiveCommand<Unit, Unit> OnStartClicked { get; }

        private void CurrentMoveHasChanged()
        {
            if (Global.GameRunning) Board.ExecuteMove(Moves[CurrentMove]);
        }

        private void AutomaticHasChanged()
        {
            if (_automatic)
                _timer.Start();
            else
                _timer.Stop();
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            Board.ExecuteMove(Moves[_currentMove]);

            if (Forward)
            {
                if (CurrentMove < AmountMoves) CurrentMove++;
            }
            else
            {
                if (CurrentMove > 0) CurrentMove--;
            }
        }

        private void DelayHasChanged()
        {
            _timer.Interval = Delay;
        }

        private void Reset()
        {
            Automatic = false;
            Forward = true;
            Moves.Clear();
            AmountMoves = 0;
            CurrentMove = 0;
        }

        private bool AiHasWon()
        {
            int uncoveredFields = 0;
            Move lastMove = Moves.Last();
            for (int y = 0; y < Global.CurrentRowsAndColumns; y++)
            for (int x = 0; x < Global.CurrentRowsAndColumns; x++)
                if (!lastMove.Fields[y, x].IsCovered)
                    uncoveredFields++;

            int amountFields = Global.CurrentRowsAndColumns * Global.CurrentRowsAndColumns;
            return uncoveredFields - (amountFields - Global.CurrentAmountBombs) == 0;
        }

        private void LoadBoard()
        {
            if (Global.GameRunning || Global.NeedsReset) return;
            Global.CurrentAmountBombs = Global.AmountBombs;
            Global.CurrentRowsAndColumns = Global.RowsAndColumns;
            Board.CreateBoard();
        }

        private void StartOrReset()
        {
            Global.SolverActive = _solverActive;

            if (Global.MainButtonStatus == MainButtonStatuses.Start)
                Start();
            else if (Global.MainButtonStatus == MainButtonStatuses.Reset)
                Stop();
            else if (Global.MainButtonStatus == MainButtonStatuses.Cancel) CancelSolver();
        }

        private void Start()
        {
            Board.Start();
            if (_solverActive) StartSolver();
            else
                Global.MainButtonStatus = MainButtonStatuses.Reset;

            AmountMoves--;
        }

        private void Stop()
        {
            _timer.Stop();
            Board.Stop();
            Reset();
            Global.MainButtonStatus = MainButtonStatuses.Start;
        }

        private void CancelSolver()
        {
            Global.InfoText = "Canceling Solver...";
            _solver!.IsCanceled = true;
        }

        private void StartSolver()
        {
            Global.InfoText = "Solver Started...";
            _solver = new Solver(Global.CurrentRowsAndColumns, Global.CurrentAmountBombs, this);
            TimeSpan solverTimeSpan;
            Global.StartTime = DateTime.Now;
            Global.SolverIsSolving = true;
            Global.MainButtonStatus = MainButtonStatuses.Cancel;
            Thread solverThread = new(() =>
            {
                try
                {
                    _solver.Main();
                    solverTimeSpan = DateTime.Now.Subtract(Global.StartTime);
                    Global.InfoText = AiHasWon()
                        ? $"Solver Won! \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms"
                        : $"Solver Finished \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms";
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(BombUncoveredException))
                    {
                        BombUncoveredException bombUncoveredException = (BombUncoveredException)e;
                        solverTimeSpan = DateTime.Now.Subtract(Global.StartTime);
                        CreationField[,] fields = CloneCreationFields(Moves[AmountMoves - 1].Fields);
                        for (int y = 0; y < Global.CurrentRowsAndColumns; y++)
                        for (int x = 0; x < Global.CurrentRowsAndColumns; x++)
                            if (fields[y, x].HasBomb)
                                fields[y, x].IsCovered = false;

                        fields[bombUncoveredException.Field.Position!.Y, bombUncoveredException.Field.Position.X]
                            .CausedLose = true;

                        Moves.Add(new Move(fields, Moves[_amountMoves - 1].FlagsSet));
                        AmountMoves++;
                        Global.InfoText = $"Solver Lost! \nin {Math.Round(solverTimeSpan.TotalMilliseconds)}ms";
                    }
                    else
                    {
                        Global.InfoText = $"Error was thrown in Solver: {e.Message}";
                    }
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Global.MainButtonStatus = MainButtonStatuses.Reset;
                Global.SolverIsSolving = false;
            });

            solverThread.Start();
        }

        private CreationField[,] CloneCreationFields(CreationField[,] creationFields)
        {
            CreationField[,] fields = new CreationField[Global.CurrentRowsAndColumns, Global.CurrentRowsAndColumns];
            for (int y = 0; y < Global.CurrentRowsAndColumns; y++)
            for (int x = 0; x < Global.CurrentRowsAndColumns; x++)
            {
                CreationField creationField = new()
                {
                    Position = new Point(y, x),
                    Value = creationFields[y, x].Value,
                    HasBomb = creationFields[y, x].HasBomb,
                    IsCovered = creationFields[y, x].IsCovered,
                    IsFlagged = creationFields[y, x].IsFlagged
                };
                fields[y, x] = creationField;
            }

            return fields;
        }
    }
}
