using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CharacterController : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    public float moveSpeed = 5f; // Movement speed of the character

    private Rigidbody playerRb;
    Animator playerAnim;

    Vector3 lookDirection = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        // Freeze the rotation of the Rigidbody to prevent unwanted tilting.
        playerRb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(verticalInput);
        // Move the character on the Z (forward) and X (sideways) axes
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(-verticalInput, 0, horizontalInput);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.z, 0.0f))
        {
            lookDirection.Set(move.z, 0, move.x);
            lookDirection.Normalize();
        }
        playerAnim.SetFloat("MoveX", lookDirection.z);
        playerAnim.SetFloat("MoveZ", lookDirection.x);
        //SetFloat("Speed", move.magnitude);
    }

    private void FixedUpdate()
    {
        Vector3 position = playerRb.position;
        position.x = position.x + moveSpeed * -verticalInput * Time.deltaTime;
        position.z = position.z + moveSpeed * horizontalInput * Time.deltaTime;
        playerRb.MovePosition(position);
    }

}
