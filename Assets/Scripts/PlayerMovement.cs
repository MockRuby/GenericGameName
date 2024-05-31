using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public AnimationCurve horizontal;
    public AnimationCurve vertical;
    public AnimationCurve horizontalSprint;
    public AnimationCurve verticalSprint;

    Vector3 leftTagetOffeset;
    Vector3 rightTagetOffeset;

    float leftLegLast;
    float rightLegLast;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        leftTagetOffeset = leftFootTarget.localPosition;
        rightTagetOffeset = rightFootTarget.localPosition;
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

        leftFootTarget.rotation = Quaternion.identity;
        rightFootTarget.rotation = Quaternion.identity;
        MovePlayer();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        RaycastHit hit;
        if ( verticalInput != 0 || horizontalInput != 0 )
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            float leftLegForMove = horizontal.Evaluate(Time.time);
            float rightLegFoeMove = horizontal.Evaluate(Time.time - 1);
            if (!sprint && grouded)
            {
                leftFootTarget.localPosition = leftTagetOffeset + this.transform.InverseTransformVector(leftFootTarget.forward) * leftLegForMove + this.transform.InverseTransformVector(leftFootTarget.up) * vertical.Evaluate(Time.time + 0.5f);
                rightFootTarget.localPosition = rightTagetOffeset + this.transform.InverseTransformVector(rightFootTarget.forward) * rightLegFoeMove + this.transform.InverseTransformVector(rightFootTarget.up) * vertical.Evaluate(Time.time - 0.5f);
                float leftLegDir = leftLegForMove - leftLegLast;
                float rightLegDir = rightLegFoeMove - rightLegLast;
                if (leftLegDir < 0 && Physics.Raycast(leftFootTarget.position + leftFootTarget.up, -leftFootTarget.up, out hit, 100f))
                {
                    leftFootTarget.position = hit.point;
                    rb.AddForce(moveDirection.normalized * moveSpeed * Mathf.Abs(leftLegDir) * 100, ForceMode.Force);
                }
                if (rightLegDir < 0 && Physics.Raycast(rightFootTarget.position + rightFootTarget.up, -rightFootTarget.up, out hit, 100f))
                {
                    rightFootTarget.position = hit.point;
                    rb.AddForce(moveDirection.normalized * moveSpeed * Mathf.Abs(rightLegDir) * 100, ForceMode.Force);
                }
            }


            leftLegLast = leftLegForMove;
            rightLegLast = rightLegFoeMove;
        }
        else
        {
            if (grouded)
            {
                leftFootTarget.localPosition = new Vector3(-0.2f, 0, 0);
                rightFootTarget.localPosition = new Vector3(0.2f, 0, 0);
                Physics.Raycast(leftFootTarget.position + leftFootTarget.up, -leftFootTarget.up, out hit, 100f);
                leftFootTarget.position = hit.point;
                Physics.Raycast(rightFootTarget.position + rightFootTarget.up, -rightFootTarget.up, out hit, 100f);
                rightFootTarget.position = hit.point;
            }
        }
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
        float leftLegForSprint = horizontalSprint.Evaluate(Time.time);
        float rightLegFoeSprint = horizontalSprint.Evaluate(Time.time - 0.6f);
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
            if(grouded)
            {
                leftFootTarget.localPosition = leftTagetOffeset + this.transform.InverseTransformVector(leftFootTarget.forward) * leftLegForSprint + this.transform.InverseTransformVector(leftFootTarget.up) * verticalSprint.Evaluate(Time.time + 0.5f);
                rightFootTarget.localPosition = rightTagetOffeset + this.transform.InverseTransformVector(rightFootTarget.forward) * rightLegFoeSprint + this.transform.InverseTransformVector(rightFootTarget.up) * verticalSprint.Evaluate(Time.time - 0.5f);
                float leftLegDir = leftLegForSprint - leftLegLast;
                float rightLegDir = rightLegFoeSprint - rightLegLast;
                RaycastHit hit;
                if (leftLegDir < 0 && Physics.Raycast(leftFootTarget.position + leftFootTarget.up, -leftFootTarget.up, out hit, 100f))
                {
                    leftFootTarget.position = hit.point;
                    rb.AddForce(moveDirection.normalized * moveSpeed * Mathf.Abs(leftLegDir) * 100, ForceMode.Force);
                }
                if (rightLegDir < 0 && Physics.Raycast(rightFootTarget.position + rightFootTarget.up, -rightFootTarget.up, out hit, 100f))
                {
                    rightFootTarget.position = hit.point;
                    rb.AddForce(moveDirection.normalized * moveSpeed * Mathf.Abs(rightLegDir) * 100, ForceMode.Force);
                }
            }
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
