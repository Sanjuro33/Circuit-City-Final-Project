using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExit : MonoBehaviour
{
    //Car Counts
    [Header("Car Counts")]
    [SerializeField] int numCarsInCircuit;
    [SerializeField] int numCarsCollided;
    
    //Flag Variables
    [Header("Flag Variables")]
    [SerializeField] bool accepting;

//Main Methods

    // Update is called once per frame
    void Update()
    {
        CheckForVictory();
    }

//Custom Methods
    
    //Delays the adding of cars that have passed through to ensure that a car is not counted more than once
    public IEnumerator waitToAdd()
    {
        yield return new WaitForSeconds(.5f);
        accepting = true;
    }

    //Adds 1 to the total cars collided each time an object with a CarController object passes through
    public void OnTriggerExit(Collider collision)
    {
        if (accepting)
        {
            numCarsCollided++;
            accepting = false;
            StartCoroutine(waitToAdd());
            UnityEngine.Debug.Log("One car has passed through my collider");
        }

    }

    //Checks for victory conditions
    public void CheckForVictory()
    {
        if (numCarsInCircuit == numCarsCollided)
        {
            StartCoroutine(GoToVictoryScreen());
        }
    }

    //Transitions to the victory screen with a slight delay
    public IEnumerator GoToVictoryScreen()
    {
        yield return new WaitForSeconds(1f);
        if (numCarsInCircuit == numCarsCollided)
        {
            SceneManager.LoadScene(2);
        }
    }


//Accessors and Mutators

    //Sets the number of cars in the circuit
    public void SetNumCarsInCircuit(int cars)
    {
        numCarsInCircuit = cars;
    }
}
