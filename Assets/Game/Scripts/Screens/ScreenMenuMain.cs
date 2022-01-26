using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMenuMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("PlayGame").GetComponent<Button>().onClick.AddListener(PressedPlayButton);
        this.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("text.menu.main");
        this.transform.Find("SwitchLanguage").GetComponent<Button>().onClick.AddListener(OnSwitchLanguage);
    }

    private void OnSwitchLanguage()
    {
        if (LanguageController.Instance.CodeLanguage == "es")
        {
            LanguageController.Instance.CodeLanguage = "en";
        }
        else
        {
            LanguageController.Instance.CodeLanguage = "es";
        }
        this.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("text.menu.main");
    }

    private void PressedPlayButton()
    {
        GameController.Instance.PlayClicked();
    }

}
