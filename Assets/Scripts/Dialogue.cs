using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    public string name { get; private set;}
    public string text { get; private set; }
    public Sprite icon { get; private set; }
    public DialogueOption[] options { get; private set; }

    public Layout layout = Layout.Horizontal; //default to horizontal


    public delegate void dialogueDelegate();
    /// <summary>
    /// Container for the text and event of an option/button. Can be left empty to have no options.
    /// </summary>
    public class DialogueOption
    {
        public string optionText;
        private dialogueDelegate optionDelegate;

        public DialogueOption(string Text, dialogueDelegate Delegate = null)
        {
            optionText = Text;
            optionDelegate = Delegate;
        }

        /// <summary>
        /// Attaches a new delegate to the dialgue option
        /// </summary>
        /// <param name="Event">The new delegate to use</param>
        public void SetEvent(dialogueDelegate Delegate)
        {
            optionDelegate = Delegate;
        }

        /// <summary>
        /// calls the delegate attached to this dialogue option
        /// </summary>
        public void Invoke()
        {
            if(optionDelegate != null)
                optionDelegate.Invoke();
        }
    }

    public enum Layout
    {
        Horizontal,
        Vertical
    }

    #region Value Setters
    /// <summary>
    /// Sets the Name/Title of the dialogue
    /// </summary>
    public void setName(string Name)
    {
        name = Name;
    }
    /// <summary>
    /// Sets the Text of the dialogue
    /// </summary>
    public void setText(string Text)
    {
        text = Text;
    }
    /// <summary>
    /// Sets the Icon of the dialogue
    /// </summary>
    public void setIcon(Sprite Icon)
    {
        icon = Icon;
    }
    /// <summary>
    /// Sets the options of the dialogue
    /// </summary>
    public void setOptions(params DialogueOption[] Options)
    {
        options = Options;
    }
    /// <summary>
    /// Sets the all values of the dialogue
    /// </summary>
    public void setAll(string Name, string Text, Sprite Icon, params DialogueOption[] Options)
    {
        setName(Name);
        setText(Text);
        setIcon(Icon);
        setOptions(Options);
    }

    #endregion

}
