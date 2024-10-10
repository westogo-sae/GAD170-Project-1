using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public GameManager gameManager;

    private int cost;
    public TMP_Text costText;
    public int strength;

    public void setCost(int cost)
    {
        this.cost = cost;
        costText.text = "$" + cost;
    }

    public void buyAttack()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            gameManager.IncreaseAttack(strength);
            gameObject.SetActive(false);
            DialogueManager.Instance.ModifyText("Your Attack Increased!!");
        }
    }
    public void buyHealth()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            gameManager.IncreaseHealth(strength);
            gameObject.SetActive(false);
            DialogueManager.Instance.ModifyText("Your Health Increased!!");
        }
    }
    public void buyXP()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            gameManager.IncreaseXP(strength);
            gameObject.SetActive(false);
            DialogueManager.Instance.ModifyText("Your XP Increased!!");
        }
    }
}
