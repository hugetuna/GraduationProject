using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserType { Friend, Teacher, Capital }

[CreateAssetMenu(fileName = "User", menuName = "Computer/User")]

public class User : ScriptableObject
{
    public string userName;
    public Sprite userIcon;
    public UserType userType;
    public string id;
}
