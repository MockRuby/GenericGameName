using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    public float groundDrag;

    public float stamina = 5f;
    public bool sprint = false; 

    public float plauerHeight;
    public LayerMask ground;
    bool grouded;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        Sprint();
        SpeedControl();
        Jump();
        grouded = Physics.Raycast(transform.position, Vector3.down, plauerHeight * 0.5f + 0.2f, ground);

        if (grouded)
        {
            rb.drag = groundDrag;

        }
        else
        {
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void Jump()
    {
        if (grouded)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * 300f, ForceMode.Force);
            }
        }
    }

    private void Sprint()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !sprint && stamina > 0)
        {
            moveSpeed = moveSpeed * 1.5f;
            sprint = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) && sprint || stamina <= 0)
        {
            moveSpeed = 4f;
            sprint = false;
        }

        if (sprint)
        {
            stamina -= Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 4);
        }
        else
        {
            stamina += Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 4);
        }
    }

    private void SpeedControl()
    {
        Vector3 playerVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(playerVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = playerVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void OnGUI()
    {
        GUILayout.Label("FPS: " + 1.0f/Time.deltaTime);
    }
}
