using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class CarController : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] TextMeshPro labelText;
    [SerializeField] CarMovement carMovement;

    //Display
    [Header("Display")]
    [SerializeField] string textValue;

//Main Methods
    // Start is called before the first frame update
    void Start()
    {
        labelText = transform.GetChild(1).GetComponent<TextMeshPro>();
        carMovement = gameObject.GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



//Accessors and Mutators

    //Mutates the text on the label of the car
    public void SetLabelText(string text)
    {
        textValue = text;
        labelText.text = textValue + "A";
    }

    //Accesses the text on the label of the car
    public string GetLabelText()
    {
        return textValue;
    }

    //Mutates the speed of the car
    public void SetSpeed(float speed)
    {
        carMovement.speed = speed;
    }

    

    
}
