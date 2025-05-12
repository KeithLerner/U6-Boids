using System.Text;
using UnityEditor;
using UnityEngine;

public static class TESTING
{
    private static int i = 0;

    //[MenuItem("Testing/Reset Testing Index (to 0)")]
    public static void RESET_TESTING_INDEX() => i = 0;
    
    //[MenuItem("Testing/Test Bins Indexing")]
    public static void INDEXING_TEST()
    {
        //GridBins bins = new GridBins(++i);
    }

    [MenuItem("Testing/Test Index Conversion")]
    public static void DIMENSION_CONVERSION_TEST()
    {
        int d = 10;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);
        GridBins<Boid> bins = new GridBins<Boid>(bounds, d);
        StringBuilder sb = new StringBuilder("TEST RESULTS:\n");

        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
            
            
        }
        
        for (int x = 0; x < d; x++)
        {
            for (int y = 0; y < d; y++)
            {
                for (int z = 0; z < d; z++)
                {
                    Vector3Int _3dIndex = new Vector3Int(x, y, z);
                    int _1dIndex = bins.BinIndexToArrayIndex(_3dIndex);
                    Vector3Int results = bins.ArrayIndexToBinIndex(_1dIndex);

                    sb.AppendLine(
                        $"<b>{results == _3dIndex}</b> : {_3dIndex} -> {_1dIndex} -> {results}");
                }
            }
        }
        Debug.Log(sb.ToString());
    } 
    
    [MenuItem("Testing/Test Indexing")]
    public static void GRID_INDEXING_TEST()
    {
        int d = 10;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);
        GridBins<Boid> bins = new GridBins<Boid>(bounds, d);
        StringBuilder sb = new StringBuilder("TEST RESULTS:\n");
        int pass = 0, fail = 0;
        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
            
            Vector3Int _3dIndex = bins.WorldPosToBinIndex(pos);
            bool valid = _3dIndex is { x: >= 0, y: >= 0, z: >= 0 } &&
                         _3dIndex.x < d && _3dIndex.y < d && _3dIndex.z < d;
            if (valid) pass++; else fail++;
            
            sb.AppendLine($"<b>{valid}</b>: {pos} -> {_3dIndex}");
        }
        
        Debug.Log($"<b>{pass} PASS\n{fail} FAIL</b>\n" + sb);
    } 
}
