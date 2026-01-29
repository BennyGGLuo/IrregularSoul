using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 3f;
    Animator anim;
    private Vector2 input;
    private Vector2 lastMoveDirection;
    //private bool facingLeft = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        ProcessInputs();
        Animate();
        //if (input.x < 0 && !facingLeft || input.x > 0 && facingLeft)
        //{
        //    Flip();
        //}
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
    void ProcessInputs()
    {
        // Store last move direction when we stop moving
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if ((moveX == 0 && moveY == 0) && (input.x != 0 || input.y != 0))
        {
            lastMoveDirection = input;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Makes diagonal movement move the same rate as other movements
        // otherwise diagonal would be faster
        input.Normalize();
    }
    void Animate()
    {
        // Set animator parameters
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMoveDirection.x);
        anim.SetFloat("LastMoveY", lastMoveDirection.y);
    }
    //void Flip()
    //{
    //    Vector3 scale = transform.localScale;
    //    scale.x *= -1; // Makes x negative >> flips sprite
    //    transform.localScale = scale;
    //    facingLeft = !facingLeft;
    //}
}
