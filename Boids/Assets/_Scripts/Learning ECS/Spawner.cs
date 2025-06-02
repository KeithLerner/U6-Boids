using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnFrequency;
}
