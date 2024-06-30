using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;
    Image healthSlider;
    private Transform UIbar;
    private Transform mainCamera;
    ChararcterState currentState;

    private void Awake()
    {
        currentState = GetComponent<ChararcterState>();
        currentState.UpdateHealthBarOnAttack += UpdateHealthBar;
    }
    private void OnEnable()
    {
        mainCamera = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar =  Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);

            }
        }
    }
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if(currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

    }
    private void LateUpdate()
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -mainCamera.forward;
            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
