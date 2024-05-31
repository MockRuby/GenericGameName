using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orentaion;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;

    public CameraStyle currentStyle;

    public GameObject thirdPerson;
    public GameObject comabatCam;

    public Transform combatLookAt;
    public enum CameraStyle
    {
        Basic,
        Combat
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentStyle = CameraStyle.Basic;
    }

    // Update is called once per frame
    void Update()
    {
        // rotate cam
        Vector3 veiwDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orentaion.forward = veiwDir.normalized;

        // rotate the player
        if(currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = orentaion.forward * verticalInput + orentaion.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentStyle = CameraStyle.Combat;
                thirdPerson.gameObject.SetActive(false);
                comabatCam.gameObject.SetActive(true);
            }
        }

        else if(currentStyle == CameraStyle.Combat)
        {
            Vector3 combatVeiwDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orentaion.forward = combatVeiwDir.normalized;

            playerObj.forward = combatVeiwDir.normalized;


            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentStyle = CameraStyle.Basic;
                thirdPerson.gameObject.SetActive(true);
                comabatCam.gameObject.SetActive(false);
            }
        }
      
    }
}
