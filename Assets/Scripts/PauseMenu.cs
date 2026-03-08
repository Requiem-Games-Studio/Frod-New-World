using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public GameObject statusMenu, mapMenu, optionsMenu;

    private bool isPaused = false;
    bool isBlockCursor;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void StatusMenu()
    {
        statusMenu.SetActive(true);
        mapMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void MapMenu()
    {
        statusMenu.SetActive(false);
        mapMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void OptionsMenu()
    {
        statusMenu.SetActive(false);
        mapMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }


    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (Cursor.visible)
        {
            isBlockCursor = false;
        }
        else
        {
            isBlockCursor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        isPaused = true;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (isBlockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        pauseMenuPanel.SetActive(false);        
        isPaused = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
