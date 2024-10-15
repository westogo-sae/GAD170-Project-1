using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public Slider healthSlider;
    public Slider XPSlider;
    public TMP_Text levelText;
    public TMP_Text goldText;
    public TMP_Text attackText;
    public GameObject[] shieldIcons;

    public void Updatehealth(int value)
    {
        healthSlider.value = value;
    }

    public void UpdateXP(int value, int threashold)
    {
        XPSlider.maxValue = threashold;
        XPSlider.value = value;
    }

    public void UpdateLevel(int value)
    {
        levelText.text = "Lvl" + value;
    }

    public void UpdateGold(int value)
    {
        goldText.text = "$" + value;
    }

    public void UpdateAttack(int value)
    {
        attackText.text = value.ToString();
    }

    /// <summary>
    /// update shield icons, MAX 3
    /// </summary>
    /// <param name="value"></param>
    public void UpdateShield(int value)
    {
        for(int i = 0; i<shieldIcons.Length; i++)
        {
            if(shieldIcons[i] != null)
            {
                if(value > i)
                {
                    shieldIcons[i].SetActive(true);
                }
                else
                    shieldIcons[i].SetActive(false);
            }
        }
    }

}

