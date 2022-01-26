using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("ButtonReload").GetComponent<Button>().onClick.AddListener(PressedReload);
        this.transform.Find("ButtonGoToMenu").GetComponent<Button>().onClick.AddListener(PressedGoToMenuMain);
        this.transform.Find("Text").GetComponent<Text>().text = LanguageController.Instance.GetText("text.lose.screen");
    }

    private void PressedGoToMenuMain()
    {
        GameController.Instance.PressedGoToMainMenu();
    }

    private void PressedReload()
    {
        GameController.Instance.PressedNextButtonGameOver();
    }
}
