using Unity.Entities;
using UnityEngine;

public class BoidSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
}

class BoidSpawnerBaker : Baker<BoidSpawnerAuthoring>
{
    public override void Bake(BoidSpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new BoidSpawner
        {
            Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
        });
    }
}
