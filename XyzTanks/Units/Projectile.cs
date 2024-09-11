using XyzTanks.Engine;

namespace XyzTanks.Units;

public class Projectile : IUpdateable
{
    public Transform Transform { get; } = new Transform();

    public void Update(double deltaSeconds)
    {
        
    }
}