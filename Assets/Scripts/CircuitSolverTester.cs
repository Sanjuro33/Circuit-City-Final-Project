using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CircuitSolverTester : MonoBehaviour
{
    [SerializeField] BusStopController[] stops;
    [SerializeField] List<float> resistances;
    [SerializeField] Dictionary<BusStopController, float> resistorCurrents;
    // Start is called before the first frame update
    void Start()
    {
        stops = FindObjectsOfType<BusStopController>();
        
       // UnityEngine.Debug.Log("Total Parallel Resistance: " + CircuitSolver.TotalParallelResistance(resistances));
        UnityEngine.Debug.Log("Total Resistance: " +CircuitSolver.TotalResistance(stops));
       // resistorCurrents = CircuitSolver.SolveLinearCircuit(stops, 5);
       // foreach (BusStopController key in resistorCurrents.Keys) 
        //{
            //UnityEngine.Debug.Log(" resistor: " + key.ToString() + " current: " + resistorCurrents[key].ToString());
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
