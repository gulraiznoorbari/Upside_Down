using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip _collect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance._audioSource.PlayOneShot(_collect);
            GameManager.Instance.IncreaseScore();
            StartCoroutine(Collect());
        }
    }

    private IEnumerator Collect()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
