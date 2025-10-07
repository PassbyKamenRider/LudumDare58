using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnGameStart()
    {
        SceneManager.LoadScene("MainGame");
    }
}
