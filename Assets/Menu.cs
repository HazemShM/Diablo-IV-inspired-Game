using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public static bool mainlevel;
    public static int currentLevel;


    // Start is called before the first frame update
    void Start()
    {
        mainlevel = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void onLevelButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void OnCharactersButton()
    {
        Time.timeScale = 1;
        currentLevel = 3;
        SceneManager.LoadScene(2);
    }

    public void onPlayButton()
    {
        if (currentLevel == 7)
        {
            GameManager.increaseLevels = true;
        }
        SceneManager.LoadScene(currentLevel);
        Time.timeScale = 1;
    }

    public void onMainLevelButton()
    {
        Time.timeScale = 1;
        mainlevel = true;
        currentLevel = 3;
        SceneManager.LoadScene(2);
    }

    public void onBossLevelButton()
    {
        Time.timeScale = 1;
        mainlevel = false;
        currentLevel = 7;
        SceneManager.LoadScene(2);
    }

    public void OnOptionsButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(4);
    }

    public void OnCreditsButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(5);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
