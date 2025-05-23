using System.Collections.Generic;
using UnityEngine;

public class GridBins<T>
{
    public List<T>[] Bins { get; private set; }

    /// <summary>
    /// Number of bins per axis.
    /// </summary>
    public readonly int BinDensity;

    public readonly Vector3 BinSize;

    private readonly Bounds _bounds;

    public GridBins(Bounds volume, int binDensity)
    {
        BinDensity = binDensity;
        _bounds = volume;
        
        BinSize = _bounds.size / BinDensity;

        Bins = new List<T>[BinDensity * BinDensity * BinDensity];
        for (int x = 0; x < BinDensity; x++)
        {
            for (int y = 0; y < BinDensity; y++)
            {
                for (int z = 0; z < BinDensity; z++)
                {
                    int i = x + y * BinDensity + z * BinDensity * BinDensity;
                    Bins[i] = new List<T>();
                }
            }
        }
    }

    /// <summary>
    /// Convert a world position into a grid bin index coordinate.
    /// </summary>
    /// <param name="worldPosition"> The query position. </param>
    /// <returns> -Vector3Int.one if invalid input given, otherwise returns a
    /// valid index to a bin in the grid. </returns>
    public Vector3Int WorldPosToBinIndex(Vector3 worldPosition)
    {
        if (!_bounds.Contains(worldPosition)) return -Vector3Int.one;

        return new Vector3Int(
            Mathf.FloorToInt(.5f * BinDensity + worldPosition.x / BinSize.x),
            Mathf.FloorToInt(.5f * BinDensity + worldPosition.y / BinSize.y),
            Mathf.FloorToInt(.5f * BinDensity + worldPosition.z / BinSize.z));
    }

    public int BinIndexToArrayIndex(Vector3Int index)
    {
        return index.x + 
               index.y * BinDensity +
               index.z * BinDensity * BinDensity;
    }
    
    public int BinIndexToArrayIndex(int x, int y, int z)
    {
        return x + 
               y * BinDensity +
               z * BinDensity * BinDensity;
    }

    public Vector3Int ArrayIndexToBinIndex(int index)
    {
        int x = index % BinDensity;
        int y = index / BinDensity % BinDensity;
        int z = index / (BinDensity * BinDensity);

        return new Vector3Int(x, y, z);
    }

    public List<Vector3Int> GetNeighborBinIndices(Vector3Int index)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        
        for (int x = index.x - 1; x <= index.x + 1; x++)
        {
            for (int y = index.y - 1; y <= index.y + 1; y++)
            {
                for (int z = index.z - 1; z <= index.z + 1; z++)
                {
                    // Check that modified index exists within grid space before
                    // adding to results
                    if (index.x < BinDensity - 2 && index.x > 0 &&
                        index.y < BinDensity - 2 && index.y > 0 &&
                        index.z < BinDensity - 2 && index.z > 0)
                        results.Add(new Vector3Int(x, y, z));
                }
            }
        }

        return results;
    }

    public void DebugBinOccupancy()
    {
        string results = "\n";
        int occupied = 0;
        for (var i = 0; i < Bins.Length; i++)
        {
            int c = Bins[i].Count;
            bool o = c > 0;
            results += $"{ArrayIndexToBinIndex(i)}: {c}";
            if (o) occupied++;
        }
        Debug.Log($"<b>{occupied} Occupied</b>:{results}");
    }
    
    public void OnDrawGizmos()
    {
        for (int x = 0; x < BinDensity; x++)
        {
            for (int y = 0; y < BinDensity; y++)
            {
                for (int z = 0; z < BinDensity; z++)
                {
                    float fill = 
                        Bins[BinIndexToArrayIndex(x, y, z)].Count / 10f;
                    Gizmos.color = new Color(1, 1, 1, fill + .001f);
                    
                    Vector3 center = _bounds.min +
                                     new Vector3(x * BinSize.x, y * BinSize.y,
                                         z * BinSize.z) + BinSize / 2;
                    
                    Gizmos.DrawCube(center, BinSize);
                }
            }
        }
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        
        for (int x = 0; x < BinDensity; x++)
        {
            for (int y = 0; y < BinDensity; y++)
            {
                for (int z = 0; z < BinDensity; z++)
                {
                    Vector3 center = _bounds.min +
                                     new Vector3(x * BinSize.x, y * BinSize.y,
                                         z * BinSize.z) + BinSize / 2;
                    
                    Gizmos.DrawWireCube(center, BinSize);
                }
            }
        }
    }
}
