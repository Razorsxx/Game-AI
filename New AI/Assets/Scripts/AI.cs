using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float moveSpeed = 6f;
    private bool isPatrolling = false;
    private bool isWalking = false;
    private bool playerAlive = true;
    public float rotateAngle = 90f;
    public bool isRotatingr = false;
    public float viewDist = 10f;

    public Vector3 SeekTargetPos;
    public Movement playerMove;

    public GameObject goal;

    RaycastHit hit;
    //Create the FSM
    public enum State
    {
        PATROL,
        CHASE
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        //set the starting state to be in patrol
        state = AI.State.PATROL;

        StartCoroutine("FSM");
    }
    // Update is called once per frame
    void Update()
    {
        if (goal.gameObject == null)
        {
            playerAlive = false;
        }
    }
    IEnumerator FSM()
    {
        while(playerAlive)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASE:
                    Chase();
                    break;
            }
            yield return null;
        }    
    }

    IEnumerator Patrolling()
    {
        int walkWait = Random.Range(1, 3);
        int walkTime = Random.Range(1, 3);
        float rotatel = Random.Range(1, 2);
        int rotateTime = Random.Range(1, 3);

        isPatrolling = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        if (rotatel == 1)
        {
            isRotatingr = false;
            yield return new WaitForSeconds(rotateTime);
            isRotatingr = true;
            
        }
        else if (rotatel == 2)
        {
            isRotatingr = true;
            yield return new WaitForSeconds(rotateTime);
            isRotatingr = false;
        }
        isWalking = false;
        isPatrolling = false;
    }
    //Patrol State Logic
    void Patrol()
    {
        if (isPatrolling == false)
        {
            //start the wander movement
            StartCoroutine(Patrolling());
        }
        if(isWalking == true)
        {          
            Debug.DrawRay(transform.position + Vector3.up, transform.forward * 15f, Color.green);
            Debug.DrawRay(transform.position + Vector3.up, Quaternion.AngleAxis(30f, transform.up) * transform.forward * viewDist, Color.green);
            Debug.DrawRay(transform.position + Vector3.up, Quaternion.AngleAxis(-30f, transform.up) * transform.forward * viewDist, Color.green);
            //raycast checks if any obstacles are hit and rotates to avoid
            if (Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(30f, transform.up) * transform.forward, out hit, viewDist))
            {
                if (hit.collider.gameObject.tag == "Obstacles" || hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Wall")
                {
                    transform.Rotate(transform.up * Time.deltaTime * -rotateAngle);
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(-30f, transform.up) * transform.forward, out hit, viewDist))
            {
                if (hit.collider.gameObject.tag == "Obstacles" || hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Wall")
                {
                    transform.Rotate(transform.up * Time.deltaTime * rotateAngle);
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, viewDist) && Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(30f, transform.up) * transform.forward, out hit, viewDist) && Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(-30f, transform.up) * transform.forward, out hit, viewDist))
            {
                if (hit.collider.gameObject.tag == "Obstacles" || hit.collider.gameObject.tag == "Enemy")
                {
                    transform.Rotate(transform.up * Time.deltaTime * 180f);
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, viewDist))
            {
                if (hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Wall")
                {
                    transform.Rotate(transform.up * Time.deltaTime * 180f);
                }
            }
            //if nothing is hit by raycast it can proceed with the wander
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            if (isRotatingr == false)
            {
                transform.Rotate(transform.up * Time.deltaTime * -rotateAngle);
            }
            else
            {
                transform.Rotate(transform.up * Time.deltaTime * rotateAngle);
            }                                    
        }
        //checks if the player is spotted and changes state to chase the player
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 15f) || Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(30f, transform.up) * transform.forward, out hit, viewDist) || Physics.Raycast(transform.position + Vector3.up, Quaternion.AngleAxis(-30f, transform.up) * transform.forward, out hit, viewDist))
        {
            if (hit.collider.tag == "Player")
            {
                state = AI.State.CHASE;
                Debug.Log("Player Found");
            }
        }
    }
    //chase state logic
    void Chase()
    {
        //find the player position and calculates the desired velocity
        SeekTargetPos = GameObject.Find("Player").transform.position;
        Vector3 desiredVelocity = (SeekTargetPos - transform.position).normalized * playerMove.walkSpeed;
        //checks if the player is running to match the speed and seeks to player location
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += desiredVelocity * Time.deltaTime * 2;
        }
        else
        {
            transform.position += desiredVelocity * Time.deltaTime;
        }       
    }
    //check if the enemy has touched the player and kills them
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {         
            playerAlive = false;
            Debug.Log("Game Over");
        }
    }
}
