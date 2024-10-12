using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Fox : MonoBehaviour
{
    public static event Action HasLanded;

    Rigidbody2D rb;
    Animator animator;
    const float groundCheckRadius = 0.01f;
    const float overheadCheckRadius = 0.01f;
    const float wallCheckRadius = 0.01f;
    public bool isDead = false;
    bool isCrouchPressed;
    bool isRunning;
    float speed = 200f;
    float horizontalValue;

    [Header("Ground & Header")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform overheadCheckCollider;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Collider2D crouchingCollider;
    [SerializeField] bool isGrounded;

    [Header("Jump & Crounch")]
    [SerializeField] float speedModifier = 1f;
    [SerializeField] float jumpPower = 1f;
    [SerializeField] bool isJumped;
    [SerializeField]  bool coyoteJump;
    [SerializeField] int totalJumps;
    [SerializeField] int availableJumps;

    [Header("Wall")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallCheckCollider;
    [SerializeField] float slideFactor = 0.1f;
    //[SerializeField] bool isSliding = false;

    

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player can move
        if (!CanControl())
            return;

        horizontalValue = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;

        //if press Jump button, enable bool jump, othwewise turn it off
        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteJump && !Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer))
                Jump();  
        }

        //if press Crouch button, enable bool crouch, othwewise turn it off
        if (Input.GetButtonDown("Crouch"))
            isCrouchPressed = true;
        else if (Input.GetButtonUp("Crouch"))
            isCrouchPressed= false;

        //Set the bool yVelocity of animator to Rb.velocity.y
        animator.SetFloat("yVelocity", rb.velocity.y);

        //Check if touching a wall
        WallCheck();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, isCrouchPressed);
    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (collider.Length > 0)
        {
            //Only reset jump IF you dont jump (press Jump)
            if (isJumped == false)
                availableJumps = totalJumps;
            isGrounded = true;
            isJumped = false;
            coyoteJump = true;

            //Trigger the HasLanded 
            if(!wasGrounded)
                HasLanded?.Invoke();
            if (!wasGrounded && animator.GetFloat("yVelocity") < -4.8f)
                AudioManager.instance.PlaySFX("landing");

            //Check if ground has a tag moving platform
            foreach(var c in collider)
            {
                if(c.tag == "MovingPlatform") 
                    transform.parent = c.transform;
            }

        } else
        {
            //Un-parent the transform
            transform.parent = null;   

            isGrounded = false;
            isJumped=true;
            if (animator.GetFloat("yVelocity") < 0 && availableJumps == totalJumps)
                StartCoroutine(CoyoteJumpDelay());
            else 
                coyoteJump = true;
        }
        
        animator.SetBool("Jump", !isGrounded);
    }

    #region Jump
    IEnumerator CoyoteJumpDelay()
    {
        yield return new WaitForSeconds(0.2f);
        coyoteJump = false;
    }

    void Jump()
    {
        //if the player is grounded and not crouching, pressed Jump
        if (!isCrouchPressed && availableJumps > 0)
        {
            // Mark that you jumped so that jump turn dont reset
            isJumped = true;
            //Add jump force
            rb.velocity = Vector2.up * jumpPower;
            availableJumps--;        
        }
        
    }
    #endregion
    private void Move(float horizontalValue, bool isCrouched)
    {
        #region Crounch
       
        if (isGrounded)
        {
          
            //ìf we are crouch and disable crouch
            //check overhead for collision with Grouch Items
            //remain crouch if there are any
            if(!isCrouched)
            {
                if (Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer))
                {
                    isCrouched = true;
                }
            }

            //if the player is grounded and pressing Crouch:
            //  + animate crouching
            //  + reduce speed
            //  + disable standing collider
            standingCollider.enabled = !isCrouched;
            crouchingCollider.enabled = isCrouched;

            //standingCollider.offset = new Vector2(-0.01563479f, -0.06867006f);
            //((BoxCollider2D)standingCollider).size = new Vector2(0.18f, 0.18f);
        }
        else
        {
            isCrouched = false;
        }

        animator.SetBool("Crouch", isCrouched);
        
        #endregion

        #region Move & Run
        if (isRunning)
            speedModifier = isCrouched ? 0.5f : 2f;
        else
            speedModifier = isCrouched ? 0.5f : 1f;
        Vector2 targetVelocity = new Vector2 (horizontalValue * speed * speedModifier * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = targetVelocity;


        //Current scale
        Vector3 currentScale = transform.localScale;
        //Facing direction
        if (horizontalValue * currentScale.x < 0) 
            currentScale.x = currentScale.x * -1;
        transform.localScale = currentScale;

        //0 for idle, 6 for walk, 12 for run => Setting xVelocity in Fox animator
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }

    void WallCheck()
    {
        //If wwe are touching a wall and moving toward wall and falling
        //Slide on the wall
        if (Physics2D.OverlapCircle(wallCheckCollider.position, wallCheckRadius, wallLayer) 
            && Mathf.Abs(horizontalValue) > 0
            && rb.velocity.y < 0
            && !isGrounded)
        {
            //isSliding = true;
            Vector2 velocity = rb.velocity;
            velocity.y = -slideFactor;
            rb.velocity = velocity;

            //Jumping
            availableJumps = 1;
            if (Input.GetButtonDown("Jump"))
            {
                if (coyoteJump && !Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer))
                {       
                    Jump();
                    //isSliding = false;
                }
            }
        } else
        {
            //isSliding = false;
        }

    }


    public void Die()
    {
        isDead = true;
        FindObjectOfType<LevelManager>().Restart();
    }

    public void ResetPlayer()
    {
        horizontalValue = 0f;
    }

    bool CanControl()
    {
        // Nếu đang xem xét vật thể thì không thể di chuyển
        if (FindObjectOfType<InteractionSystem>().isExamining)
            return false;
        if(FindObjectOfType<InventorySystem>().isOpen)
            return false;
        if (isDead)
            return false;
        return true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckCollider.position, groundCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(overheadCheckCollider.position, overheadCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallCheckCollider.position, wallCheckRadius);

    }




}
