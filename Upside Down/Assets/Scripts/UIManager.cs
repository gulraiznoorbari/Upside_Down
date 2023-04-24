using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Menus")]
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _gameCompletionMenu;
    [SerializeField] private GameObject _loginScreen;
    [SerializeField] private GameObject _registerScreen;
    [SerializeField] private GameObject _startScreen;

    [Header("Animations")]
    [SerializeField] private Animator _completionMenuAnimator;

    private int CompletionMenuAnimatorKey;

    private static UIManager instance;

    // Singleton Instantiation:
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<UIManager>();
            return instance;
        }
    }

    private void Awake()
    {
        _startScreen.gameObject.SetActive(true);
        CompletionMenuAnimatorKey = Animator.StringToHash("LevelCompletionMenu");
    }

    public void LoadDeathMenu()
    {
        Time.timeScale = 0f;
        _deathMenu.SetActive(true);
    }

    public void LoadGameCompletionMenu()
    {
        _gameCompletionMenu.SetActive(true);
        _completionMenuAnimator.SetTrigger(CompletionMenuAnimatorKey);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void EnableLoginScreen()
    {
        _startScreen.gameObject.SetActive(false);
        _loginScreen.gameObject.SetActive(true);        
    }

    public void EnableRegisterScreen()
    {
        _startScreen.gameObject.SetActive(false);
        _registerScreen.gameObject.SetActive(true);        
    }

    public void OnLogin()
    {
        WhiteLabelLoginManager.Instance.Login();
        Time.timeScale = 1f;
    }

    public void OnRegister()
    {
        WhiteLabelLoginManager.Instance.Register();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
