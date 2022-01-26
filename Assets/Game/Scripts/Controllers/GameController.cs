using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : StateMachine
{
    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<GameController>();
            }
            return _instance;
        }
    }

    public const int MAIN_MENU = 0;
    public const int LOAD_GAME = 1;
    public const int GAME_RUNNING = 2;
    public const int WIN = 3;
    public const int LOSE = 4;
    public const int PAUSE = 5;

    public GameObject GameUI;

    public Player MyPlayer;
    public LevelController[] Levels;
    public int CurrentLevel = 0;

    private bool hasPlayerPressedButton = false;
    private float m_timeLoadProgress = 0;
    private bool m_pressedLoadGameSceneAgain = false;
    
    private bool m_hasPressedToResumeGame = false;
    private bool m_hasPressedToReloadGame = false;
    private int m_counterEnemiesKilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        SystemEventController.Instance.Event += ProcessSystemEvent;

        ChangeState(MAIN_MENU);
    }

    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {
            m_counterEnemiesKilled++;
        }
    }

    public void PlayClicked()
    {
        hasPlayerPressedButton = true;
    }

    private void RenderMenu()
    {

    }

    private void LoadGame()
    {

    }

    private bool PressedPlayButton()
    {
        return hasPlayerPressedButton;
    }

    private bool HasLoadedGame()
    {
        m_timeLoadProgress += Time.deltaTime;
        if (m_timeLoadProgress > 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RunGame()
    {

    }


    private bool IsPlayerDead()
    {
        return MyPlayer.Life <= 0;
    }

    private bool PressedNextButton()
    {
        return m_pressedLoadGameSceneAgain;
    }

    private void DisableAllScreens()
    {
        GameUI.SetActive(false);
        ScreenController.Instance.DestroyScreens();
    }

    private void ActivationScreenMenuMain(bool activation)
    {
        DisableAllScreens();
        ScreenController.Instance.CreateScreen("MenuMain", true);
    }

    private void ActivationScreenLoadGame(bool activation)
    {
        DisableAllScreens();
        m_pressedLoadGameSceneAgain = false;
        ScreenController.Instance.CreateScreen("LoadingScreen", true);
    }

    private void ActivationScreenWin(bool activation)
    {
        DisableAllScreens();
        ScreenController.Instance.CreateScreen("ScreenWin", true);
    }

    private void ActivationScreenLose(bool activation)
    {
        DisableAllScreens();
        ScreenController.Instance.CreateScreen("ScreenLose", true);
    }

    private void ActivationScreenPause(bool activation)
    {
        DisableAllScreens();
        ScreenController.Instance.CreateScreen("ScreenPause", true);
    }

    private void InitializeLogicGameElements()
    {
        GameUI.SetActive(true);
        MyPlayer.InitLogic();
        LevelController.Instance.InitializeLogicGameElements();
    }

    private void FreezeLogicGameElements()
    {
        GameUI.SetActive(false);
        MyPlayer.FreezeLogic();
        LevelController.Instance.FreezeLogicGameElements();
    }

    public void PressedNextButtonGameOver()
    {
        m_pressedLoadGameSceneAgain = true;
    }
    public void PressedGoToMainMenu()
    {
        ChangeState(MAIN_MENU);
    }

    private bool HasPressedPause()
    {
        return Input.GetKeyDown(KeyCode.P);
    }

    public void PressResumeGame()
    {
        m_hasPressedToResumeGame = true;
    }

    public void PressReloadGame()
    {
        m_hasPressedToReloadGame = true;
    }

    private bool ResumeGame()
    {
        return m_hasPressedToResumeGame;
    }

    private bool PressedToReloadGame()
    {
        return m_hasPressedToReloadGame;
    }

    public void SwitchLanguage()
    {
        if (LanguageController.Instance.CodeLanguage == "es")
        {
            LanguageController.Instance.CodeLanguage = "en";
        }
        else
        {
            LanguageController.Instance.CodeLanguage = "es";
        }
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);
        
        switch (m_state)
        {
            case MAIN_MENU:
                hasPlayerPressedButton = false;
                MyPlayer.GetComponent<Rigidbody>().useGravity = false;
                if (LevelController.Instance != null) LevelController.Instance.Destroy();
                CurrentLevel = 0;
                ActivationScreenMenuMain(true);
                SoundsController.Instance.StopSoundFXs();
                SoundsController.Instance.StopSoundBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_MAIN_MENU, true, 1);
                CameraController.Instance.FreezeCamera();
                Debug.Log("GAME CONTROLLER A ESTADO MAIN_MENU");
                Cursor.lockState = CursorLockMode.None;
                break;

            case LOAD_GAME:
                if (LevelController.Instance != null) LevelController.Instance.Destroy();
                Instantiate(Levels[CurrentLevel]);
                SoundsController.Instance.StopSoundBackground();
                ActivationScreenLoadGame(true);
                CameraController.Instance.FreezeCamera();
                Debug.Log("GAME CONTROLLER A ESTADO LOAD_GAME");
                break;

            case GAME_RUNNING:
                Cursor.lockState = CursorLockMode.Locked;
                MyPlayer.GetComponent<Rigidbody>().useGravity = true;
                SoundsController.Instance.StopSoundBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_INGAME, true, 1);
                DisableAllScreens();
                InitializeLogicGameElements();
                CameraController.Instance.RestorePreviousCamera();
                Debug.Log("GAME CONTROLLER A ESTADO GAME_RUNNING");
                break;

            case WIN:
                CurrentLevel++;
                SoundsController.Instance.StopSoundFXs();
                SoundsController.Instance.StopSoundBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_WIN, false, 1);
                ActivationScreenWin(true);
                Debug.Log("GAME CONTROLLER A ESTADO WIN");
                CameraController.Instance.FreezeCamera();
                Cursor.lockState = CursorLockMode.None;
                break;

            case LOSE:
                SoundsController.Instance.StopSoundFXs();
                SoundsController.Instance.StopSoundBackground();
                SoundsController.Instance.PlaySoundBackground(SoundsController.MELODY_LOSE, false, 1);
                ActivationScreenLose(true);
                CameraController.Instance.FreezeCamera();
                Debug.Log("GAME CONTROLLER A ESTADO LOSE");
                Cursor.lockState = CursorLockMode.None;
                break;

            case PAUSE:
                FreezeLogicGameElements();
                ActivationScreenPause(true);
                CameraController.Instance.FreezeCamera();
                Debug.Log("GAME CONTROLLER A ESTADO PAUSE");
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case MAIN_MENU:
                RenderMenu();
                if (PressedPlayButton() == true)
                {
                    ChangeState(LOAD_GAME);
                }
                break;
            case LOAD_GAME:
                LoadGame();
                if (HasLoadedGame() == true)
                {
                    ChangeState(GAME_RUNNING);
                }
                break;
            case GAME_RUNNING:
                RunGame();
                if (LevelController.Instance.HasKilledAllEnemies() == true)
                {
                    ChangeState(WIN);
                }
                if (IsPlayerDead() == true)
                {
                    ChangeState(LOSE);
                }
                if (HasPressedPause() == true)
                {
                    ChangeState(PAUSE);
                }
                break;
            case WIN:
                if (PressedNextButton() == true)
                {
                    ChangeState(LOAD_GAME);
                }
                break;
            case LOSE:
                if (PressedNextButton() == true)
                {
                    ChangeState(LOAD_GAME);
                }
                break;

            case PAUSE:
                if (ResumeGame() == true)
                {
                    m_hasPressedToResumeGame = false;
                    ChangeState(GAME_RUNNING);
                }
                if (PressedToReloadGame() == true)
                {
                    m_hasPressedToReloadGame = false;
                    SceneManager.LoadScene("Game");
                }
                break;
        }
    }
}
