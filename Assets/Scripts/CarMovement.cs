using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] Animator carAnimator;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider carCollider;
    [SerializeField] Transform childTransform;

    //Turn Flags
    [Header("Turn Flags")]
    [SerializeField] bool turnRight = false;
    [SerializeField] bool turnLeft = false;
    [SerializeField] bool canTurn = true;

    //Turn Values
    [Header("Turn Values")]
    [SerializeField] public float speed = 8f;
    [SerializeField] float turnSpeed = 170f;
    [SerializeField] Vector3 direction;
    [SerializeField] float normalRot;

//Main Methods
    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward;
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
        CheckForTurn();
    }

//Custom Methods

    //Checks to see if the car is exiting a turn collider and then reacts accordingly
    public void OnTriggerExitChild(Collider collision)
    {
        //Checks that what the car is colliding with is a TurnIndicator
        if (collision.gameObject.tag == "turnIndicator")
        {
            //Checks to see that the car hasn't been commanded to turn already
            if (canTurn)
            {
                //Registers the TurnIndicator component of the object it's colliding with
                TurnIndicator indicator = collision.gameObject.GetComponent<TurnIndicator>();
                UnityEngine.Debug.Log("I've collided with something");

                //If it's a right turn marker, flag to turn right
                if (indicator.GetDirection() == "right")
                {

                    //UnityEngine.Debug.Log("I've collided with a right marker");

                    normalRot = transform.rotation.eulerAngles.y;
                    
                    turnLeft = false;
                    turnRight = true;

                    direction = transform.forward;
                }

                //If it's a right turn marker, flag to turn left
                if (indicator.GetDirection() == "left")
                {
                    //UnityEngine.Debug.Log("I've collided with a left marker");
                    normalRot = transform.rotation.eulerAngles.y;
                    
                    turnRight = false;
                    turnLeft = true;

                    direction = transform.forward;
                }

                //flag canTurn to be false until made true after the waitToTurn() coroutine
                canTurn = false;
                StartCoroutine(waitToTurn());

            }
                
        }
    }

    public void CheckForTurn()
    {
        //execute when the turnRight flag has been set to true
        if (turnRight)
        {
           
            //float someSpeed = 170f;

            //Use Quaternion.RotateTowards to rotate the object 90 degrees to the right over an extended period of time
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, (normalRot + 90), 0), turnSpeed * Time.deltaTime);
            direction = transform.forward;

            //Check for when the car has turned to the required angle and tell it to stop turning
            if (transform.rotation.eulerAngles.y == (normalRot + 90))
            {
                turnRight = false;
            }
        }

        //execute when the turnLeft flag has been set to true
        if (turnLeft)
        {

            
            //float someSpeed = 170f;

            //Use Quaternion.RotateTowards to rotate the object 90 degrees to the left over an extended period of time
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, (normalRot - 90), 0), turnSpeed * Time.deltaTime);
            direction = transform.forward;

            //Check for when the car has turned to the required angle and tell it to stop turning
            if (transform.rotation.eulerAngles.y == (normalRot - 90))
            {
                
                turnLeft = false;


            }
        }
    }
    //Waits a small amount of time to make sure that it doesn't collide twice with the same turnIndicator
    public IEnumerator waitToTurn()
    {
        yield return new WaitForSeconds(.5f);
        canTurn = true;
    }

    //Tells the turnIndicator to switch it's value from left to right
    public IEnumerator WaitToTurnAgain(TurnIndicator indicator)
    {
        yield return new WaitForSeconds(1f);

        if (indicator.IsSwitcher())
        {
            //UnityEngine.Debug.Log("I've collided with a switcher");
            if (indicator.GetDirection() == "right")
            {
                //UnityEngine.Debug.Log("I'm changing it from right to left");
                indicator.SetDirection("left");
            }

            if (indicator.GetDirection() == "left")
            {
                indicator.SetDirection("right");
            }
        }
    }
   
    
}
