using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInitialSettings", menuName = "Game/Initial Settings")]
public class GameSettingsSO : ScriptableObject
{
    [Header("永久儲存初始設定資料")]
    public List<AnimalFarmSaveData> FarmsDataList;
    public List<AnimalSaveData> animalDataList = new List<AnimalSaveData>();
    public int teamIndex = (int)IdolTeamIndex.None;
    public List<IdolSaveData> idolDataList = new List<IdolSaveData>();
    public DaySaveData DayData;
    public string sceneNameSave = "";
    public ResourceSaveData ResourceData;
    public ChatSaveData chatSaveData = new();
    public TeacherSaveData teacherSaveData = new();
    public ProductSaveData productSaveData = new();
    public ActivitySaveData activitySaveData = new();
}
