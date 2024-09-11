namespace XyzTanks.Engine;

public struct Vector2Int : IEquatable<Vector2Int>
{
    public Vector2Int() { }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public bool Equals(Vector2Int other) => other.X == X && other.Y == Y;
    public override bool Equals(object? obj) => obj is Vector2Int vector && vector.Equals(this);

    public static Vector2Int operator +(Vector2Int firstVector, Vector2Int secondVector)
        => new(firstVector.X + secondVector.X, firstVector.Y + secondVector.Y);

    public static Vector2Int operator *(Vector2Int firstVector, Vector2Int secondVector)
       => new(firstVector.X * secondVector.X, firstVector.Y * secondVector.Y);

    public static bool operator ==(Vector2Int firstVector, Vector2Int secondVector)
        => firstVector.Equals(secondVector);

    public static bool operator !=(Vector2Int firstVector, Vector2Int secondVector)
        => !firstVector.Equals(secondVector);

    public static Vector2Int Up => new(0, -1);
    public static Vector2Int Down => new(0, 1);
    public static Vector2Int Right => new(1, 0);
    public static Vector2Int Left => new(-1, 0);
    public static Vector2Int Zero => new(0, 0);

    public override int GetHashCode() => base.GetHashCode();
}
