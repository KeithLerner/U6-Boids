using UnityEngine;

public static class SimulationSettings
{
    public static readonly Bounds Bounds = new Bounds()
        { center = Vector3.zero, size = Vector3.one * 256 };

    public static readonly int SpawnCount = 2500;

    public static readonly float BoidSpeed = 6;
}
