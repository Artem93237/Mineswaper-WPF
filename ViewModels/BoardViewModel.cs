using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using DynamicData.Binding;
using Minesweeper.Models;

namespace Minesweeper.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        private readonly List<FieldViewModel> _bombs = new(); // Performance
        private readonly Random _rnd = new();

        public BoardViewModel(Global global)
        {
            Global = global;
            CreateBoard();
        }

        public Global Global { get; }

        public ObservableCollectionExtended<FieldViewModel> Fields { get; set; } = new();

        private FieldViewModel? GetField(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Global.CurrentRowsAndColumns || y >= Global.CurrentRowsAndColumns)
                return null;

            int index = Global.CurrentRowsAndColumns * y + x;

            if (index >= Fields.Count || index < 0)
                return null;

            FieldViewModel field = Fields[index];
            return field;
        }

        private FieldViewModel? GetField(Point position)
        {
            return GetField(position.X, position.Y);
        }


        private void RemoveRedBackground()
        {
            bool dark = false;
            for (int i = 0; i < Global.CurrentRowsAndColumns; i++)
            {
                if (Global.CurrentRowsAndColumns % 2 == 0) dark = !dark;

                for (int j = 0; j < Global.CurrentRowsAndColumns; j++)
                {
                    FieldViewModel field = GetField(j, i)!;
                    dark = !dark;
                    if (field.Background.Equals(Brushes.Red))
                        field.Background = dark ? Brushes.Beige : Brushes.LightGray;
                }
            }
        }

        public void CreateBoard()
        {
            if (Global.CurrentAmountBombs > Math.Pow(Global.CurrentRowsAndColumns, 2) / 2)
                Global.CurrentAmountBombs = Global.CurrentAmountBombs = (int)(Math.Pow(Global.CurrentRowsAndColumns, 2) / 4);

            using (Fields.SuspendNotifications())
            {
                Fields.Clear();

                bool dark = false;
                for (int i = 0; i < Global.CurrentRowsAndColumns; i++)
                {
                    if (Global.CurrentRowsAndColumns % 2 == 0) dark = !dark;

                    for (int j = 0; j < Global.CurrentRowsAndColumns; j++)
                    {
                        FieldViewModel field = new(0, new Point(i, j), this, Global);
                        dark = !dark;
                        field.Background = dark ? Brushes.Beige : Brushes.LightGray;
                        field.CoverColor = dark ? Brushes.Green : Brushes.LightGreen;
                        Fields.Add(field);
                    }
                }
            }
        }

        public void FillInBombs(Point firstFieldPosition)
        {
            Global.NeedsReset = true;
            Global.StartTime = DateTime.Now;
            Global.BombsArePlaced = true;
            AddBombs(Global.CurrentAmountBombs, firstFieldPosition);
            GenerateValue();
        }

        private void GenerateValue()
        {
            foreach (FieldViewModel field in Fields)
            {
                IEnumerable<FieldViewModel> surroundingFields = GetSurroundingFields(field.Position);
                int value = surroundingFields.Count(f => f.HasBomb);
                field.Value = field.HasBomb ? 0 : value;
                field.HasNumber = field.Value > 0;
            }
        }

        private void AddBombs(int amount, Point firstFieldPosition)
        {
            List<Point> availablePoints = new();
            for (int i = 0; i < Global.CurrentRowsAndColumns; i++)
            for (int j = 0; j < Global.CurrentRowsAndColumns; j++)
            {
                if (Math.Abs(j - firstFieldPosition.X) <= 1 &&
                    Math.Abs(i - firstFieldPosition.Y) <= 1) // It is impossible to hit a bomb on the first click
                    continue;

                availablePoints.Add(new Point(i, j));
            }

            for (int i = 0; i < amount; i++)
            {
                int pointIndex = _rnd.Next(availablePoints.Count);
                Point point = availablePoints[pointIndex];
                FieldViewModel bomb = GetField(point)!;
                bomb.HasBomb = true;
                _bombs.Add(bomb);
                availablePoints.Remove(point);
            }
        }

        public void UncoverSurroundingZeros(FieldViewModel field)
        {
            if (field.Value != 0) return;
            foreach (FieldViewModel surroundingUncoveredField in GetSurroundingFields(field.Position!))
            {
                if (!surroundingUncoveredField.IsCovered) continue;
                Uncover(surroundingUncoveredField); // if you dont uncover it, then it will return -1 as value
                if (surroundingUncoveredField.Value == 0) UncoverSurroundingZeros(surroundingUncoveredField);
            }
        }

        public void Uncover(FieldViewModel field)
        {
            field.IsCovered = false;
        }

        public void UncoverEveryFieldSurroundingIfValueMatchesFlags(FieldViewModel field)
        {
            IEnumerable<FieldViewModel> surroundingFields = GetSurroundingFields(field.Position);
            if (field.Value != surroundingFields.Count(f => f.IsFlagged))
                return;
            bool isGameOver = false;
            List<FieldViewModel> uncoveredBombs = new();
            foreach (FieldViewModel fieldViewModel in surroundingFields.Where(f => !f.IsFlagged))
            {
                if (fieldViewModel.IsCovered) // should improve performance
                    Uncover(fieldViewModel);

                if (fieldViewModel.Value == 0) UncoverSurroundingZeros(fieldViewModel);

                if (fieldViewModel.HasBomb)
                {
                    isGameOver = true;
                    uncoveredBombs.Add(fieldViewModel);
                }
            }

            if (isGameOver) GameOver(uncoveredBombs);
        }

        private IEnumerable<FieldViewModel> GetSurroundingFields(Point position)
        {
            FieldViewModel? topLeft = GetField(position.X - 1, position.Y - 1);
            if (topLeft != null)
                yield return topLeft;

            FieldViewModel? top = GetField(position.X, position.Y - 1);
            if (top != null)
                yield return top;

            FieldViewModel? topRight = GetField(position.X + 1, position.Y - 1);
            if (topRight != null)
                yield return topRight;

            FieldViewModel? right = GetField(position.X + 1, position.Y);
            if (right != null)
                yield return right;

            FieldViewModel? bottomRight = GetField(position.X + 1, position.Y + 1);
            if (bottomRight != null)
                yield return bottomRight;

            FieldViewModel? bottom = GetField(position.X, position.Y + 1);
            if (bottom != null)
                yield return bottom;

            FieldViewModel? bottomLeft = GetField(position.X - 1, position.Y + 1);
            if (bottomLeft != null)
                yield return bottomLeft;

            FieldViewModel? left = GetField(position.X - 1, position.Y);
            if (left != null)
                yield return left;
        }

        private void Reset()
        {
            foreach (FieldViewModel field in Fields)
            {
                field.HasBomb = false;
                field.IsCovered = true;
                field.Value = 0;
                field.IsFlagged = false;
                field.HasNumber = false;
                _bombs.Clear();
            }

            Global.Reset();
        }

        public bool HasWon()
        {
            return !Fields.Any(f => !f.HasBomb && f.IsCovered);
        }

        public void GameOver(List<FieldViewModel> uncoveredBombs)
        {
            foreach (FieldViewModel uncoveredBomb in uncoveredBombs) uncoveredBomb.Background = Brushes.Red;
            foreach (FieldViewModel bomb in _bombs) bomb.IsCovered = false;
            Global.Stop();
            Global.InfoText = "You Lost!";
        }

        public void Win()
        {
            Global.Stop();
            Global.InfoText =
                $"You Won! \n You needed \n {Math.Floor(DateTime.Now.Subtract(Global.StartTime).TotalSeconds)}sec";
        }

        public void Start()
        {
            RemoveRedBackground();
            Global.Start();
            Global.InfoText = "Game running";
        }

        public void Stop()
        {
            Reset();
            Global.InfoText = "Game stopped";
        }

        public void ExecuteMove(Move move)
        {
            if (Global.FlagsSet != move.FlagsSet) Global.FlagsSet = move.FlagsSet;
            for (int column = 0; column < Global.CurrentRowsAndColumns; column++)
            for (int row = 0; row < Global.CurrentRowsAndColumns; row++)
            {
                FieldViewModel fieldViewModel = GetField(column, row)!;
                CreationField field = move.Fields[column, row];
                if (fieldViewModel.IsCovered != field.IsCovered) fieldViewModel.IsCovered = field.IsCovered;

                if (fieldViewModel.IsFlagged != field.IsFlagged) fieldViewModel.IsFlagged = field.IsFlagged;

                if (fieldViewModel.Value != field.Value) fieldViewModel.Value = field.Value;

                if (fieldViewModel.HasBomb != field.HasBomb) fieldViewModel.HasBomb = field.HasBomb;

                if (fieldViewModel.Background.Equals(Brushes.Red) && !field.CausedLose) RemoveRedBackground();

                if (!fieldViewModel.Background.Equals(Brushes.Red) && field.CausedLose)
                    fieldViewModel.Background = Brushes.Red;
            }
        }
    }
}
