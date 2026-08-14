using UnityEngine;

public class LevelSelectInScene : LevelSelect
{
    //change the level while in the main scene
    //no inter-scene persistence required
    protected override void OnButtonClick(int index)
    {
        Debug.Log("Switch to level: " + index);
        GameManager.s_instance.SetSelectedLevel(index);
    }
}
