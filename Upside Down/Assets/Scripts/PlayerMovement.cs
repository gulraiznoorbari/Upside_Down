using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Run & Jump")]
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _speed;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip _death;

    private Rigidbody2D _rb;
    private bool isGrounded;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Independent of frame rate
    private void FixedUpdate()
    {
        #region GroundCheck
        isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        #endregion

        #region Run
        Vector2 _moveInput = Vector2.zero;
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector2(_moveInput.x * _speed, _rb.velocity.y);
        #endregion

        #region Jump
        if (Input.GetKeyDown(KeyCode.UpArrow)  && isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpHeight);
        }
        #endregion
    }

    public void DieA()
    {
        if (GameManager.Instance.lives > 0)
        {
            GameManager.Instance.RespawnPlayerA();
        }
    }

    public void DieB()
    {
        if (GameManager.Instance.lives > 0)
        {
            GameManager.Instance.RespawnPlayerB();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cones_A"))
        {
            GameManager.Instance._audioSource.PlayOneShot(_death);
            DieA();
            Debug.Log("Player Died and Respawning!");
        }

        if (collision.gameObject.CompareTag("Cones_B"))
        {
            GameManager.Instance._audioSource.PlayOneShot(_death);
            DieB();
            Debug.Log("Player Died and Respawning!");
        }
    }
}
