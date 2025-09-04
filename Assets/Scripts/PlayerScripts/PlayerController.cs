using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private int score = 0;
    
    public int GetScore() => score;
    public void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
    }

    [SerializeField] private int relicsCollected = 0;

    public void AddRelic()
    {
        relicsCollected++;
    }

    public int GetRelics()
    {
        return relicsCollected;
    }

    private float speedMultiplier = 1f;

    private Coroutine playerSpeedChange = null;

    public void ApplySpeedPowerup(float multiplier, float duration)
    {
        if (playerSpeedChange != null) 
            StopCoroutine(playerSpeedChange);
        playerSpeedChange = StartCoroutine(SpeedPowerup(multiplier, duration));
    }

    private IEnumerator SpeedPowerup(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        playerSpeedChange = null;
    }

    [SerializeField] private float groundCheckRadius = 0.2f;

    [SerializeField] float stompBounceSpeed = 12f;

    //[SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool isCasting = false;

    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Animator anim;
    [SerializeField] private GroundCheck groundCheck;
    private float initialGroundCheckRadius;

    private Vector2 groundCheckPos => new Vector2(col.bounds.min.x + col.bounds.extents.x, col.bounds.min.y);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        
        groundLayer = LayerMask.GetMask("Ground");

        if (groundLayer == 0)
        {
           Debug.LogWarning("Ground Layer not set fool! Please set the Ground Layer in the LayerMask. Groundcheck has not been created!");
        }

        groundCheck = new GroundCheck(col, groundLayer, ref groundCheckRadius);
        initialGroundCheckRadius = groundCheckRadius;
    }
    void Update()
    {
        groundCheck.CheckIsGrounded();

        float hValue = Input.GetAxis("Horizontal");

        if (!isCasting)
        {
            SpriteFlip(hValue);
            rb.linearVelocityX = hValue * 5f * speedMultiplier;
        }
        else
        {
            rb.linearVelocityX = 0f;
        }



        if (Input.GetButtonDown("Jump") && groundCheck.IsGrounded && !isCasting)
        {
            rb.AddForce(Vector2.up * 9f, ForceMode2D.Impulse);
        }

        isFalling = rb.linearVelocity.y < 0 && !groundCheck.IsGrounded;

        if (Input.GetMouseButtonDown(0) && groundCheck.IsGrounded && !isCasting)
        {
            anim.SetTrigger("Cast");
            isCasting = true;
        }

        anim.SetFloat("hValue", Mathf.Abs(hValue));
        anim.SetBool("isGrounded", groundCheck.IsGrounded);
        anim.SetBool("isFalling", isFalling);
        if (initialGroundCheckRadius != groundCheckRadius)
            groundCheck.UpdateGroundCheckRadius(initialGroundCheckRadius);
    }

    void SpriteFlip(float hValue)
    {
        if ((hValue > 0 && sr.flipX) || (hValue < 0 && !sr.flipX))
            sr.flipX = !sr.flipX;
    }
    public void EndCast()
    {
        isCasting = false;
    }

    public bool IsFalling()
    {
        return isFalling;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Squish Collider") && rb.linearVelocityY < 0)
        {
            collision.GetComponentInParent<Enemy>().TakeDamage(0, DamageType.JumpedOn);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * stompBounceSpeed * rb.mass, ForceMode2D.Impulse);
        }
    }

}