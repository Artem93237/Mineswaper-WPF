namespace Minesweeper.Models
{
    public class Move
    {
        public readonly CreationField[,] Fields;
        public readonly int FlagsSet;

        public Move(CreationField[,] fields, int flagsSet)
        {
            Fields = fields;
            FlagsSet = flagsSet;
        }
    }
}