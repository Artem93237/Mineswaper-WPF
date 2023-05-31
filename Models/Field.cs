using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Minesweeper.Models
{
    public class Field
    {
        private readonly Func<MoveType, Unit> _addMovesFunc;

        private readonly int _value;

        private Point? _position;

        public Field(bool hasBomb, int value, Func<MoveType, Unit> addMovesFunc)
        {
            HasBomb = hasBomb;
            _value = value;
            _addMovesFunc = addMovesFunc;
            IsCovered = true;
        }

        private bool HasBomb { get; }

        [Range(0, 8)]
        public int Value
        {
            /*
             * returns -1 if the field is covered 
             */
            get
            {
                if (IsCovered) return -1;
                return _value;
            }
        }

        public bool IsFlagged { get; private set; }
        public bool IsCovered { get; private set; }

        public Point? Position
        {
            get => _position;
            set
            {
                if (Position == null) _position = value;
            }
        }

        public bool Uncover()
            /*
             * returns true if the field is now uncovered.
             * returns false if the field could not be uncovered because it was flagged.
             * throws a BombUncoveredException if a bomb was uncovered.
             * DON'T CATCH THIS ERROR!
             */
        {
            if (IsFlagged) return false;

            if (HasBomb) throw new BombUncoveredException(GetAsCreationField());

            IsCovered = false;
            AddMove(MoveType.Else);
            return true;
        }

        public bool Flag()
            /*
     * returns true if the field is now flagged.
     * returns false if the field could not be flagged because it was uncovered.
     */
        {
            if (!IsCovered) return false;
            IsFlagged = true;
            AddMove(MoveType.Flag);
            return true;
        }

        public bool UnFlag()
            /*
     * returns true if the field is now unflagged.
     * returns false if the field could not be unflagged because it was uncovered.
     */
        {
            if (!IsCovered) return false;
            IsFlagged = false;
            AddMove(MoveType.Unflag);
            return true;
        }

        private void AddMove(MoveType moveType)
        {
            _addMovesFunc(moveType);
        }

        public CreationField GetAsCreationField()
            /*
             * dont use this function! For the solver
             */
        {
            return new CreationField
            {
                Position = _position,
                HasBomb = HasBomb,
                Value = _value,
                IsCovered = IsCovered,
                IsFlagged = IsFlagged,
                CausedLose = false
            };
        }
    }
}