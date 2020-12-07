using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class BusStopController : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] TextMeshPro labelText;
    [SerializeField] Material yMat;
    [SerializeField] Material nMat;
    [SerializeField] MeshRenderer mesh;

    //Display
    [Header("Display")]
    [SerializeField] string textValue;

    //Calculations
    [Header("Calculations")]
    [SerializeField] float voltageKey;
    [SerializeField] float resistance;



//Main Methods

    // Start is called before the first frame update
    void Start()
    {
        labelText = transform.GetChild(0).GetComponent<TextMeshPro>();
        mesh = gameObject.GetComponent<MeshRenderer>();

        
    }

    // Update is called once per frame
    void Update()
    {
        labelText.text = resistance.ToString() + "Ω";
    }
    

//Custom Methods

    //Checks if the car that passes in front of it has a proper value for it's current
    public void OnTriggerEnter(Collider other)
    {
       
        if (other.transform.parent.GetComponent<CarController>() != null)
        {
            UnityEngine.Debug.Log("I can see a CarController");
            if (other.transform.parent.GetComponent<CarController>().GetLabelText() != voltageKey.ToString())
            {
                UnityEngine.Debug.Log(other.transform.parent.GetComponent<CarController>().GetLabelText());
                UnityEngine.Debug.Log(voltageKey);
                mesh.material = nMat;
                StartCoroutine(ResetGame());
            }
            else
            {
                mesh.material = yMat;
            }
        }
    }

   

    //Resets the scene/level with a delay
    public IEnumerator ResetGame()
    {
        

        yield return new WaitForSeconds(2f);
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }


   
    
//Accessors and Mutators    
    
    //Accessor for float resistance
    public float GetResistance()
    {
        return resistance;
    }
    //Mutator for float resistance
    public void SetResistance(float value)
    {
        resistance = value;
    }

    //Mutator for voltageKey
    public void SetVoltageKey(float voltage)
    {
        voltageKey = voltage;
    }

    //Mutator for the text on the Bus Stop's Label
    public void SetLabelText(float text)
    {
        resistance = text;
        labelText.text = resistance.ToString() + "Ω";
    }
}

    
