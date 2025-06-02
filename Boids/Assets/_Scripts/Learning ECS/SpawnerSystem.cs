using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Random = UnityEngine.Random;

/*
public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }
    
    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
        {
            ProcessSpawner(ref state, spawner);
        }
    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner> spawner)
    {
        var spawnerRO = spawner.ValueRO;
        
        if (spawnerRO.NextSpawnTime > SystemAPI.Time.ElapsedTime)
            return;

        Entity newEntity =
            state.EntityManager.Instantiate(spawnerRO.Prefab);

        state.EntityManager.SetComponentData(newEntity,
            LocalTransform.FromPosition(spawnerRO.SpawnPosition +
                                        (float3)Random.insideUnitSphere * 2));

        spawner.ValueRW.NextSpawnTime =
            (float)SystemAPI.Time.ElapsedTime + spawnerRO.SpawnFrequency;
    }
}
*/
