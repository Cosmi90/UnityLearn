using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    private Transform playerTransform;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    public GameObject player;
    private GameObject weapon;
     
    private float lastPositionY;
    private int direction;

    public float jumpVelocity;
    public float backdashVelocity;
    public float backdashTime;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float moveSpeed;
    public bool facingRight;

    void Awake()
    {
        playerTransform = transform.GetComponent<Transform>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        animator = transform.GetComponent<Animator>();
        player = transform.GetComponent<GameObject>();
        weapon = GameObject.Find("Weapon");
    }

    // Start is called before the first frame update
    void Start()
    {
        weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleAttack();

        lastPositionY = playerTransform.position.y;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, .1f, platformsLayerMask);

        bool isGrounded = raycastHit2d.collider != null;
        animator.SetBool("OnGround", isGrounded);

        return isGrounded;
    }

    private void FlipPlayer(float horizontal, bool isGrounded)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            if (isGrounded)
            {
                animator.SetTrigger("Turn");
            }
            else
            {
                animator.SetBool("TurnWhileJump", true);
            }
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private IEnumerator HandleBackdash()
    {
        animator.SetTrigger("Backdash");

        if (facingRight)
        {
            float initialBackdashTime = backdashTime;
            while (backdashTime > 0)
            {
                rigidbody2d.velocity += new Vector2(-backdashVelocity, rigidbody2d.velocity.y);
                backdashTime -= Time.deltaTime;

                yield return null;
            }
            backdashTime = initialBackdashTime;
        }
        else
        {
            float initialBackdashTime = backdashTime;
            while (backdashTime > 0)
            {
                rigidbody2d.velocity += new Vector2(+backdashVelocity, rigidbody2d.velocity.y);
                backdashTime -= Time.deltaTime;

                yield return null;
            }
            backdashTime = initialBackdashTime;
        }
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();

        // JUMP
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("JumpStart", true);
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
        }
        if (rigidbody2d.velocity.y < 0)
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody2d.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if ((!isGrounded && lastPositionY > playerTransform.position.y))
        {
            animator.SetBool("JumpFall", true);
            animator.SetBool("JumpStart", false);

            if (Input.GetKey(KeyCode.Space))
                animator.SetBool("LongStraightJump", true);
        }

        // UP
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded)
            {
                animator.SetBool("UseSimple", true);
            }
        }
        else if (!Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("UseSimple", false);
        }

        // DOWN
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            animator.SetBool("IsDucked", true);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.C))
                animator.SetTrigger("DuckSwordAttack");
        }
        else if (!Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("IsDucked", false);
        }

        // LEFT
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (isGrounded)
            {
                animator.SetBool("JumpFall", false);
            }
            animator.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));

            FlipPlayer(-moveSpeed, isGrounded);

            rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
        }

        // RIGHT
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (isGrounded)
            {
                animator.SetBool("JumpFall", false);
            }
            animator.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));

            FlipPlayer(+moveSpeed, isGrounded);

            rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
        }

        // DASH
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(HandleBackdash());
        }

        // No keys pressed
        else if (isGrounded)
        {
            animator.SetFloat("MoveSpeed", 0);
            animator.SetBool("JumpFall", false); // -- CHECK THE IsGrounded Method for MORE INFO !!! -- FIND BETTER PLACE TO PUS ALL OF THESE animator.Set*
            animator.SetBool("LongStraightJump", false);
            animator.SetBool("TurnWhileJump", false);

            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        }

        //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Alucard_MoveJumpStart"))
        //{
        //    animator.SetBool("JumpFall", true);
        //    animator.SetBool("JumpStart", false);

        //    if (Input.GetKey(KeyCode.Space))
        //        animator.SetBool("LongStraightJump", true);
        //}
    }

    private void HandleAttack()
    {
        // Attack
        if (Input.GetKeyDown(KeyCode.C))
        {
            weapon.SetActive(true);
            animator.SetTrigger("StandingSwordAttack");
        }
    }
}