using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformsLayerMask;
    private Transform playerTransform;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider2d;
    private PlayerAnimationManager PAM;
    private float lastPositionY;

    public float jumpVelocity;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float moveSpeed;
    public bool facingRight;
    

    void Awake()
    {
        playerTransform = transform.GetComponent<Transform>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        PAM = transform.GetComponent<PlayerAnimationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60; // to be removed from here
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        lastPositionY = playerTransform.position.y;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, .1f, platformsLayerMask);

        bool isGrounded = raycastHit2d.collider != null;
        //animator.SetBool("OnGround", isGrounded);
        return isGrounded;
    }

    private void FlipPlayer(float horizontal, bool isGrounded)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            if (isGrounded)
            {
                PAM.PlayAnimation(new PlayAnimationInput
                {
                    AnimationName = "Alucard_Turn",
                    RequireFullPlay = true,
                    InterruptCurrentAnimation = true,
                    SelfIntrerrupt = true
                });
            }
            else
            {
                //animator.SetBool("TurnWhileJump", true);
            }
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();

        // JUMP
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            PAM.PlayAnimation(new PlayAnimationInput
            {
                AnimationName = "Alucard_IdleJumpStart",
                RequireFullPlay = true,
                InterruptCurrentAnimation = true
            });
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
        if (!isGrounded && lastPositionY > playerTransform.position.y)
        {
            //animator.SetBool("JumpFall", true);
            //animator.SetBool("JumpStart", false);

            //if (Input.GetKey(KeyCode.Space)) animator.SetBool("LongStraightJump", true);
        }

        // UP
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //PAM.PlayAnimation("Alucard_Use1", true, true, false, true);
            PAM.PlayAnimation(new PlayAnimationInput
            {
                AnimationName = "Alucard_Use1",
                RequireFullPlay = true,
                InterruptCurrentAnimation = true,
                PlayAnimationWhileKeyIsPressed = true
            });
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
            PAM.StopAnimation("Alucard_Use1");

        // LEFT
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!isGrounded)
            {
                PAM.PlayAnimation(new PlayAnimationInput
                {
                    AnimationName = "Alucard_MoveJumpStart",
                    RequireFullPlay = true,
                    PlayAnimationWhileKeyIsPressed = true
                });
            }
            //animator.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));

            FlipPlayer(-moveSpeed, isGrounded);

            rigidbody2d.velocity = new Vector2(-moveSpeed, rigidbody2d.velocity.y);
        }

        // RIGHT
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!isGrounded)
            {
                PAM.PlayAnimation(new PlayAnimationInput
                {
                    AnimationName = "Alucard_MoveJumpStart",
                    RequireFullPlay = true,
                    PlayAnimationWhileKeyIsPressed = true
                });
            }
            PAM.PlayAnimation(new PlayAnimationInput
            {
                AnimationName = "Alucard_StartRun1",
                InterruptCurrentAnimation = true,
                PlayAnimationWhileKeyIsPressed = true
            });

            FlipPlayer(+moveSpeed, isGrounded);

            rigidbody2d.velocity = new Vector2(+moveSpeed, rigidbody2d.velocity.y);
        }

        // No keys pressed
        else if (isGrounded)
        {
            //animator.SetFloat("MoveSpeed", 0);
            //animator.SetBool("JumpFall", false);
            //animator.SetBool("LongStraightJump", false);
            //animator.SetBool("TurnWhileJump", false);
            PAM.PlayAnimation(new PlayAnimationInput
            {
                AnimationName = "Alucard_Idle_1"
            });

            rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
        }
    }
}