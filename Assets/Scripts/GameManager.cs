using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DialogueStream defaultDialogueStream;

    public bool gameOver;

    private void Start()
    {
        DialogueManager.Instance.PlayStream(defaultDialogueStream);
    }

    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    public void Die()
    {
        Debug.Log("arw nowr im ded");
    }

    IEnumerator GameLoop()
    {
        while(!gameOver)
        {
            Debug.Log("Game go brr");
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }
}
