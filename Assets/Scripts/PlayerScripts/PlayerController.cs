using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    [SerializeField] private bool isGrounded = false;
    private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Animator anim;

    private Vector2 groundCheckPos => new Vector2(col.bounds.min.x + col.bounds.extents.x, col.bounds.min.y);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        groundLayer = LayerMask.GetMask("Ground");

        if (groundLayer == 0)
        {
            Debug.LogWarning("Ground Layer not set fool! Please set the Ground Layer in the LayerMask.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float hValue = Input.GetAxis("Horizontal");
        SpriteFlip(hValue);
        
        rb.linearVelocityX = hValue * 5f;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 9f, ForceMode2D.Impulse);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);

        anim.SetFloat("hValue", Mathf.Abs(hValue));
        anim.SetBool("isGrounded", isGrounded);
    }

    void SpriteFlip(float hValue)
    {
        if ((hValue > 0 && sr.flipX) || (hValue < 0 && !sr.flipX))
            sr.flipX = !sr.flipX;
    }
}
