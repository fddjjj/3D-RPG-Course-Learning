using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour
{

    //TextMeshPro levelText;
    // Text levelText;
    TextMeshProUGUI levelText;
    Image healthSlider;
    Image expSlider;
    private void Awake()
    {
        //levelText = transform.GetChild(2).GetComponent<TextMeshPro>;
        levelText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    private void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playerState.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth() 
    {
        float sliderPercent = (float)GameManager.Instance.playerState.currentHealth / GameManager.Instance.playerState.maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerState.characterData.currentExp / GameManager.Instance.playerState.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}
