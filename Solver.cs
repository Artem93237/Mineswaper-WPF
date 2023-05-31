using Minesweeper.Models;
using Minesweeper.ViewModels;

namespace Minesweeper
{
    public class Solver : SolverBase
    {
        // don't delete this!
        public Solver(int rowsAmount, int amountBombs, ControlsViewModel controlsViewModel) : base(rowsAmount,
            amountBombs, controlsViewModel)
        {
        }

        // write your solver here ↓
        public void Main() // This method will be executed by the program
        {
            // This starts your solver
            // y: 0, x: 0 is in the top left corner
            // The specified point is the location where the first click is simulated.
            // At the specified point wont be a bomb
            PlaceBombs(new Point(0, 0));

            // Flag a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.Flag();

            // Unflag a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.UnFlag();

            // Uncover a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.Uncover();

            // Get a field's value
            // The "!" means, that we are sure, that it will NOT return null
            int value = GetField(2, 3)!.Value;

            // Get if a field is covered or not
            // The "!" means, that we are sure, that it will NOT return null
            bool isCovered = GetField(2, 3)!.IsCovered;

            // Get if a field is flagged or not
            // The "!" means, that we are sure, that it will NOT return null
            bool isFlagged = GetField(2, 3)!.IsFlagged;

            // You can also get a Field with a point
            // The "!" means, that we are sure, that it will NOT return null
            Field field = GetField(new Point(2, 3))!;

            // Get fields manually
            field = Fields[2, 3];

            // Get the amounts of rows or columns of the board
            int amountRows = RowsAmount;

            // Get the total amount of Bombs on the board
            int amountBombs = AmountBombs;

            // This will be set to true if the user clicks cancel in the gui. If you don't exit if this is set to true, then will just nothing happen, when cancel is clicked.
            bool isCanceled = IsCanceled;

            // If this is set to false, then no moves will be added until it's true again
            bool addMoves = AddMoves; 
                
            // DON'T use this!
            // This could only be used for cheating so just dont
            GetField(2, 3)!.GetAsCreationField();
        }
    }
}