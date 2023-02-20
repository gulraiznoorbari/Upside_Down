using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerSpawnPointA;
    [SerializeField] private Transform _playerSpawnPointB;
    [SerializeField] private TextMeshProUGUI _livesText;
    [HideInInspector] public int lives;

    [Header("Score Data")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Animator _scoreAnimator;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    private int score;
    private int ScoreAnimatorKey;

    [Header("Audio")]
    public AudioSource _audioSource;

    private static GameManager instance;

    // Singleton Instantiation:
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        score = 0;
        _scoreText.SetText(score.ToString());
        lives = 3;
        _livesText.SetText(lives.ToString());
        ScoreAnimatorKey = Animator.StringToHash("score");
    }

    private void Start()
    {
        Instantiate(_playerPrefab, _playerSpawnPointA.position, Quaternion.identity);
    }

    public void IncreaseScore()
    {
        _scoreAnimator.SetTrigger(ScoreAnimatorKey);
        score++;
        _scoreText.SetText(score.ToString());
    }

    public int getScore()
    {
        return score;
    }

    public void RespawnPlayerA()
    {
        lives--;
        if (lives <= 0)
        {
            _finalScoreText.SetText(getScore().ToString());
            StartCoroutine(DeathMenu());
        }

        Destroy(GameObject.FindGameObjectWithTag("Player"));
        _livesText.SetText(lives.ToString());
        Instantiate(_playerPrefab, _playerSpawnPointA.position, Quaternion.identity);
    }

    public void RespawnPlayerB()
    {
        lives--;
        if (lives <= 0)
        {
            _finalScoreText.SetText(getScore().ToString());
            StartCoroutine(DeathMenu());
        }

        Destroy(GameObject.FindGameObjectWithTag("Player"));
        _livesText.SetText(lives.ToString());
        Instantiate(_playerPrefab, _playerSpawnPointB.position, Quaternion.identity);
    }

    private IEnumerator DeathMenu()
    {
        yield return new WaitForSeconds(0.05f);
        UIManager.Instance.LoadDeathMenu();
    }
}
