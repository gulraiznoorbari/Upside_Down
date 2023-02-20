using System.Collections;
using UnityEngine;
using TMPro;

public class LevelCompletionTrigger : MonoBehaviour
{
    [Header("Final Score")]
    [SerializeField] private TextMeshProUGUI _finalScoreText;

    [Header("Sounds")]
    [SerializeField] private AudioClip _levelCompleted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _finalScoreText.SetText(GameManager.Instance.getScore().ToString());
            GameManager.Instance._audioSource.PlayOneShot(_levelCompleted);
            UIManager.Instance.LoadGameCompletionMenu();
        }
    }
}
