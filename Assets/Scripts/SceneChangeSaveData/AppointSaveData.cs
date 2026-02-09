using System.Collections.Generic;


/* 用來儲存預約相關資訊，如老師或商演等（與 GameManager 連接以跨場景保存資料） */
[System.Serializable]
public class AppointSaveData
{
    public List<TeacherInfo> trainingTeachers = new(); // 記錄已預約的老師用戶名稱＆性質

    // 部分預約資料可能需要每天更新，之後再寫就好
}

[System.Serializable]
public class TeacherInfo
{
    public TrainingType trainingType;
    public string teacherName;
}
