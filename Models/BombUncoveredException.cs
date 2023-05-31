using System;

namespace Minesweeper.Models
{
    public class BombUncoveredException : Exception
    {
        public readonly CreationField Field;

        public BombUncoveredException(CreationField field)
            /*
             * DON'T CATCH THIS ERROR!
             */
        {
            Field = field;
        }
    }
}