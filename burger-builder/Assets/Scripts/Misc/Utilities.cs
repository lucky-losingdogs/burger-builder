using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utilities
{
    //return the scriptable objects in the folder path as a list
    public static List<T> LoadList<T>(string folderPath) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(folderPath).ToList();
    }
    
    //return the scriptable objects in the folder path as an ordered list
    public static List<T> LoadList<T>(string folderPath,Func<T, int> orderBy) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(folderPath).OrderBy(orderBy).ToList();
    }

    //return the scriptable objects in the folder path as an array
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

    //remove the rest of the list after the trim root
    public static List<T> TrimExcess<T>(List<T> list, int trimRoot) where T : UnityEngine.Object
    {
        if (list.Count <= trimRoot)
            return list;

        for (int i = list.Count - 1; i >= trimRoot; i--)
        {
            list.RemoveAt(i);
        }

        return list;
    }

    //extend a list to the desired capacity
    public static List<T> ExtendList<T>(List<T> list, int capacity) where T : UnityEngine.Object
    {
        if (list.Count < 0)
            return list;
        
        List<T> tempList = new List<T>();

        int j = 0;
        for (int i = 0; i < capacity; i++)
        {
            tempList.Add(list[j]);

            if (j < list.Count - 1)
                j++;
            else
                j = 0;
        }

        return tempList;
    }

    // Source - https://stackoverflow.com/a/1262619
    // Posted by grenade, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-06-21, License - CC BY-SA 4.0

    private static System.Random rng = new System.Random();

    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }  
    
    //<summary>
    // rotates a vector3 by 90 * the rotation value
    // e.g: 2 is 180 degree rotation
    // </summary>
    public static Vector3Int RotateVector3(Vector3Int vect, int rotation)
    {
        return rotation switch
        {
            0 => vect,
            1 => new Vector3Int(vect.y, -vect.x, vect.z),
            2 => new Vector3Int(-vect.x, -vect.y, vect.z),
            3 => new Vector3Int(-vect.y, vect.x, vect.z),
            _ => vect
        };
    }

    public static Vector2 Add(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x + b.x, a.y + b.y);
    }
}
