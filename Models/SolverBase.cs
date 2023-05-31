using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Minesweeper.ViewModels;
using ReactiveUI;

namespace Minesweeper.Models
{
    public class SolverBase
    {
        private readonly ControlsViewModel _controlsViewModel;
        private readonly CreationField[,] _creationFields;
        private readonly Random _rnd = new();
        private int _flagsSet;

        protected SolverBase(int rowsAmount, int amountBombs, ControlsViewModel controlsViewModel)
        {
            RowsAmount = rowsAmount;
            AmountBombs = amountBombs;
            _creationFields = new CreationField[rowsAmount, rowsAmount];
            Fields = new Field[rowsAmount, rowsAmount];
            _controlsViewModel = controlsViewModel;
        }

        protected int RowsAmount { get; }
        protected int AmountBombs { get; }
        protected Field[,] Fields { get; }
        public bool IsCanceled { get; set; }
        protected bool AddMoves { get; set; } = true; 
        protected Field? GetField(int x, int y)
        {
            if (x < 0 || y < 0 || x >= RowsAmount || y >= RowsAmount)
                return null;

            Field field = Fields[y, x];
            return field;
        }

        protected Field? GetField(Point position)
        {
            return GetField(position.X, position.Y);
        }

        private Unit AddMove(MoveType moveType = MoveType.Else)
        {
            if (!AddMoves) return new Unit();
            if (moveType == MoveType.Flag) _flagsSet++;

            if (moveType == MoveType.Unflag) _flagsSet--;
            Move move = new(FieldsToCreationFields(Fields), _flagsSet);
            _controlsViewModel.Moves.Add(move);
            _controlsViewModel.AmountMoves++;
            return new Unit();
        }

        private CreationField[,] FieldsToCreationFields(Field[,] fields)
        {
            CreationField[,] creationFields = new CreationField[RowsAmount, RowsAmount];
            for (int column = 0; column < RowsAmount; column++)
            for (int row = 0; row < RowsAmount; row++)
                creationFields[column, row] = fields[column, row].GetAsCreationField();

            return creationFields;
        }

        // the Region SimulationBoard contains methods which could be helpful for the solver but are
        // NOT intended to be available to the solver developer. the idea is, that the solver developer writes it's own
        // versions of the methods contained in SimulationBoard

        #region SimulationBoard

        private CreationField? GetCreationField(int x, int y)
        {
            if (x < 0 || y < 0 || x >= RowsAmount || y >= RowsAmount)
                return null;

            CreationField field = _creationFields[y, x];
            return field;
        }


        protected void PlaceBombs(Point startPoint)
            /*
             * This method simulates the first click on the board. This means it will generate the bombs and the values for every field
             * the startPoint is the fields position on which you want to simulate the first click.
             */
        {
            GenerateBoardWithBombsAndValue(startPoint);
            FillInFieldsWithCreationFields();
            if (_controlsViewModel.Moves.Count == 0) AddMove();
            GetField(startPoint)!.Uncover();
        }

        private void FillInFieldsWithCreationFields()
        {
            for (int column = 0; column < RowsAmount; column++)
            for (int row = 0; row < RowsAmount; row++)
            {
                CreationField creationField = _creationFields[column, row];
                Fields[column, row] = new Field(creationField.HasBomb, creationField.Value, AddMove)
                    { Position = creationField.Position };
            }
        }

        private void GenerateBoardWithBombsAndValue(Point startPoint)
            /*
             * This method only creates a board (_creationFields) filled with CreationFields you need to convert everything
             * to Field if you want to use it for the Fields board.
             */
        {
            CreateBoard();
            AddBombs(startPoint);
            GenerateValue();
        }

        private void CreateBoard()
        {
            for (int column = 0; column < RowsAmount; column++)
            for (int row = 0; row < RowsAmount; row++)
                _creationFields[column, row] = new CreationField
                {
                    Position = new Point(column, row)
                };
        }

        private void AddBombs(Point firstFieldPosition)
        {
            List<Point> availablePoints = new();
            for (int i = 0; i < RowsAmount; i++)
            for (int j = 0; j < RowsAmount; j++)
            {
                if (Math.Abs(j - firstFieldPosition.X) <= 1 &&
                    Math.Abs(i - firstFieldPosition.Y) <= 1) // It is impossible to hit a bomb on the first click
                    continue;

                availablePoints.Add(new Point(i, j));
            }

            for (int i = 0; i < AmountBombs; i++)
            {
                int pointIndex = _rnd.Next(availablePoints.Count);
                Point point = availablePoints[pointIndex];
                CreationField bomb = GetCreationField(point.X, point.Y)!;
                bomb.HasBomb = true;
                availablePoints.Remove(point);
            }
        }

        private void GenerateValue()
        {
            foreach (CreationField field in _creationFields)
            {
                IEnumerable<CreationField> surroundingFields = GetSurroundingCreationFields(field.Position!);
                int value = surroundingFields.Count(f => f.HasBomb);
                field.Value = field.HasBomb ? 0 : value;
            }
        }

        private IEnumerable<CreationField> GetSurroundingCreationFields(Point position)
        {
            CreationField? topLeft = GetCreationField(position.X - 1, position.Y - 1);
            if (topLeft != null)
                yield return topLeft;

            CreationField? top = GetCreationField(position.X, position.Y - 1);
            if (top != null)
                yield return top;

            CreationField? topRight = GetCreationField(position.X + 1, position.Y - 1);
            if (topRight != null)
                yield return topRight;

            CreationField? right = GetCreationField(position.X + 1, position.Y);
            if (right != null)
                yield return right;

            CreationField? bottomRight = GetCreationField(position.X + 1, position.Y + 1);
            if (bottomRight != null)
                yield return bottomRight;

            CreationField? bottom = GetCreationField(position.X, position.Y + 1);
            if (bottom != null)
                yield return bottom;

            CreationField? bottomLeft = GetCreationField(position.X - 1, position.Y + 1);
            if (bottomLeft != null)
                yield return bottomLeft;

            CreationField? left = GetCreationField(position.X - 1, position.Y);
            if (left != null)
                yield return left;
        }

        #endregion
    }

    public class CreationField
    {
        public Point? Position { get; init; }
        public int Value { get; set; }
        public bool HasBomb { get; set; }
        public bool IsCovered { get; set; }
        public bool IsFlagged { get; init; }
        public bool CausedLose { get; set; }
    }

    public enum MoveType
    {
        Flag,
        Unflag,
        Else
    }
}
