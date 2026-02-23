using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    public GameObject statusMenu, mapMenu, optionsMenu;

    private bool isPaused = false;

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
        isPaused = true;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
