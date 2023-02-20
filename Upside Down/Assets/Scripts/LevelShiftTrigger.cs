using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelShiftTrigger : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private GameObject _previousLevel;
    [SerializeField] private GameObject _nextLevel;

    [Header("Animations")]
    [SerializeField] private Animator _levelRotateAnimation;
    private int _levelRotateAnimatorKey;

    [Space(10)]
    [SerializeField] private UnityEvent _spawnPlayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip _levelRotate;

    private void Awake()
    {
        _levelRotateAnimatorKey = Animator.StringToHash("RotateLevel");
        _nextLevel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("LevelShift"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance._audioSource.PlayOneShot(_levelRotate);
            _levelRotateAnimation.SetTrigger(_levelRotateAnimatorKey);
            _nextLevel.SetActive(true);
            StartCoroutine(DisablePrevLevel());
        }
    }

    private IEnumerator DisablePrevLevel()
    {
        yield return new WaitForSecondsRealtime(2f);
        Destroy(_previousLevel);
        _spawnPlayer.Invoke();
    }
}
