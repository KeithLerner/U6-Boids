using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
public partial struct BoidSpawnerSystem : ISystem
{
    private Bounds _bounds;
    private int _count;
    private int _spawned;

    private NativeArray<float3> _randomPositions;
    private NativeArray<float3> _randomVelocities;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("SYSTEM CREATED");

        _bounds = SimulationSettings.Bounds;
        _count = SimulationSettings.SpawnCount;
        _spawned = 0;
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        if (_spawned >= _count) return;
        
        // Calculate spawn position
        float3 pos = new float3(
            Random.Range(_bounds.min.x * .9f, _bounds.max.x * .9f),
            Random.Range(_bounds.min.y * .9f, _bounds.max.y * .9f),
            Random.Range(_bounds.min.z * .9f, _bounds.max.z * .9f));
        
        // Calculate and set spawn velocity
        float3 vel = (float3)Random.insideUnitSphere *
                     SimulationSettings.BoidSpeed;

        new ProcessBoidSpawnerJob
        {
            Ecb = GetEntityCommandBuffer(ref state),
            Position = pos,
            Velocity = vel
        }.ScheduleParallel();

        _spawned++;
    }
    
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(
        ref SystemState state)
    {
        var ecbSingleton = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}

[BurstCompile]
public partial struct ProcessBoidSpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float3 Position;
    public float3 Velocity;

    // IJobEntity generates a component data query based on the parameters of
    // its `Execute` method. This example queries for all Spawner components
    // and uses `ref` to specify that the operation requires read and write
    // access. Unity processes `Execute` for each entity that matches the
    // component data query.
    private void Execute([ChunkIndexInQuery] int chunkIndex, ref BoidSpawner boidSpawner)
    {
        // Create new entity
        Entity newEntity = Ecb.Instantiate(chunkIndex, boidSpawner.Prefab);
        
        // Set spawn position value
        Ecb.AddComponent(chunkIndex, newEntity,
            new Position { Value = Position });
            
        // Set spawn velocity value 
        Ecb.AddComponent(chunkIndex, newEntity,
            new Velocity { Value = Velocity });
            
        // Set world position to spawn position
        Ecb.SetComponent(chunkIndex, newEntity,
            LocalTransform.FromPositionRotation(Position,
                quaternion.Euler(Velocity)));
    }
}