using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Utilities
{
    public static List<T> LoadList<T>(string folderPath) where T : UnityEngine.Object
    {
        T[] objectsArr = Resources.LoadAll<T>(folderPath);
        return new List<T>(objectsArr);
    }

    public static T[] LoadArray<T>(string folderPath) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(folderPath);
    }

    public static Vector3Int RoundToInt(Vector3 vect3)
    {
        return new Vector3Int (Mathf.RoundToInt(vect3.x),
                            Mathf.RoundToInt(vect3.y),
                            Mathf.RoundToInt(vect3.z));
    }
}
