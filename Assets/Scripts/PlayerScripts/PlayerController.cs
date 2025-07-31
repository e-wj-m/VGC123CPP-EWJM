using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float groundCheckRadius = 0.2f;

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool isDiving = false;
    [SerializeField] private bool isCasting = false;

    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Animator anim;

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
            Debug.LogWarning("Ground Layer not set fool! Please set the Ground Layer in the LayerMask.");
        }
    }
    void Update()
    {

        float hValue = Input.GetAxis("Horizontal");

        if (!isCasting)
        {
            SpriteFlip(hValue);
            rb.linearVelocityX = hValue * 5f;
        }
        else
        {
            rb.linearVelocityX = 0f;
        }



        if (Input.GetButtonDown("Jump") && isGrounded && !isCasting)
        {
            rb.AddForce(Vector2.up * 9f, ForceMode2D.Impulse);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, groundLayer);
        isFalling = rb.linearVelocity.y < 0 && !isGrounded;

        bool downHeld = Input.GetKey(KeyCode.DownArrow);
        isDiving = isFalling && downHeld;

        if (Input.GetMouseButtonDown(0) && isGrounded && !isCasting)
        {
            anim.SetTrigger("Cast");
            isCasting = true;
        }

        anim.SetFloat("hValue", Mathf.Abs(hValue));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isFalling", isFalling);
    }

    void SpriteFlip(float hValue)
    {
        if ((hValue > 0 && sr.flipX) || (hValue < 0 && !sr.flipX))
            sr.flipX = !sr.flipX;
    }

    private void FixedUpdate()
    {
        if (isDiving)
        {
            rb.AddForce(Vector2.down * 35f);
        }

    }

    public void EndCast()
    {
        isCasting = false;
    }

}