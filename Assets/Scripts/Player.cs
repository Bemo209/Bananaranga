using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb;
    private Animator animator;
    private int score = 0;

    private bool isPowerUp = false;
    private Vector2 move;

    [SerializeField] private UIManager ui;
    public GameObject projectilePrefab;
    private Vector2 startPosition;
    public int lives = 3;
    private bool isJumping = false;
    private AudioSource _audio;
    public AudioClip collectSound;
    public Image powerUpTimer;
    private int jumpCount = 0;

    public float JumpHeight = 2.4f;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        _audio = GetComponent<AudioSource>();
        

    }

    private void Fire()
    {
        GameObject go = Instantiate(projectilePrefab, rb.transform.position, Quaternion.identity);
        Projectile pro = go.GetComponent<Projectile>();
        pro.Launch(new Vector2(direction, 0), 300);
    }

    void Update()
    {
        
        float moveX = Input.GetAxisRaw("Horizontal");
        move = new Vector2(moveX, 0);


        if (InputSystem.actions["Jump"].IsPressed() && jumpCount < 2)
        {
            InputSystem.actions["Jump"].Reset();
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(0,
                Mathf.Sqrt(-2 * Physics2D.gravity.y * JumpHeight)), ForceMode2D.Impulse);
            isJumping = true;
            jumpCount++;
        }

        
        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }

        if (move.x != 0)
        {
            direction = move.x < 0 ? -1 : 1;
            animator.SetInteger("Direction", direction);
        }

        animator.SetFloat("Move", Mathf.Abs(move.x));
    }

    void FixedUpdate()
    {
        // Smoothly move the player while preserving vertical velocity (gravity)
        float targetSpeed = move.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPowerUp && collision.gameObject.tag == "SpeedPowerUp")
        {
            Destroy(collision.gameObject);
            speed = speed * 2;
            isPowerUp = true;
            animator.speed *= 2;
            powerUpTimer.enabled = true;
        }
        if (!isPowerUp && collision.gameObject.tag == "Checkpoint")
        {
            startPosition = transform.position;
        }
        if (collision.gameObject.name.Contains("EnemyProjectile"))
        {
            lives--;
            ui.UpdateLives(lives);
            transform.position = startPosition;
        }
    }

    public void CollectGold_banana()
    {
        score++;
        ui.SetScore(score);
        _audio.PlayOneShot(collectSound);
    }
}