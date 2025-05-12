using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    private BoidManager _manager;
    public Vector3 Velocity { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        Velocity = Random.insideUnitSphere * _manager.speed;
    }

    public void Init(BoidManager bm)
    {
        _manager = bm;
    }

    private void Update()
    {
        if (_manager == null)
            throw new NullReferenceException("Boid Manager instance not found");
        
        FixToBounds();
        
        List<Boid> neighbors = _manager.GetNeighbors(this, _manager.neighborRadius);
        
        Vector3 alignment  = Vector3.zero;
        Vector3 cohesion   = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 seekCenter = Vector3.zero;

        int c = 0;
        foreach (Boid boid in neighbors)
        {
            if (boid == this) continue;

            var bPos = boid.transform.position;
            Vector3 toBoid = bPos - transform.position;
            
            alignment += boid.Velocity;
            cohesion  += bPos;
            if (toBoid.magnitude < _manager.separationDistance)
                separation -= toBoid.normalized;

            c++;
        }

        float dist =
            Vector3.Distance(transform.position, _manager.bounds.center);
        if (dist > _manager.bounds.extents.magnitude / 2)
            seekCenter +=
                (_manager.bounds.center - transform.position).normalized *
                _manager.speed;
        
        if (c > 0)
        {
            alignment  = (alignment / c).normalized * _manager.speed;
            cohesion   = (cohesion  / c).normalized * _manager.speed;
            separation = separation.normalized      * _manager.speed;
        }
        
        Velocity += (alignment  * _manager.weightAlignment  +
                     cohesion   * _manager.weightCohesion   +
                     separation * _manager.weightSeparation +
                     seekCenter * _manager.weightAvoidEdges) * Time.deltaTime;
        Velocity = Velocity.normalized * _manager.speed;

        transform.position += Velocity * Time.deltaTime;
    }

    private void LateUpdate()
    {
        transform.forward =
            Vector3.Slerp(transform.forward, Velocity.normalized, .5f);
    }

    private void FixToBounds()
    {
        Bounds bounds = _manager.bounds;
        if (bounds.Contains(transform.position)) return;
        
        Vector3 newPos = transform.position;
        if (transform.position.x > bounds.max.x)
            newPos.x = bounds.min.x;
        if (transform.position.y > bounds.max.y)
            newPos.y = bounds.min.y;
        if (transform.position.z > bounds.max.z)
            newPos.z = bounds.min.z;
                
        if (transform.position.x < bounds.min.x)
            newPos.x = bounds.max.x;
        if (transform.position.y < bounds.min.y)
            newPos.y = bounds.max.y;
        if (transform.position.z < bounds.min.z)
            newPos.z = bounds.max.z;

        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        GridBins bins = _manager.GridBins;
        Vector3Int gridBinIndex =
            bins.WorldPosToBinIndex(_manager.bounds, transform.position);

        Vector3 binSize = bins.BinSize;
        Vector3 center = new Vector3(gridBinIndex.x * binSize.x,
                             gridBinIndex.y * binSize.y,
                             gridBinIndex.z * binSize.z) +
                         bins.BinSize / 2;
        
        Gizmos.color = new Color(1, 0, 0, .25f);
        Gizmos.DrawCube(center, binSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, binSize);
    }
}
