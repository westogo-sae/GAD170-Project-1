using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public ShopItem[] shopItems;
    public GameObject chestObj;
    public GameObject mimicObj;
    public Item[] chestItems;
    public GameObject[] enemyObjects;
    public GameObject gameOverScreen;

    public UIManager ui;

    public AudioSource hurtAudio;
    public DialogueStream defaultDialogueStream;
    public DialogueStream gameEndDialogue;

    public bool gameOver;
    public int maxHealth = 30;

    private int playerLevel;
    private int playerXP;
    private int levelUpThreashold;
    private int playerAttack;
    private int playerHealth;
    private int playerGold;
    private int defenceTime;

    private List<Enemy> currentEnemies;
    private bool isMimicEncounter;

    
    public InputAction restartAction;


    enum RoomType
    {
        SHOP,
        CHEST,
        ENEMY
    }

    RoomType currentRoom;

    #region Restart Input Enable Disable

    // required to make input work
    private void OnEnable() {
        restartAction.Enable();
        restartAction.performed += RestartAction;
    }

    private void OnDisable() {
        restartAction.Disable();
    }

    public void RestartAction(InputAction.CallbackContext context)
    {
        Restart();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    private void Start()
    {
        DialogueManager.Instance.PlayStream(defaultDialogueStream);
    }

    public void StartGame()
    {
        currentEnemies = new();
        StartCoroutine(GameLoop());
    }


    IEnumerator GameLoop()
    {
        //initialize player stats and level
        playerLevel = 0;
        playerXP = 0;
        levelUpThreashold = 15;
        playerHealth = maxHealth;
        playerAttack = 8;
        playerGold = 0;
        defenceTime = 0;
        
        //initialize UI
        ui.gameObject.SetActive(true);
        ui.UpdateLevel(playerLevel);
        ui.UpdateXP(playerXP, levelUpThreashold);
        ui.Updatehealth(playerHealth);
        ui.UpdateAttack(playerAttack);
        ui.UpdateGold(playerGold);
        ui.UpdateShield(defenceTime);
        

        while(!gameOver)
        {

            //bonus: show dialogue to let the player know the they have moved on
            DialogueManager.Instance.PlaySingle(new Dialogue(text: "You continue to the next room"));

            yield return DialogueManager.Instance.WaitUntilDialogueContinue();

            //select a random room
            currentRoom = (RoomType)Random.Range(0, 3);
            //currentRoom = 0;

            ////////////////////////////////////
            ///////////// SHOP ROOM ////////////
            ////////////////////////////////////

            if(currentRoom == RoomType.SHOP)
            {
                //display shop items with new random costs
                foreach(ShopItem item in shopItems)
                {
                    item.setCost(Random.Range(5,20));
                    item.gameObject.SetActive(true);
                }

                //shop dialogue
                string message = "You encounter three items in the room a rugged sign reads \"DO NOT STEAL\" ... better safe than sorry";
                //create dialogue with a continue option
                Dialogue dialogue = new Dialogue(text: message, options: new Dialogue.DialogueOption("Continue", ContinueDialogue));
                //play dialogue
                DialogueManager.Instance.PlaySingle(dialogue);

                //wait until the player continues
                yield return DialogueManager.Instance.WaitUntilDialogueContinue();

                //test if need to level up
                while(playerXP >= levelUpThreashold)
                {
                    yield return levelupRoutine();
                }

                //disable all shop items after continue
                foreach(ShopItem item in shopItems)
                {
                    item.gameObject.SetActive(false);
                }

            }

            ////////////////////////////////////
            ///////////// CHEST ROOM ///////////
            ////////////////////////////////////
            
            if(currentRoom == RoomType.CHEST)
            {
                //show chest
                chestObj.SetActive(true);
                ///////////setting up dialogue
                string message = "You see a chest in the centre of the room... open it?";
                bool chestOpened = false;
                Dialogue.DialogueOption optionNo = new Dialogue.DialogueOption("No", () => chestOpened = false);
                Dialogue.DialogueOption optionYes = new Dialogue.DialogueOption("Yes", () => chestOpened = true);
                Dialogue.DialogueOption[] options = {optionNo, optionYes};
                DialogueManager.Instance.PlaySingle(new Dialogue(text:message, options:options));

                yield return DialogueManager.Instance.WaitUntilDialogueContinue();
                
                if(chestOpened)
                {
                    //chance of encountering a mimic is 1/30
                    isMimicEncounter = Random.Range(0, 30) == 0;
                    if(isMimicEncounter)
                    {
                        //is a mimic encounter

                        //swap chest sprite with mimic sprite
                        chestObj.SetActive(false);
                        mimicObj.SetActive(true);
                        //reset enemy
                        Enemy mimic = mimicObj.GetComponent<Enemy>();
                        mimic.makeNewEnemy(7);
                        // damage player
                        takeDamage(mimic.attack);
                        //target mimic enemy
                        currentEnemies.Add(mimic);
                        //now in enemy room
                        currentRoom = RoomType.ENEMY;

                        ui.Updatehealth(playerHealth);
                        
                    }
                    else
                    {
                        //hide chest and show random item
                        chestObj.SetActive(false);
                        Item chestItem = chestItems[Random.Range(0, chestItems.Length)];
                        chestItem.gameObject.SetActive(true);
                        
                        //set up dialogue
                        message = "You found an Item!";
                        Dialogue dialogue = new Dialogue(text: message, options: new Dialogue.DialogueOption("Continue", ContinueDialogue));
                        DialogueManager.Instance.PlaySingle(dialogue);
                        
                        //wait for player to continue
                        yield return DialogueManager.Instance.WaitUntilDialogueContinue();

                        //hide all items
                        foreach(Item item in chestItems)
                        {
                            item.gameObject.SetActive(false);
                        }

                        //test if need to level up
                        while(playerXP >= levelUpThreashold)
                        {
                            yield return levelupRoutine();
                        }
                    }
                }else
                {
                    chestObj.SetActive(false);
                }
            }

            ////////////////////////////////////
            ///////////// ENEMY ROOM ///////////
            ////////////////////////////////////
            
            if (currentRoom == RoomType.ENEMY)
            {
                string message;

                if(!isMimicEncounter)
                {   
                    //generate enemies between 0 and 3
                    int enemyCount = Random.Range(0, 4);
                    if(enemyCount == 0)
                    {
                        //no enemies move on to next room
                        message = "The room is empty...";
                        DialogueManager.Instance.PlaySingle(new Dialogue(text:message));
                        yield return DialogueManager.Instance.WaitUntilDialogueContinue();
                        //skip to next loop
                        continue;
                    }else
                    {
                        for(int i = 0; i < enemyCount; i++)
                        {
                            //activate enemy objects
                            enemyObjects[i].SetActive(true);

                            //reset enemy stats to random level from 1 to 5
                            Enemy newEnemy = enemyObjects[i].GetComponent<Enemy>();
                            newEnemy.makeNewEnemy(Random.Range(1, 6));
                            //target enemies
                            currentEnemies.Add(newEnemy.GetComponent<Enemy>());
                        }
                    }
                 
                }
                    
            

                while (currentEnemies.Count > 0)
                {
                    //always target enemies[0]

                    //differentiate between mimic
                    if(isMimicEncounter)
                        message = $"You encountered: Mimic (Level {currentEnemies[0].level})";
                    else
                        message = $"You encountered:{currentEnemies[0].name} (Level {currentEnemies[0].level})";
                        
                    
                    //set up attack or defend dialogue
                    bool playerAttacked = false;
                    Dialogue.DialogueOption optionAttack = new Dialogue.DialogueOption("Attack", () => playerAttacked = true);
                    Dialogue.DialogueOption optionDefend = new Dialogue.DialogueOption("Defend", () => playerAttacked = false);
                    DialogueManager.Instance.PlaySingle(new Dialogue(text:message, options:new Dialogue.DialogueOption[] {optionAttack, optionDefend}));
                    yield return DialogueManager.Instance.WaitUntilDialogueContinue();

                    //if the player chose attack
                    if(playerAttacked)
                    {
                        currentEnemies[0].health -= playerAttack;
                        DialogueManager.Instance.PlaySingle(new Dialogue(text:"You Damaged the Enemy!"));
                    }else
                    {
                        //reset defence time
                        defenceTime = 3;
                        ui.UpdateShield(defenceTime);
                        DialogueManager.Instance.PlaySingle(new Dialogue(text:"Your Defence Increases, You take half damage for the next 3 turns."));
                    }
                    yield return DialogueManager.Instance.WaitUntilDialogueContinue();

                    if(currentEnemies[0].health <= 0)
                    {
                        //increase XP by enemy xp amount
                        playerXP += currentEnemies[0].XP;
                        playerGold += currentEnemies[0].gold;

                        //differentiate between mimic and orc
                        if(isMimicEncounter)
                            message = $"You killed a Mimic. Gained {currentEnemies[0].gold} gold and {currentEnemies[0].XP} XP.";
                        else
                            message = $"You killed {currentEnemies[0].name}. Gained {currentEnemies[0].gold} gold and {currentEnemies[0].XP} XP.";

                        //update UI
                        ui.UpdateXP(playerXP, levelUpThreashold);
                        ui.UpdateGold(playerGold);

                        //hide enemy
                        currentEnemies[0].gameObject.SetActive(false);

                        DialogueManager.Instance.PlaySingle(new Dialogue(text:message));
                        yield return DialogueManager.Instance.WaitUntilDialogueContinue();

                        //test if need to level up
                        while(playerXP >= levelUpThreashold)
                        {
                            yield return levelupRoutine();
                        }
                        
                        //ensure mimic encounter cannot be repeated
                        isMimicEncounter = false;
                        //remove enemy
                        currentEnemies.RemoveAt(0);
                    }
                    else 
                    {
                        //bonus: let the player know when the enemy is low health
                        if(currentEnemies[0].health <= 4)
                        {
                            DialogueManager.Instance.PlaySingle(new Dialogue(text:currentEnemies[0].name + " is bleeding profusly!"));
                            yield return DialogueManager.Instance.WaitUntilDialogueContinue();
                        }

                        int damage = currentEnemies[0].attack;
                        if(defenceTime > 0)
                        {
                            // take half damage if defended
                            damage = (int)(damage * 0.5f);
                        }

                        takeDamage(damage);
                        ui.Updatehealth(playerHealth);

                        if(playerHealth <= 0)
                        {
                            //display lose screen
                            gameOverScreen.SetActive(true);
                            //break out of game loop
                            gameOver = true;
                            yield break;
                        }
                    }
                    defenceTime--;
                    ui.UpdateShield(defenceTime);
                }
                defenceTime = 0;
                ui.UpdateShield(defenceTime);
            }

            if(playerLevel >= 5)
            {
                //break out of game loop
                gameOver = true;
                //hide UI
                ui.gameObject.SetActive(false);
                // play end dialogue
                DialogueManager.Instance.PlayStream(gameEndDialogue);
            }


        }
    }

    public void takeDamage(int amount)
    {
        playerHealth -= amount;
        hurtAudio.Play();
        if(playerHealth <= 0)
        {
            playerHealth = 0;
        }
    }

    IEnumerator levelupRoutine()
    {
        //show level up dialogue
        string message = "You have enough Xp to level up.";
        DialogueManager.Instance.PlaySingle(new Dialogue(text:message, options:new Dialogue.DialogueOption("Level Up",ContinueDialogue)));
        yield return DialogueManager.Instance.WaitUntilDialogueContinue();
        //increase level
        playerLevel ++;
        //allows for XP rollover
        playerXP -= levelUpThreashold;
        //increase stats and threashold
        levelUpThreashold += 5;
        playerAttack = (int)(playerAttack * 1.25);
        //update UI
        ui.UpdateLevel(playerLevel);
        ui.UpdateAttack(playerAttack);
        ui.UpdateXP(playerXP, levelUpThreashold);

        message = " You Leveled Up!! Your attack increases.";
        DialogueManager.Instance.PlaySingle(new Dialogue(text:message));
    }


    public void IncreaseAttack(int amount)
    {
        playerAttack += amount;
        ui.UpdateAttack(playerAttack);
    }
    public void IncreaseHealth(int amount)
    {
        playerHealth += amount;
        if(playerHealth > maxHealth) playerHealth = maxHealth;
        ui.Updatehealth(playerHealth);
    }
    public void IncreaseXP(int amount)
    {
        playerXP += amount;
        ui.UpdateXP(playerXP, levelUpThreashold);
    }
    public void increaseGold(int amount)
    {
        playerGold += amount;
        ui.UpdateGold(playerGold);
    }
    public bool CanPlayerAfford(int cost)
    {
        return playerGold >= cost;
    }

    public void ContinueDialogue()
    {
        DialogueManager.Instance.Continue();
    }

}
