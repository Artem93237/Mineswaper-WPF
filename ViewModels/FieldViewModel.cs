using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Media;
using Minesweeper.Models;
using ReactiveUI;

namespace Minesweeper.ViewModels
{
    public class FieldViewModel : ViewModelBase, IEquatable<FieldViewModel>
    {
        private readonly BoardViewModel _board;

        private IBrush _background;

        private readonly Global _global;

        private bool _hasBomb;
        private bool _hasNumber;

        private bool _isCovered = true;

        private bool _isFlagged;

        private int _value;

        private IBrush _valueColor = Brushes.Black;

        public FieldViewModel(int value, Point position, BoardViewModel board, Global global)
        {
            _global = global;
            OnFieldLeftClicked = ReactiveCommand.Create(FieldLeftClicked);
            Position = position;
            Value = value;
            _board = board;
        }

        public int Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                UpdateFontColor();
            }
        }

        public bool IsCovered
        {
            get => _isCovered;
            set => this.RaiseAndSetIfChanged(ref _isCovered, value);
        }

        public IBrush ValueColor
        {
            get => _valueColor;
            set => this.RaiseAndSetIfChanged(ref _valueColor, value);
        }

        public bool HasBomb
        {
            get => _hasBomb;
            set => this.RaiseAndSetIfChanged(ref _hasBomb, value);
        }

        public ReactiveCommand<Unit, Unit> OnFieldLeftClicked { get; }

        public Point Position { get; }

        public IBrush Background
        {
            get => _background;
            set => this.RaiseAndSetIfChanged(ref _background, value);
        }

        public IBrush CoverColor { get; set; }

        public bool HasNumber
        {
            get => _hasNumber;
            set => this.RaiseAndSetIfChanged(ref _hasNumber, value);
        }

        public bool IsFlagged
        {
            get => _isFlagged;
            set => this.RaiseAndSetIfChanged(ref _isFlagged, value);
        }

        public bool Equals(FieldViewModel? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _board.Equals(other._board) && Position.Equals(other.Position);
        }

        public void FieldLeftClicked()
        {
            if (IsFlagged || !_global.GameRunning || _global.SolverActive) return;
            Console.WriteLine($"Field Left clicked X: {Position.X} Y: {Position.Y}");


            if (!_global.BombsArePlaced) _board.FillInBombs(Position);

            if (!IsCovered)
            {
                if (HasNumber) _board.UncoverEveryFieldSurroundingIfValueMatchesFlags(this);
                if (_board.HasWon()) _board.Win();
            }
            else
            {
                _board.Uncover(this);
                if (HasBomb)
                {
                    List<FieldViewModel> uncoveredBombs = new() { this };
                    _board.GameOver(uncoveredBombs);
                }
                else
                {
                    _board.UncoverSurroundingZeros(this);
                    if (_board.HasWon()) _board.Win();
                }
            }
        }

        public void FieldRightClicked()
        {
            if (!IsCovered || !_global.GameRunning || _global.SolverActive) return;
            Console.WriteLine($"Field Right clicked X: {Position.X} Y: {Position.Y}");
            if (!IsFlagged)
            {
                IsFlagged = true;
                _global.FlagsSet++;
            }
            else
            {
                _global.FlagsSet--;
                IsFlagged = false;
            }

            if (_board.HasWon()) _board.Win();
        }

        private void UpdateFontColor()
        {
            HasNumber = true;

            switch (Value)
            {
                case 0:
                    HasNumber = false;
                    break;
                case 1:
                    ValueColor = Brushes.Blue;
                    HasNumber = true;
                    break;
                case 2:
                    ValueColor = Brushes.Green;
                    HasNumber = true;
                    break;
                case 3:
                    ValueColor = Brushes.Red;
                    HasNumber = true;
                    break;
                case 4:
                    ValueColor = Brushes.Purple;
                    HasNumber = true;
                    break;
                case 5:
                    ValueColor = Brushes.Orange;
                    HasNumber = true;
                    break;
                case 6:
                    ValueColor = Brushes.Yellow;
                    HasNumber = true;
                    break;
                case 7:
                    ValueColor = Brushes.Pink;
                    HasNumber = true;
                    break;
                case 8:
                    ValueColor = Brushes.Black;
                    HasNumber = true;
                    break;
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldViewModel)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_board, Position);
        }

        public static bool operator ==(FieldViewModel? left, FieldViewModel? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FieldViewModel? left, FieldViewModel? right)
        {
            return !Equals(left, right);
        }
    }
}