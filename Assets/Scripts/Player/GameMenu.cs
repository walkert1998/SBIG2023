using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject menuDropdown;
    public GameObject settingsMenu;
    public GameObject controlsMenu;
    public bool gamePaused = false;
    public bool canPause = true;
    public PlayerInput controller;
    //public NPCDialogue dialogue;
    // public LevelTransition levelTransition;
    //BackPack backPack;
    // Start is called before the first frame update
    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (menuDropdown != null)
        {
            menuDropdown.SetActive(false);
        }
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        if (controlsMenu != null)
        {
            controlsMenu.SetActive(false);
        }
        if (controller == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        //backPack = GetComponent<BackPack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && pauseMenuUI != null)
        {
            if (gamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        gamePaused = true;
    }

    public void PauseGame()
    {
        InventoryManager.HideNotePad_Static();
        pauseMenuUI.SetActive(true);
        controller.GetComponent<ThirdPersonController>().LockMovement(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // menuDropdown.SetActive(true);
        PlayerInteraction.LockInteraction();
        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        gamePaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        // menuDropdown.SetActive(false);
        settingsMenu.SetActive(false);
        // controlsMenu.SetActive(false);
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        gamePaused = false;
        if (!InventoryManager.IsNotePadOpen_Static() && !DialogueScreen.instance.dialogueScreenUI.activeSelf)
        {
            controller.GetComponent<ThirdPersonController>().LockMovement(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerInteraction.UnlockInteraction();
        }
    }

    public void OpenControls()
    {
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        // controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void MenuResume()
    {
        Time.timeScale = 1.0f;
        gamePaused = false;
    }

    public bool InSequence()
    {
        return false;
    }

    public void LoadMainMenu()
    {
        // LoadingScreen.SetLevelToLoadStatic("Level00", SceneManager.GetActiveScene().name);
        // LoadingScreen.levelToLoad = "Level00";
        // levelTransition.ChangeLevelTo("LoadingScreen");
    }

    public void RestartLevel()
    {
        // LoadingScreen.SetLevelToLoadStatic(SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().name);
        // LoadingScreen.levelToLoad = SceneManager.GetActiveScene().name;
        // levelTransition.ChangeLevelTo("LoadingScreen");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}
