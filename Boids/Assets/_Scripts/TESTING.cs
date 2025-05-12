using UnityEditor;
using UnityEngine;

public class TESTING : MonoBehaviour
{
    private static int i = 0;

    //[MenuItem("Testing/Reset Testing Index (to 0)")]
    public static void RESET_TESTING_INDEX() => i = 0;
    
    //[MenuItem("Testing/Test Bins Indexing")]
    public static void INDEXING_TEST()
    {
        //GridBins bins = new GridBins(++i);
    }
}
