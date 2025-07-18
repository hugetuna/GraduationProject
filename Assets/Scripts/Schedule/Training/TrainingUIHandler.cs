using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainingUIHandler : MonoBehaviour
{
    public GameObject trainingUI;
    public GameObject trainingUIInstance = null; // 記錄被生成的訓練 UI

    public TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    private List<PlayerControlMainWorld> teamMembers = new(); // 記錄取得的隊伍成員
    private Image[] characterImages; // 顯示在 UI 上的（角色）圖片
    public Sprite[] characterSprites; // 角色 UI 圖片來源（檔案資料夾）
    private Dictionary<string, Sprite> characterSpriteDict; // 角色名稱與圖片來源的對照表

    public TeamUIData teamUIData; // 將隊伍 UI 資料寫入 ScriptableObject
 
    public TrainingUIData trainingUIData; // 訓練 UI 的資料 ScriptableObject



    void Start()
    {
        if (teamUIData != null)
        {
            teamUIData.Reset(); // 重置 ScriptableObject 的資料
            Debug.Log("TeamUIData 已重置");
        }
        else
        {
            Debug.Log("讀不到 TeamUIData 的資料");
        }

        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 訂閱並監聽與門互動事件
        ScheduleManager.OnChangeDay += EndThisDay; // 訂閱並監聽切換天數事件

        characterSpriteDict = new() { // 建立角色名稱與圖片來源對照表
            { "Kuma", characterSprites[0] },
            { "Karo", characterSprites[1] },
            { "Sirius", characterSprites[2] },
        };
        teamUIData.characterSpriteDict = characterSpriteDict;

    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 檢查滑鼠左鍵是否被按下
        {
            if (!IsCursorClickUIObject() && trainingUIInstance != null) // 點擊非 UI 區域時關閉 UI
            {
                Debug.Log("關閉訓練 UI");
                trainingUIInstance.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
        DoorInteraction.OnDoorInteracted -= ShowTrainingUI; // 取消訂閱與門互動事件
        ScheduleManager.OnChangeDay -= EndThisDay; // 取消訂閱切換天數事件
    }

    private void EndThisDay(){
        Destroy(trainingUIInstance); // 銷毀訓練 UI 實例
        foreach (var character in ScheduleManager.disappearCharacters) 
        {
            character.SetActive(true); // 回復隱藏並停用的隊伍成員
        }
        teamUIData.ResetTeam(); // 重置 ScriptableObject 的資料（不含 characterSpriteDict）
    }

    private void ShowTrainingUI()
    {
        Debug.Log("開啟訓練 UI");
        if (trainingUIInstance == null)
        {
            trainingUIInstance = Instantiate(trainingUI); // 切換天數時再進行銷毀
            //初始化 UI 外觀
        }
        else
        {
            trainingUIInstance.SetActive(true); // 如果 UI 已經存在，則顯示它
        }

        CanvaLocator canvaLocator = trainingUIInstance.GetComponent<CanvaLocator>();

        // 取得 UI 文字的插槽位置
        Text TypeText = canvaLocator.TypeText;
        Text TeacherText = canvaLocator.TeacherText;
        Text VigourText = canvaLocator.VigourText;
        Text BenefitText = canvaLocator.BenefitText;

        TypeText.text = trainingUIData.trainingType; // 設定訓練類型的 UI 文字內容
        if (trainingUIData.isWithTeacher) // 設定老師的 UI 文字內容
        {
            // 暫時寫死為一星的 Amy 老師
            trainingUIData.teacherName = "Amy";
            TeacherText.text = $"老師：{trainingUIData.teacherName} ★1";
            BenefitText.text = $"基本收益：{trainingUIData.withTeacherBenefit}"; // 設定基本收益的 UI 文字內容
        }
        else
        {
            TeacherText.text = "老師：無";
            BenefitText.text = $"基本收益：{trainingUIData.basicBenefit}"; // 設定基本收益的 UI 文字內容
        }
        VigourText.text = $"耗費體力：{trainingUIData.neededVigour}"; // 設定耗費體力的 UI 文字內容

        // 取得 UI 圖片的插槽位置
        characterImages = canvaLocator.characterImages;

        // 根據目前隊伍成員決定 UI 的初始樣貌（但因為 TeamManager 的清單是固定的，所以看起來都一樣）
        teamMembers = teamManager.teamMembers;
        for (int i = 0; i < teamMembers.Count && i < characterImages.Length; i++) // 確保不會超出陣列範圍
        {
            string memberName = teamMembers[i].name; // 取得隊伍成員名稱
            memberName = memberName.Replace("Character_", ""); // 去除前綴（只剩名字）
            if (characterSpriteDict.ContainsKey(memberName)) // 檢查對照表中是否有該名稱
            {
                characterImages[i].sprite = characterSpriteDict[memberName]; // 指派圖片來源
            }

            if(!teamUIData.teamMembers.Contains(memberName)) teamUIData.teamMembers.Add(memberName);
        }
    }

    private bool IsCursorClickUIObject()
    {
        // 根據當前操作，設定滑鼠或觸控位置
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // RaycastAll 會從 eventData 中的滑鼠位置發射一條射線，檢測所有碰撞的 UI 元素
        // 符合條件的 UI 元素會被加到 raycastResults 清單中
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Count > 0;
    }
}
