using System.Collections.Generic;


/* 用來儲存預約老師的資訊（與 GameManager 連接以跨場景保存資料） */
[System.Serializable]
public class TeacherSaveData
{
    public List<TeacherInfo> trainingTeachers = new(); // 記錄已預約的老師用戶名稱＆性質

    public bool IsWithTeacherToday(TrainingType trainingType) // 記錄今天是否有老師協助訓練（方便存取用）
    {
        var teacher = trainingTeachers.Find(t => t.trainingType == trainingType );
        return teacher != null;
    }

    public void CleanTeacherAppointments()
    {
        var newList = new List<TeacherInfo>();
        foreach (var teacher in trainingTeachers)
        {
            int totalDays = DayManager.Instance.date + DayManager.Instance.chapter * 3; // 僅適用於新手教學＆第一章
            bool isExpired = teacher.day < totalDays;
            if (!teacher.hasCameToLesson && !isExpired) // 保留尚未完成課程＆尚未過期的老師們
            {
                newList.Add(teacher);
            }
        }
        trainingTeachers = newList;
    }

    public string GetTeacherNameByType(TrainingType trainingType)
    {
        var teacher = trainingTeachers.Find(t => t.trainingType == trainingType);
        return teacher != null ? teacher.teacherName : "無";
    }

    public void SetTeacherLessonCompleted(TrainingType trainingType)
    {
        var teacher = trainingTeachers.Find(t => t.trainingType == trainingType);
        if (teacher != null)
        {
            teacher.hasCameToLesson = true;
        }
    }
}

[System.Serializable]
public class TeacherInfo
{
    public TrainingType trainingType;
    public string teacherName;
    public bool hasCameToLesson; // 預設為 false，可用於記錄當天的課程是否完成
    public int day; // 老師要來的日子（預約隔天）

    public TeacherInfo(string teacherName, TrainingType trainingType, bool hasCameToLesson = false)
    {
        this.teacherName = teacherName;
        this.trainingType = trainingType;
        this.hasCameToLesson = hasCameToLesson;
        day = DayManager.Instance.date + DayManager.Instance.chapter * 3 + 1; // 僅適用於新手教學＆第一章
    }
}
