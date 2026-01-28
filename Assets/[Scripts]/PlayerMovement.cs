using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    Vector3 movementVector;

    [SerializeField] float speed = 3f;

    PlayerAnimation animate;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        movementVector = new Vector3();
        animate = GetComponent<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.y = Input.GetAxis("Vertical");

        animate.horizontal = movementVector.x;

        movementVector *= speed;

        rigidbody2d.linearVelocity = movementVector;
    }
}
