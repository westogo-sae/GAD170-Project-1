using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : Item
{

    private int cost;
    public TMP_Text costText;


    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        
        //falls at slightly random speeds (looks pretty)
        animator.speed = Random.Range(0.8f, 1.2f);
    }

    public void setCost(int cost)
    {
        this.cost = cost;
        costText.text = "$" + cost;
    }

    public override void AddAttack()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            base.AddAttack();
        }
        else
            DialogueManager.Instance.ModifyText("You can't afford that");
    }
    public override void AddHealth()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            base.AddHealth();
        }
        else
            DialogueManager.Instance.ModifyText("You can't afford that");
    }
    public override void AddXP()
    {
        //only buy item if the player can afford it
        if (gameManager.CanPlayerAfford(cost))
        {
            //increase gold by negatiive of cost to decrease it
            gameManager.increaseGold(-cost);
            base.AddXP();
        }
        else
            DialogueManager.Instance.ModifyText("You can't afford that");
    }

}
