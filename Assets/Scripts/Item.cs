using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameManager gameManager;
    public int strength;


    public virtual void AddAttack()
    {
        gameManager.IncreaseAttack(strength);
        gameObject.SetActive(false);
        DialogueManager.Instance.ModifyText("Your Attack Increased!!");
    }
    public virtual void AddHealth()
    {
        gameManager.IncreaseHealth(strength);
        gameObject.SetActive(false);
        DialogueManager.Instance.ModifyText("Your Health Increased!!");
    }
    public virtual void AddXP()
    {
        
        gameManager.IncreaseXP(strength);
        gameObject.SetActive(false);
        DialogueManager.Instance.ModifyText("Your XP Increased!!");
    
    }
    public void AddGold()
    {
        gameManager.increaseGold(strength);
        gameObject.SetActive(false);
        DialogueManager.Instance.ModifyText($"You found {strength} gold");
    }
}
