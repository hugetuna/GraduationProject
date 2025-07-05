using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamUIData", menuName = "Schedule/TeamUIData")]
public class TeamUIData : ScriptableObject
{
    public List<string> teamMembers = new(); // 儲存當前隊伍成員（在 TrainingUIHandler 初始化）
    public List<string> teamTrainees = new(); // 儲存當前訓練成員（在 DragToLesson 初始化）
    public Dictionary<string, Sprite> characterSpriteDict = new(); // 角色名稱與圖片來源的對照表（在 TrainingUIHandler 初始化）

    public void Reset()
    {
        teamMembers.Clear();
        teamTrainees.Clear();
        characterSpriteDict.Clear();
    }
    public void ResetTeam()
    {
        teamMembers.Clear();
        teamTrainees.Clear();
    }

}