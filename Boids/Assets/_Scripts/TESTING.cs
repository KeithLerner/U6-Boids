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

    [MenuItem("Testing/Test Bins Indexing")]
    public static void DIMENSION_CONVERSION_TEST()
    {
        int d = 10;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);
        GridBins bins = new GridBins(bounds, d);
        StringBuilder sb = new StringBuilder("TEST RESULTS:\n");
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
}
