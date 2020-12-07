using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class CircuitGenerator : MonoBehaviour
{


    //Components
    [Header("Components")]
    [SerializeField] private TurnIndicator levelEntrance;
    [SerializeField] public Transform componentPosition;

    //GameObjects
    [Header("GameObjects")]
    [SerializeField] GameObject car;
    [SerializeField] GameObject exit;

    // GameObject Lists
    [Header("GameObject Lists")]
    [SerializeField] private List<GameObject> centerComponents;
    [SerializeField] private List<GameObject> corners;
    [SerializeField] private List<GameObject> entryAndExit;

    //Integers
    [Header("Integers")]
    [SerializeField] private int topPaths;
    [SerializeField] private int midPaths;
    [SerializeField] private int botPaths;
    [SerializeField] private int parallels;
    [SerializeField] private int series;
    [SerializeField] private int Paths;
    [SerializeField] float paths = 0;

    //Floats
    [Header("Turn Flags")]
    [SerializeField] private float difficulty;
    [SerializeField] public float squareDist = 200f;
    [SerializeField] public float maxDist = 200f;
    [SerializeField] public float carSpacing = 50f;

//Main methods       
    // Start is called before the first frame update
    void Start()
    {
        //Set the intial number of parallels and series upon creation
        parallels = 0;
        series = 0;

    }

//Custom methods
    //Sets up a randomly generated field based on provided difficulty
    public void SetUpField(float difficulty)
    {
        //Establish the center of the circuit as a base for the random generation
        componentPosition = gameObject.transform.GetChild(0).GetComponent<Transform>();
        
        //Generate level of difficulty "1"
        if (difficulty == 1f)
        {
            
            //Create Frame
            CreateEntranceAndExit(0, 1, componentPosition);

            //Create Center Components
            int midPaths = PlaceComponentInCircuit(0f, componentPosition, "straight", true);
            //Paths = PlaceComponentInCircuit(0f, componentPosition, "straight", false);


            CreateCars(midPaths, 2* maxDist, componentPosition);
            CreateExit(midPaths, 1.25f * maxDist, componentPosition);
        }

        //Generate level of difficulty "2"
        if (difficulty == 2f)
        {
            //Create Frame
            CreateEntranceAndExit(4, 5, componentPosition);
            CreateCorners(4, componentPosition);

            //Create top paths
            int topPaths = PlaceComponentInCircuit(maxDist, componentPosition, "left", false);

            

            //Create bottom paths
            int botPaths = PlaceComponentInCircuit(-maxDist, componentPosition, "right", false);
          
            //Set up the levelEntrance in the level to switch directions the requisite amount of times
            levelEntrance.MakeSwitcher();

            //Generate enough cars for the level to be functioning
            CreateCars(botPaths + topPaths, 2 * maxDist, componentPosition);
            CreateExit(botPaths + topPaths, 1.25f * maxDist, componentPosition);
        }

        //Generate level of difficulty "3"
        if (difficulty == 3f)
        {
            //Create Frame
            CreateEntranceAndExit(6, 7, componentPosition);
            CreateCorners(4, componentPosition);


            //Create the top components
            int topPaths = PlaceComponentInCircuit(maxDist, componentPosition, "left", false);

            //Create the middle components
            int midPaths = PlaceComponentInCircuit(0f, componentPosition, "straight", false);
            
            //Create the bottom components
            int botPaths = PlaceComponentInCircuit(-maxDist, componentPosition, "right", false);





            //Set up the levelEntrance in the level to switch directions the requisite amount of times
            levelEntrance.MakeSwitcher();

            //Generate enough cars for the level to be functioning
            CreateCars(botPaths + topPaths + midPaths, 2 * maxDist, componentPosition);
            CreateExit(botPaths + topPaths + midPaths, 1.25f * maxDist, componentPosition);

        }
    }

    // Serves as a way to execute the CreateComponent Method while still accounting for the levelEntrance
    private int PlaceComponentInCircuit(float vertDist, Transform componentPosition, string directionTag, bool justSeries)
    {
        //Places a random component according to the parameters, finds the total number of paths
        int paths = CreateComponent(vertDist, componentPosition, justSeries);
        //decides how many times to add the tag to the level entrance depending on the number of paths in the circuit
        for (int x = paths; x > 0; x--)
        { 
            //Adds the tag ("right", "left", "straight") to the level entrance so it knows which way to direct the cars
            levelEntrance.AddToDirectionMap(directionTag);
        }
        //returns paths so the generator knows how many paths are in each part of the circuit
        return paths;
    }

    //Creates a random component when given a parent transform, a vertical distance from that parent transform, and a notification if it is soley for a series circuit
    private int CreateComponent(float vertDist, Transform componentPosition, bool justSeries)
    {
        

        //Randomly generate a component to place in the center
        GameObject centerComponent = Instantiate(centerComponents[UnityEngine.Random.Range(0, (centerComponents.Count)-1)]) as GameObject;
        centerComponent.transform.parent = componentPosition;
        centerComponent.transform.position = new Vector3(componentPosition.position.x, componentPosition.position.y, (componentPosition.position.z + vertDist));
        List<Transform> resistors = new List<Transform>();

        
        UnityEngine.Debug.Log(paths);
        //Add all of the resistors in the component to a list
        for (int x = 0; x < centerComponent.transform.GetChild(0).childCount; x++)
        {

            resistors.Add(centerComponent.transform.GetChild(0).GetChild(x));
        }
        
       
        //Only add 1 to the paths with each resistor if they're in parallel
        bool inSeries = false;
        foreach (Transform resistor in resistors)
        {
            if (resistors.Count != 0)
            {
                //Randomly assign the value of each resistor in the circuit
                resistor.GetComponent<BusStopController>().SetResistance(UnityEngine.Random.Range(1, 10) * 10);
                //Make sure that the circuit isn't just series resistors
                if (!justSeries)
                {
                    //Assign the parallel tag to resistors in series
                    if (resistor.gameObject.tag == "series")
                    {
                        resistor.gameObject.tag = "parallel" + 1 + "." + (series + 1).ToString();
                        inSeries = true;


                    }
                    //Assign the parallel tag to resistors in parallel
                    else if (resistor.gameObject.tag.Substring(0, 8) == "parallel")
                    {
                       
                        resistor.gameObject.tag = "parallel" + 1 + "." + (int.Parse(resistor.gameObject.tag.Substring(8).Substring(0, 1)) + series);
                        inSeries = false;
                        //Add one more path to the total after each individual parallel resistor
                        series += 1;

                    }
                  
                }
            }
            
        }
        //if the circuit element only has series resistances add one path to the count at the end
        if(inSeries)
        {
            series += 1;
        }
        return (int)centerComponent.GetComponent<CircuitComponent>().paths;
    }

    //Creates the entrance and exit to the level based on indexing them from a list of assigned entry and exit components
    private void CreateEntranceAndExit(int leftCode, int rightCode, Transform componentPostion)
    {
        //Create Entrance
        GameObject leftEntrance = Instantiate(entryAndExit[leftCode]) as GameObject;
        leftEntrance.transform.parent = componentPosition;
        leftEntrance.transform.position = new Vector3((componentPosition.position.x - maxDist), componentPosition.position.y, componentPosition.position.z);
        levelEntrance = leftEntrance.GetComponentInChildren<TurnIndicator>();

        //Create Exit
        GameObject rightEntrance = Instantiate(entryAndExit[rightCode]) as GameObject;
        rightEntrance.transform.parent = componentPosition;
        rightEntrance.transform.position = new Vector3((componentPosition.position.x + maxDist), componentPosition.position.y, componentPosition.position.z);
    }

    //Creates the corners for the circuit based on the number given
    private void CreateCorners(int numcorners, Transform componentPosition)
    { 
        //if the number given is greater than or equal to 2 it will generate the top left and right corners
        if(numcorners >= 2)
        {
            //Place the top left corner
            GameObject topLeft = Instantiate(corners[0]) as GameObject;
            topLeft.transform.parent = componentPosition;
            topLeft.transform.position = new Vector3((componentPosition.position.x - maxDist), componentPosition.position.y, (componentPosition.position.z + maxDist));
            //Place the top right corner
            GameObject topRight = Instantiate(corners[1]) as GameObject;
            topRight.transform.parent = componentPosition;
            topRight.transform.position = new Vector3((componentPosition.position.x + maxDist), componentPosition.position.y , (componentPosition.position.z + maxDist));
        }

        //If the number given is greater than or equal to 4 it will generate the bottom left and right corners
        if(numcorners == 4)
        {
            //Place the bottom left corner
            GameObject bottomLeft = Instantiate(corners[2]) as GameObject;
            bottomLeft.transform.parent = componentPosition;
            bottomLeft.transform.position = new Vector3((componentPosition.position.x - maxDist), componentPosition.position.y , (componentPosition.position.z - maxDist));
            //Place the bottom right corner
            GameObject bottomRight = Instantiate(corners[3]) as GameObject;
            bottomRight.transform.parent = componentPosition;
            bottomRight.transform.position = new Vector3((componentPosition.position.x + maxDist), componentPosition.position.y , (componentPosition.position.z - maxDist));
        }

        else
        {
            //tell the engine that it has requested an unacceptable amount of corners if it is under 2
            UnityEngine.Debug.Log("Error: " + numcorners + " is not a suitable amount of corners");
        }
    }

    //Generates the number of cars based on a provided number (usually from the number of paths in the circuit), the border at the edge of the level, and the transform for the center of the level
    private void CreateCars(int carNumber, float borderDistance, Transform componentPosition)
    {
        //Sets up the placement location for the first car
        float carPos = borderDistance;
        
        //repeats the car placement process for the 
        for(int x = carNumber; x > 0; x--)
        {
        //Places a car in the given position
        GameObject newCar = Instantiate(car) as GameObject;
        newCar.transform.parent = componentPosition;
        newCar.transform.position = new Vector3(componentPosition.position.x - carPos, componentPosition.position.y + 10, (componentPosition.position.z));
        newCar.transform.Rotate(0, 90, 0);

        //Sets the placement position a little bit farther back for each consecutive car
        carPos += carSpacing;
        }
    }

    public void CreateExit(int carNumber, float borderDistance, Transform componentPosition)
    {
        float exitPos = borderDistance;

        GameObject levelExit = Instantiate(exit) as GameObject;
        levelExit.GetComponent<LevelExit>().SetNumCarsInCircuit(carNumber);
        levelExit.transform.parent = componentPosition;
        levelExit.transform.position = new Vector3(componentPosition.position.x + exitPos, componentPosition.position.y, (componentPosition.position.z));
    }


//Accessors and Mutators
    //Sets the difficulty of the generation
    void SetDifficulty(float level)
    {
        difficulty = level;
    }
}
