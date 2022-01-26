using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenPause : MonoBehaviour
{
    void Start()
    {
        this.transform.Find("ButtonResume").GetComponent<Button>().onClick.AddListener(PressedResume);
        this.transform.Find("ButtonReload").GetComponent<Button>().onClick.AddListener(PressedReload);
    }

    private void PressedResume()
    {
        GameController.Instance.PressResumeGame();
    }

    private void PressedReload()
    {
        GameController.Instance.PressedGoToMainMenu();
    }

}
