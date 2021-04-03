using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public Rigidbody rb;

    public float moveSpeed;
    public float jumpForce;

    private void OnEnable()
    {
        //Lock the cursor to the middle of the screen.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
    }

    void Movement()
    {
        //Gets the input and converts it into a direction vector in relation
        //to where the player is faceing.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = transform.right * x + transform.forward * z;

        direction *= moveSpeed;
        direction.y = rb.velocity.y;

        rb.velocity = direction;
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        //Sets the groundRay
        Ray groundRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Vector3.down);
        RaycastHit hit;

        //Checks if anything is hit
        if(Physics.Raycast(groundRay, out hit, 0.2f))
        {
            return hit.collider != null;
        }

        return false;
    }
}