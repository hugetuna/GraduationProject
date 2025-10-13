using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserType { Friend, Teacher, Capital }

[CreateAssetMenu(fileName = "User", menuName = "Computer/User")]

public class User : ScriptableObject
{
    public string userName; // 用戶名稱
    public Sprite userIcon; // 用戶頭像
    public UserType userType; // 用戶類型
    public TextAsset inkJSONAsset; // 用戶的聊天室紀錄（Ink 文字檔）
    public string id; // 用戶 ID（唯一識別碼）
}
