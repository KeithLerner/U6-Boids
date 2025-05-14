using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    private BoidManager _manager;
    private int _owningGridArrayIndex;
    public Vector3 Velocity { get; private set; }
    private GridBins<Boid> _bins;
    
    // Start is called before the first frame update
    void Start()
    {
        Velocity = Random.insideUnitSphere * _manager.speed;
    }

    public void Init(BoidManager bm)
    {
        _manager = bm;
        _bins = _manager.GridBins;
    }

    private void Update()
    {
        if (_manager == null)
            throw new NullReferenceException("Boid Manager instance not found");
        
        FixToBounds();
        
        UpdateOwningGridBin();

        Movement();
    }

    private void LateUpdate()
    {
        transform.forward =
            Vector3.Slerp(transform.forward, Velocity.normalized, .5f);
    }

    private void Movement()
    {
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

    private void FixToBounds()
    {
        Bounds bounds = _manager.bounds;
        if (bounds.Contains(transform.position)) return;
        
        Vector3 newPos = transform.position;
        if (transform.position.x >= bounds.max.x)
            newPos.x = bounds.min.x + 1;
        if (transform.position.y >= bounds.max.y)
            newPos.y = bounds.min.y + 1;
        if (transform.position.z >= bounds.max.z)
            newPos.z = bounds.min.z + 1;
                
        if (transform.position.x <= bounds.min.x)
            newPos.x = bounds.max.x - 1;
        if (transform.position.y <= bounds.min.y)
            newPos.y = bounds.max.y - 1;
        if (transform.position.z <= bounds.min.z)
            newPos.z = bounds.max.z - 1;

        transform.position = newPos;
    }

    private void UpdateOwningGridBin()
    {
        Vector3Int gridBinIndex =
            _bins.WorldPosToBinIndex(transform.position);
        int i = _bins.BinIndexToArrayIndex(gridBinIndex);
        
        if (i == _owningGridArrayIndex) return;

        _bins.Bins[_owningGridArrayIndex].Remove(this);
        _bins.Bins[i].Add(this);
        _owningGridArrayIndex = i;
    }
}
