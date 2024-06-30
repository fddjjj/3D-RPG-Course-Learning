using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChararcterState : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterDataSo templateData;
    public CharacterDataSo characterData;
    public AttackDataSo attackDataSo;

    [HideInInspector]
    public bool isCritical;
    #region Read Data From CharacterDataSo
    public int maxHealth 
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }
    public int currentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }
    public int baseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int currentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion
    #region character attack damage calculate
    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }
    public void TakeDamage(ChararcterState attacker,ChararcterState defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.currentDefence,0);
        currentHealth = Mathf.Max(currentHealth - damage,0);
        if(attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("hit");
        }
        //FIXME:
        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);

    }
    public void TakeDamage(int damage,ChararcterState defener)
    {
        int currentDamage = Mathf.Max(damage - defener.currentDefence,0);
        currentHealth = Mathf .Max(currentHealth - currentDamage,0);
        UpdateHealthBarOnAttack?.Invoke(currentHealth, maxHealth);
    }
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackDataSo.minDamage, attackDataSo.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackDataSo.criticalMultiplier;
        }
        return (int)coreDamage;
    }
    #endregion
}
