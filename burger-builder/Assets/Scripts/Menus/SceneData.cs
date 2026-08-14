using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneData", menuName = "Scriptable Objects/SceneData")]
public class SceneData : ScriptableObject
{
    public string sceneName;

    //get the scene index from the scene name
    //so its less fragile than using strings but more explicit about what index is what scene
    public int GetIndex() => SceneUtility.GetBuildIndexByScenePath(sceneName);

#if UNITY_EDITOR
    //get the scene name from the name of the SO asset
    private void OnValidate()
    {
        sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
    }
#endif
}