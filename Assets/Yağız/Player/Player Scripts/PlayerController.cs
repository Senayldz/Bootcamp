using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckRadius;
    [SerializeField] float turnSpeed;

    private GameObject draggableObject;

    private LayerMask groundLayer;

    public GameObject groundCheck;

    bool facingRight;
    public bool FacingRight { get { return facingRight; } }
    bool isGrounded;
    

    Collider[] groundcollision;

    Animator playerAnim;

    Rigidbody playerRb;

    PlayerDragController dragControl;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Draggable", "Ground");
    }
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        dragControl = GetComponent<PlayerDragController>();
        draggableObject = GameObject.FindGameObjectWithTag("Draggable");
        facingRight = true;
        isGrounded = true;

    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        PreventInfiniteJump();
        MovementWhileDragging();
        Jump();
        if (dragControl.IsPickedUp && transform.position.x > draggableObject.transform.position.x && facingRight )
        {
            Flip();
        }

        else if (dragControl.IsPickedUp && transform.position.x < draggableObject.transform.position.x && !facingRight)
        {
            Flip();
        }
    }


    void Movement()
    {
        float moveX = Input.GetAxis("Horizontal");
        playerAnim.SetFloat("speed", Mathf.Abs(moveX));

        playerRb.velocity = new Vector3(moveX * playerSpeed, playerRb.velocity.y, 0);

        if (moveX > 0 && !facingRight)
        {
            Flip();
        }

        else if (moveX < 0 && facingRight)
        {
            Flip();
        }

    }

    void Jump()
    {

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, 0);


        }
        playerAnim.SetBool("grounded", isGrounded);

    }

    public void Flip()
    {
        facingRight = !facingRight;

        Vector3 playerScale = transform.localScale;
        playerScale.z *= -1;
        transform.localScale = playerScale;
    }

    void PreventInfiniteJump()
    {
        groundcollision = Physics.OverlapSphere(groundCheck.transform.position, groundCheckRadius, groundLayer);

        if (groundcollision.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void MovementWhileDragging()
    {
        if (dragControl.IsPickedUp)
        {
            playerRb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            playerRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
        
    }

    

}
