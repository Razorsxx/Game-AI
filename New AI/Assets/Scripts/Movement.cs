using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController characterController;
    public float walkSpeed = 6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        
        characterController.Move(move * walkSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            characterController.Move(move * (walkSpeed * 2) * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Goal")
        {
            Destroy(collision.gameObject);
            Debug.Log("You Win");
        }
    }
}
