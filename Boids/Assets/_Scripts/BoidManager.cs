using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance { get; private set; }
    
    public enum Mode { V0, GridBins }

    [Header("System")] 
    //public Mode mode;
    public GameObject boidPrefab;
    public int count;
    public Bounds bounds;
    
    [Header("Boid")]
    public float speed = 5;
    public float neighborRadius = 3;
    public float separationDistance = 1;
    public float weightAlignment = 1;
    public float weightCohesion = 1;
    public float weightSeparation = 1;
    public float weightAvoidEdges = 1;
    
    // Normal Boids
    private List<Boid> _boids;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null) 
            throw new Exception("Multiple instances found in singleton pattern.");

        if (boidPrefab == null)
            throw new NullReferenceException("No boid prefab provided");

        _boids = new List<Boid>();
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));

            GameObject go = Instantiate(boidPrefab, pos, Quaternion.identity,
                transform);
            Boid boid = go.GetComponent<Boid>();
            boid.Init(this);
            _boids.Add(boid);
        }
    }

    public List<Boid> GetNeighbors(Boid boid, float radius)
    {
        List<Boid> results = new();

        if (_boids == null)
            throw new NullReferenceException("Boids list not initialized");
        
        if (_boids.Count == 0) throw new Exception("No boids found");
        
        foreach (var b in _boids)
        {
            if (b == boid) continue;

            if (Vector3.Distance(b.transform.position,
                    boid.transform.position) < radius)
            {
                results.Add(b);
            }
        }

        return results;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .05f);
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
