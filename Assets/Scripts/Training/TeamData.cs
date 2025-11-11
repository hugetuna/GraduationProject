using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalTeamData
{
    private static IdolInstance[] idolInstances;

    public static IdolInstance[] IdolInstances
    {
        get
        {
            // 如果陣列不存在或裡面物件被刪掉，就刷新
            if (idolInstances == null || idolInstances.Length == 0)
            {
                Refresh();
            }
            return idolInstances;
        }
    }

    public static void Refresh()
    {
        idolInstances = Object.FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
    }
}

[CreateAssetMenu(fileName = "TeamData", menuName = "Training/TeamData")]
public class TeamData : ScriptableObject
{
    private List<string> members = new(); // 儲存當前隊伍成員
    private List<string> trainees = new(); // 儲存當前訓練成員
    //-----------------------------------------------------------------//
    private Dictionary<string, Sprite> characterSpriteDict = new(); // 角色名稱與圖片來源的對照表

    public void Initialize(TeamManager teamManager, List<Sprite> characterSprites)
    {
        // 初始化當前隊伍成員名稱
        List<PlayerControlMainWorld> teamMembers = teamManager.teamMembers;
        foreach (var member in teamMembers)
        {
            string memberName = member.name; // 取得隊伍成員名稱
            // 去除前後多餘的字元（只剩名字）
            memberName = memberName.Replace("Character_", "");
            memberName = memberName.Replace("2.0", "");

            if (!members.Contains(memberName)) members.Add(memberName);
        }

        // 初始化圖片對照表
        foreach (var sprite in characterSprites)
        {
            string spriteName = sprite.name.Replace("UI_character_", ""); // 取得圖片名稱
            if (!characterSpriteDict.ContainsKey(spriteName) && members.Contains(spriteName))
            {
                // 該角色圖片必須在隊伍成員中才能加入對照表
                characterSpriteDict.Add(spriteName, sprite);
            }
        }
    }

    public List<string> GetMembers()
    {
        return members;
    }

    public List<string> GetTrainees()
    {
        return trainees;
    }

    // public void AddTrainee(List<PlayerControlMainWorld> teamTrainees)
    // {
    //     foreach (var trainee in teamTrainees)
    //     {
    //         string traineeName = trainee.name; // 取得訓練成員名稱
    //         // 去除前後多餘的字元（只剩名字）
    //         traineeName = traineeName.Replace("Character_", "");
    //         traineeName = traineeName.Replace("2.0", "");

    //         if (!trainees.Contains(traineeName)) trainees.Add(traineeName);
    //     }
    // }

    // public void RemoveTrainee(List<PlayerControlMainWorld> teamTrainees)
    // {
    //     foreach (var trainee in teamTrainees)
    //     {
    //         string traineeName = trainee.name; // 取得訓練成員名稱
    //         // 去除前後多餘的字元（只剩名字）
    //         traineeName = traineeName.Replace("Character_", "");
    //         traineeName = traineeName.Replace("2.0", "");

    //         if (trainees.Contains(traineeName)) trainees.Remove(traineeName);
    //     }
    // }

    public List<Sprite> GetAllCharacterSprites()
    {
        List<Sprite> sprites = new();
        sprites.AddRange(characterSpriteDict.Values);

        return sprites;
    }

    public void Reset() // 恢復預設值
    {
        members.Clear();
        trainees.Clear();
    }

}