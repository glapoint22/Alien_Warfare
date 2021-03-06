using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Prompt : MonoBehaviour {
    public bool isVisible;
    private UIPrompt prompt;
    private Groups fadeGroup;
    private float PromptFadeTime = 0.25f;
    private IEnumerator fadeOut;
    private IEnumerator fadeIn;

    //Delegate for canceling the prompt
    public delegate void CancelDelegate();
    public CancelDelegate cancelDelegate;

    public static Prompt instance
    {
        get
        {
            return (Prompt)FindObjectOfType(typeof(Prompt));
        }
    }
    

    // Use this for initialization
    void Awake ()
    {
        DontDestroyOnLoad(gameObject);
        prompt = transform.GetChild(0).GetComponent<UIPrompt>();
        
    }

    public void Initialize(Groups fadeGroup)
    {
        GameObject canvas = GameObject.Find("Canvas");
        transform.SetParent(canvas.transform, false);
        gameObject.SetActive(false);
        this.fadeGroup = fadeGroup;
    }

    public void Show(PromptInfo promptInfo)
    {
        //Position the prompt in the center of the screen
        RectTransform rectTransform = (RectTransform)prompt.transform;
        rectTransform.anchoredPosition = Vector3.zero;

        //Show the prompt
        gameObject.SetActive(true);
        prompt.panel.SetActive(false);

        //Set focus to button 1
        Scene.currentSelectedGameObject.GetComponent<UIEvent>().OnGameObjectDeselect();
        Scene.currentSelectedGameObject = prompt.button1.buttonComponent.gameObject;
        Scene.SetSelectedGameObject(false);


        //Set the title and description
        prompt.promptTitleText.text = promptInfo.promptTitle;
        prompt.promptDescriptionText.text = promptInfo.promptDescription;

        //XButton
        prompt.xButton.onClick.RemoveAllListeners();
        prompt.xButton.onClick.AddListener(promptInfo.xButtonAction);
        prompt.xButton.onClick.AddListener(Hide);

        //Button1
        prompt.button1.buttonText.text = promptInfo.button1Text;
        prompt.button1.buttonComponent.onClick.RemoveAllListeners();
        prompt.button1.buttonComponent.onClick.AddListener(promptInfo.button1Action);
        prompt.button1.buttonComponent.onClick.AddListener(Hide);

        if (promptInfo.button2Text == null)
        {
            prompt.button2.buttonComponent.gameObject.SetActive(false);
        }
        else
        {
            //Button2
            prompt.button2.buttonText.text = promptInfo.button2Text;
            prompt.button2.buttonComponent.onClick.RemoveAllListeners();
            prompt.button2.buttonComponent.onClick.AddListener(promptInfo.button2Action);
            prompt.xButton.onClick.AddListener(promptInfo.button2Action);
            prompt.button2.buttonComponent.onClick.AddListener(Hide);
        }


        //Fade in the prompt and fade out the scene
        fadeIn = UIGroups.Fade(Groups.Prompt, 0, 1, PromptFadeTime);
        fadeOut = UIGroups.Fade(fadeGroup, 0.05f, -1, PromptFadeTime);
        StartCoroutine(fadeIn);
        StartCoroutine(fadeOut);

        //Flag that the prompt is showing
        isVisible = true;
    }


    private void Hide()
    {
        isVisible = false;
        prompt.panel.SetActive(true);
        StartCoroutine(HidePrompt());
    }

    public void Cancel()
    {
        cancelDelegate();
        Hide();
    }

    private IEnumerator HidePrompt()
    {
        //Stop the fades if they are still running
        StopCoroutine(fadeIn);
        StopCoroutine(fadeOut);

        //Fade out the prompt and fade in the scene
        fadeIn = UIGroups.Fade(fadeGroup, 0, 1, PromptFadeTime);
        fadeOut = UIGroups.Fade(Groups.Prompt, 0, -1, PromptFadeTime);
        StartCoroutine(fadeIn);
        yield return StartCoroutine(fadeOut);

        //Disable the prompt
        gameObject.SetActive(false);
    }
}


[System.Serializable]
public struct PromptButton
{
    public Button buttonComponent;
    public Text buttonText;
}

public struct PromptInfo
{
    public string promptTitle;
    public string promptDescription;

    //Button1
    public string button1Text;
    public UnityAction button1Action;

    //Button2
    public string button2Text;
    public UnityAction button2Action;

    //xButton
    public UnityAction xButtonAction;
}
