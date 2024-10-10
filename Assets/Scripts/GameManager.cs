using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public ShopItem[] shopItems;


    public DialogueStream defaultDialogueStream;

    public bool gameOver;

    int playerLevel;
    int playerXP;
    int levelUpThreashold;
    int playerAttack;
    int playerHealth;
    int playerGold;


    enum RoomType
    {
        SHOP,
        CHEST,
        ENEMY
    }

    RoomType currentRoom;


    private void Start()
    {
        DialogueManager.Instance.PlayStream(defaultDialogueStream);
    }

    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        //initialize player stats and level
        playerLevel = 0;
        playerXP = 0;
        levelUpThreashold = 15;
        playerHealth = 10;
        playerAttack = 5;
        playerGold = 0;
        

        while(!gameOver)
        {

            //bonus: show dialogue to let the player know the they have moved on
            DialogueManager.Instance.PlaySingle(new Dialogue(text: "You continue to the next room"));
            yield return DialogueManager.Instance.WaitUntilDialogueContinue();

            //select a random room
            currentRoom = (RoomType)Random.Range(0, 3);

            if(currentRoom == RoomType.SHOP)
            {
                //display shop items with new random costs
                foreach(ShopItem item in shopItems)
                {
                    item.setCost(Random.Range(5,30));
                    item.gameObject.SetActive(true);
                }

                //shop dialogue
                string dialogueText = "You encounter three items in the room a rugged sign reads \"DO NOT STEAL\" ... better safe than sorry";
                //create dialogue with a continue option
                Dialogue dialogue = new Dialogue(text: dialogueText, options: new Dialogue.DialogueOption("Continue", ContinueDialogue));
                //play dialogue
                DialogueManager.Instance.PlaySingle(dialogue);

                //wait until the player continues
                yield return DialogueManager.Instance.WaitUntilDialogueContinue();

            }

            if(currentRoom == RoomType.CHEST)
            {
                //pass
            }

            if (currentRoom == RoomType.ENEMY)
            {
                //pass
            }

            if(playerLevel >= 5)
            {
                //pass
            }


        }
        yield return null;
    }

    void GenerateShopItems()
    {

    }

    public void IncreaseAttack(int amount)
    {
        playerAttack += amount;
    }
    public void IncreaseHealth(int amount)
    {
        playerHealth += amount;
    }
    public void IncreaseXP(int amount)
    {
        playerXP += amount;
    }
    public void increaseGold(int amount)
    {
        playerGold += amount;
    }
    public bool CanPlayerAfford(int cost)
    {
        return cost >= playerGold;
    }

    public void ContinueDialogue()
    {
        DialogueManager.Instance.Continue();
    }
}
