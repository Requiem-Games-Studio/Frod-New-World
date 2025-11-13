using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public float playTime;
    public float playProgress;
    public Vector2 playerPosition;
    public List<PowerType> unlockedPowers = new List<PowerType>();
    public int health;
    public int coins;
}
