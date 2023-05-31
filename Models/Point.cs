using System;

namespace Minesweeper.Models
{
    public class Point : IEquatable<Point>
    {
        public Point(int y, int x)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(Point? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point? left, Point? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Point? left, Point? right)
        {
            return !Equals(left, right);
        }
    }
}