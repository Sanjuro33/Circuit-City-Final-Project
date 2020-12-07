using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    
    //Flag Variables
    [Header("Flag Variables")]
    [SerializeField] bool singleUse;
    [SerializeField] bool switcher;
    [SerializeField] bool switching;

    //Direction Variables
    [Header("Direction Variables")]
    [SerializeField] int directionIndex;
    [SerializeField] string directionTag;
    [SerializeField] List<string> directionMap = new List<string>();
 
    //Main Methods    

    // Start is called before the first frame update
    void Start()
    {
        switching = true;
        if(switcher)
        {
            directionIndex = 0;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (switcher)
        {
            directionTag = directionMap[directionIndex];
        }
    }

//Custom Methods
   
    //If the TurnIndicator is a switcher, it sends the command to switch to the next direction on the list
    public void OnTriggerExit(Collider collision)
    {
        if(switching)
        {
        UnityEngine.Debug.Log("I know that a car has exited my collider");
        switching = false;
            StartCoroutine(SwitchLabel());
        StartCoroutine(waitToSwitch());
        }
        
    }

    //switches the label on the turn indicator if it is a switcher, activated in the OnTriggerExit Method
    private IEnumerator SwitchLabel()
    {
        UnityEngine.Debug.Log("The Coroutine is starting");
        yield return new WaitForSeconds(1f);
        if (switcher)
        {
            directionIndex += 1;
            if (directionIndex >= directionMap.Count)
            {
                directionIndex = 0;
            }
            directionTag = directionMap[directionIndex];
        }
        //switching = true;

    }

    //tells the turnIndicator to wait to switch it's label to make sure that it doesn't collide with the car multiple times
    public IEnumerator waitToSwitch()
    {
        yield return new WaitForSeconds(.5f);
        switching = true;
    }

//Accessors and Mutators

    //Accesses the directionTag variable
    public string GetDirection()
    {
        return directionTag;
    }

    //Mutates the directionTag variable
    public void SetDirection(string direction)
    {
        directionTag = direction;
    }

    //Returns true if the indicator is a switcher
    public bool IsSwitcher()
    {
        return switcher;
    }

    //Mutates the value of switcher to become true
    public void MakeSwitcher()
    {
        switcher = true;
    }

    //Returns true if the turn indicator is single use
    public bool GetFrequency()
    {
        return singleUse;
    }

    //Adds another string value to the direction map
    public void AddToDirectionMap(string direction)
    {
        directionMap.Add(direction);
    }
}
