using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data",menuName = "Character State/DataSo")]
public class CharacterDataSo : ScriptableObject
{
    [Header("Base State")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

}
