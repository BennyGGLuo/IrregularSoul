using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 3f;
    Animator anim;

    [HideInInspector]
    public Vector2 movementVector;
    private Vector2 lastMoveDirection;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        lastHorizontalVector = -1f;
        lastVerticalVector = 1f;
    }

    void Update()
    {
        ProcessInputs();
        Animate();

        if (movementVector.x != 0)
        {
            lastHorizontalVector = movementVector.x;
        }
        if (movementVector.y != 0)
        {
            lastVerticalVector = movementVector.y;
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = movementVector * speed;
    }
    void ProcessInputs()
    {
        // Store last move direction when we stop moving
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if ((moveX == 0 && moveY == 0) && (movementVector.x != 0 || movementVector.y != 0))
        {
            lastMoveDirection = movementVector;
        }

        movementVector.x = moveX;
        movementVector.y = moveY;

        // Makes diagonal movement move the same rate as other movements
        // otherwise diagonal would be faster
        movementVector.Normalize();
    }
    void Animate()
    {
        // Set animator parameters
        anim.SetFloat("MoveX", movementVector.x);
        anim.SetFloat("MoveY", movementVector.y);
        anim.SetFloat("MoveMagnitude", movementVector.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }
}
