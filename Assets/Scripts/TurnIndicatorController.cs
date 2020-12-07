using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicatorController : MonoBehaviour
{
   [SerializeField] bool oneUse;
   // Start is called before the first frame update
   public bool IsOneUse()
   {
        return oneUse;
   }
}
