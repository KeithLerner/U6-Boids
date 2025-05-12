using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance { get; private set; }

    [Header("System")] 
    public GameObject boidPrefab;
    public Transform boidParent;
    public int spawnCount = 100;
    public Bounds bounds;
    public int binsPerAxis = 10;
    
    [Header("Boid")]
    public float speed = 8;
    public float neighborRadius = 4;
    public float separationDistance = 1;
    public float weightAlignment = 2;
    public float weightCohesion = 1;
    public float weightSeparation = 1;
    public float weightAvoidEdges = 4;
    
    public GridBins<Boid> GridBins { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) 
            throw new Exception("Multiple instances found in singleton pattern.");

        if (boidPrefab == null)
            throw new NullReferenceException("No boid prefab provided");

        GridBins = new GridBins<Boid>(bounds, binsPerAxis);
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));

            GameObject go = Instantiate(boidPrefab, pos, Quaternion.identity,
                boidParent);
            Boid boid = go.GetComponent<Boid>();
            boid.Init(this);

            Debug.DrawLine(pos, pos + Vector3.up, Color.green);
            Vector3Int binIndex = GridBins.WorldPosToBinIndex(pos);
            int arrayIndex = GridBins.BinIndexToArrayIndex(binIndex);
            GridBins.Bins[arrayIndex].Add(boid);
        }
    }

    public List<Boid> GetNeighbors(Boid boid, float radius)
    {
        List<Boid> results = new();

        if (GridBins == null)
            throw new NullReferenceException("GridBins not initialized");

        if (GridBins.Bins.Length == 0) throw new Exception("No boids found");

        Vector3 pos = boid.transform.position;
        Vector3Int binIndex = GridBins.WorldPosToBinIndex(pos);
        List<Vector3Int> searchBins = new List<Vector3Int>{ binIndex };
        searchBins.AddRange(GridBins.GetNeighborBinIndices(binIndex));

        foreach (Vector3Int bI in searchBins)
        {
            int arrayIndex = GridBins.BinIndexToArrayIndex(bI);
            foreach (var b in GridBins.Bins[arrayIndex])
            {
                if (b == boid) continue;

                if (Vector3.Distance(b.transform.position,
                        boid.transform.position) < radius)
                {
                    results.Add(b);
                }
            }
        }

        return results;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .05f);
        Gizmos.DrawCube(bounds.center, bounds.size);
        
        GridBins?.OnDrawGizmos();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        GridBins?.OnDrawGizmosSelected();
    }
}
