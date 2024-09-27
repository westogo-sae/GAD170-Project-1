using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text dialogueOut;
    [SerializeField]
    private TMP_Text nameOut;
    [SerializeField]
    private SpriteRenderer iconOut;

    public GameObject buttonPrefab;
    private GameObject[] activeButtons;
    public RectTransform horizontalBounds;
    public RectTransform verticalBounds; // unimplemented

    public InputAction continueAction;

    private bool hasOptions;
    private bool canContinue;

    public static DialogueManager Instance;

    #region Enable/Disable continueAction

    private void OnEnable()
    {
        continueAction.Enable();
        continueAction.performed += (Continue);
    }

    private void OnDisable()
    {
        continueAction.Disable();
    }

    private void Continue(InputAction.CallbackContext context)
    {
        if(!hasOptions)
        {
            canContinue = true;
        }
    }

    #endregion

    private void Awake()
    {
        if (Instance != null) // get singletoned
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        GameObject buttonObj = new GameObject();
    }


    /// <summary>
    /// Packs a dialogue's content to the applicable components, WaitUntilDialogueContinue should be used after this to prevent errors
    /// </summary>
    /// <param name="dialogue">The dialogue to be packed</param>
    public void packDialogue(Dialogue dialogue)
    {
        if (dialogue.name != null) nameOut.text = dialogue.name;

        if (dialogue.text != null) dialogueOut.text = dialogue.text;

        if (dialogue.icon != null) iconOut.sprite = dialogue.icon;

        if (dialogue.options.Length > 0)
        {
            hasOptions = true;
            // sets up button gameobjects and components
            destroyButtons();
            packOptionButtons(dialogue.options, dialogue.layout);
        }
        else
            hasOptions = false;
    }


    private void destroyButtons()
    {
        foreach(GameObject buttonObj in activeButtons)
        {
            Destroy(buttonObj);
        }
        activeButtons = null;
    }

    private void packOptionButtons(Dialogue.DialogueOption[] options, Dialogue.Layout layout = Dialogue.Layout.Horizontal)
    {
        int count = options.Length;
        activeButtons = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            switch (layout)
            {
                case Dialogue.Layout.Horizontal:
                    activeButtons[i] = Instantiate(buttonPrefab, horizontalBounds);

                    //streatch button to fit bounds
                    RectTransform buttonTrans = activeButtons[i].GetComponent<RectTransform>();
                    buttonTrans.anchorMin = new Vector2(i/count, buttonTrans.anchorMin.y);
                    buttonTrans.anchorMax = new Vector2((i+1)/count, buttonTrans.anchorMin.y);
                    break;
                default:
                    Debug.LogError("Layout not implemented Exception");
                    break;
            }
            //change button text
            activeButtons[i].GetComponentInChildren<Text>().text = options[i].optionText;

            //add listeners to button
            Button button = activeButtons[i].GetComponent<Button>();
            button.onClick.AddListener(options[i].Invoke);
            button.onClick.AddListener(() => canContinue = true);

            
        }
    }

    public IEnumerator WaitUntilDialogueContinue()
    {
        
        yield return new WaitUntil(() => canContinue);

        canContinue = false;
    }


}
