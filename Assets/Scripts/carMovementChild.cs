using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class carMovementChild : MonoBehaviour
{
    [SerializeField] CarMovement crMovement;
    bool rotateRight = true;
    
   
    //Starts a method on it's parent object's CarController script
    void OnTriggerExit(Collider collision)
    {

        crMovement.OnTriggerExitChild(collision);
        
        

    }

}  