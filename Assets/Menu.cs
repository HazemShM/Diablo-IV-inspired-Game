using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void onLevelButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCharactersButton()
    {
        SceneManager.LoadScene(2);
    }

    public void onPlayButton()
    {
        SceneManager.LoadScene(3);
    }

    public void OnOptionsButton()
    {
        SceneManager.LoadScene(4);
    }

    public void OnCreditsButton()
    {
        SceneManager.LoadScene(5);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
