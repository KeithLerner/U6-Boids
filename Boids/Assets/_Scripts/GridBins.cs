using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridBins
{
    public List<Boid>[] Bins { get; private set; }
    
    /// <summary>
    /// Number of bins per axis.
    /// </summary>
    public int BinDensity { get; private set; }
    
    public Vector3 BinSize { get; private set; }

    public GridBins(Bounds volume, int binDensity)
    {
        BinDensity = binDensity;
        BinSize = volume.size / BinDensity;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(BinDensity.ToString());

        Bins = new List<Boid>[BinDensity * BinDensity * BinDensity];
        for (int x = 0; x < BinDensity; x++)
        {
            for (int y = 0; y < BinDensity; y++)
            {
                for (int z = 0; z < BinDensity; z++)
                {
                    int i = x + y * BinDensity + z * BinDensity * BinDensity;
                    Bins[i] = new List<Boid>();
                    sb.AppendLine(i.ToString());
                }
            }
        }
        
        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// Convert a world position into a grid bin index coordinate.
    /// </summary>
    /// <param name="volume"> The Bounds of the volume the grid occupies. </param>
    /// <param name="worldPosition"> The query position. </param>
    /// <returns> -Vector3Int.one if invalid input given, otherwise returns a
    /// valid index to a bin in the grid. </returns>
    public Vector3Int WorldPosToBinIndex(Bounds volume, Vector3 worldPosition)
    {
        if (!volume.Contains(worldPosition)) return -Vector3Int.one;

        return new Vector3Int(
            Mathf.FloorToInt(worldPosition.x / BinSize.x),
            Mathf.FloorToInt(worldPosition.y / BinSize.y),
            Mathf.FloorToInt(worldPosition.z / BinSize.z));
    }
    
    /*private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .05f);
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }*/
}
