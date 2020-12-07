using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public static class CircuitSolver 
{
    //solves the currents for each resistor in a presented list
    public static Dictionary<BusStopController, float> SolveLinearCircuit(BusStopController[] resistors, float voltage)
    {
        //Set up variables based on the arguments of the function
        float totResistance = TotalResistance(resistors);
        float totCurrent = FindCurrentThrough(totResistance, voltage);
        float startVoltage = voltage;
        List<string> solvedParallels = new List<string>();

        Dictionary<BusStopController, float> resistorCurrents = new Dictionary<BusStopController, float>();

        //Create a dictionary that allows for the easy return of the resistors and their individual current values
        foreach (BusStopController resistor in resistors)
        {
            resistorCurrents.Add(resistor, 0f);
        }
        //Go through each of the individual resistors and find their currents
        foreach (BusStopController resistor in resistors)
        {
            //If the resistor is in series it just has the total current, but it need to reduce the voltage potential
            if (resistor.gameObject.tag == "series")
            {
                resistorCurrents[resistor] = (float) MyRound(totCurrent * 100f) / 100f;
                float voltDrop = totCurrent * resistor.GetResistance();
                startVoltage -= voltDrop;
                //UnityEngine.Debug.Log(startVoltage.ToString() + "Beforehand");
            }
            //If the resistor is a parallel resistor it needs to be solved along with it's parallel bretheren
            else if (resistor.gameObject.tag.Substring(0, 8) == "parallel")
            {
                //make sure this group of parallel resistors hasn't already been solved
                if(!solvedParallels.Contains(resistor.gameObject.tag.Substring(0, 9)))
                {
                    //Create a dictonary to store the resistors in the circuit and their currents
                    Dictionary<string, float> seriesParallels = new Dictionary<string, float>();
                    Dictionary<BusStopController, float> parallelResistances = new Dictionary<BusStopController, float>();

                    int i = 0;
                    bool looping = true;
                    string parallelTag;

                    //Loop through all of the resistors that are in a parallel section (eg parallel1) and combine the resistances of their individual series conterparts (e.g parallel1.1 + parallel1.2)
                    while (looping)
                    {
                        i++;
                        parallelTag = resistor.gameObject.tag.Substring(0, 9) + "." + i;
                        seriesParallels.Add(parallelTag, 0f);
                        //Checks each individual resistor in the circuit to see if they belong to a group (e.g parallel1.1 and parallel1.2)
                        foreach (BusStopController subres in resistors)
                        {
                            //adds the values of series resistances inside of the parallel circuit
                            if(subres.gameObject.tag == parallelTag)
                            {
                                seriesParallels[parallelTag] = seriesParallels[parallelTag] + subres.GetResistance();
                            }
                        }

                        //sends out the parallel tag and it's value for debugging purposes
                        //UnityEngine.Debug.Log(parallelTag);
                        //UnityEngine.Debug.Log(seriesParallels[parallelTag]);
                        
                        //if there reaches a point where one of the subseries is equal to 0, it deletes it from the collection of parallel series resistances and ends the search for parallel series resistances
                        if (seriesParallels[parallelTag] == 0)
                        {
                            seriesParallels.Remove(parallelTag);
                            looping = false;
                        }
                    }
                    //Set a variable as the total parallel resistance
                    float totParallelResistance = 0f;
                    //Go through all of the keys and assign them their various currents

                    foreach (KeyValuePair<string, float> seriesResistance in seriesParallels)
                    {
                        //Sends out the name and value of each individual subseries in the parallel circuit.
                        //UnityEngine.Debug.Log(seriesResistance.Key);
                        //UnityEngine.Debug.Log(seriesResistance.Value);
                        UnityEngine.Debug.Log("The voltage through this resistor is: " + startVoltage);
                        float seriesCurrent = startVoltage / seriesResistance.Value;
                        UnityEngine.Debug.Log("The series current through this resistor is: " + seriesCurrent);
                        //finds the total series current accross a branch of the parallel circuit

                        //sets that series current as the current through each individual resistor in the subseries
                        foreach (BusStopController subres in resistors)
                        {
                            if (subres.gameObject.tag == seriesResistance.Key)
                            {
                          

                                resistorCurrents[subres] = (float) MyRound(seriesCurrent * 100f) / 100f;
                            }
                            
                        }
                        //adds the inverse of the the series resistance to the total value of the parallel circuit's resistance
                        totParallelResistance += 1 / seriesResistance.Value;
                        //UnityEngine.Debug.Log(seriesResistance.Value.ToString());
                        
                    }
                    //inverses the sum of the inverses of the individual subseries resistances to find the total resistance of the parallel element.
                    totParallelResistance = 1/totParallelResistance;
                    //UnityEngine.Debug.Log("The total resistance of the combined parallels of this tag is " + totParallelResistance);

                    //foreach (BusStopController subres in parallelResistances.Keys)
                    //{
                    // UnityEngine.Debug.Log(parallelResistances[subres]);
                    // UnityEngine.Debug.Log(startVoltage.ToString() + "Afterwards");
                    //resistorCurrents[subres] = startVoltage / parallelResistances[subres];
                    //totParallelResistance += 1/parallelResistances[subres];
                    //}


                    //Ackgnowedge the total voltage drop across the circuit from the parallel resistance
                    //UnityEngine.Debug.Log(startVoltage.ToString() + "Beforehand");
                    float voltDrop = totCurrent * 1/totParallelResistance;
                    UnityEngine.Debug.Log("I am dropping the voltage");
                    startVoltage -= voltDrop;
                    //UnityEngine.Debug.Log(startVoltage.ToString() + "Afterwards");
                    solvedParallels.Add(resistor.gameObject.tag.Substring(0, 9));
                }
                
            }

        }
        return resistorCurrents;
    }
    //Finds the total resistance for a circuit made out of the given list of resistors
    public static float TotalResistance(BusStopController[] resistors)
    {

        //Definie local variables for the method
        float totResistance = 0f;
        bool parallel = true;
        List<BusStopController> parallelResistors = new List<BusStopController>();
        List<float> parallelResistances = new List<float>();
        int i = 1;

        //Start checking for parallel circuit elements, check for parallel before series
        while (parallel)
        {
            //Set up the search for parallel1... can be later changed to parallel 2 if there are more than one parallel branch
            string tag = "parallel" + i.ToString();

            //As long as it's not a series resistor, add it if it's part of the parallelx set
            foreach (BusStopController resistor in resistors)
                if (resistor.gameObject.tag != "series")
                {

                    if (resistor.gameObject.tag.Substring(0, 9) == tag)
                    {
                        parallelResistors.Add(resistor);
                    }
                }
            //In the case there are no parallel resistors, exit the loop searching for parallel resistors
            if (parallelResistors.Count == 0)
            {
                parallel = false;
            }
            //If there are parallel resistors in the circuit:
            else
            {
                //set up local variables
                bool series = true;
                int j = 0;
                Dictionary<string, float> seriesParallels = new Dictionary<string, float>();
                
                //check through the resistances of each branch of the circuit
                while (series)
                {
                    j++;
                    string parallelTag = tag + "." + j;
                    seriesParallels.Add(parallelTag, 0f);

                    //adds eacj element of a subseries to it's total series resistance value
                    foreach (BusStopController subresistor in parallelResistors)
                    {
                        if (subresistor.tag == parallelTag)
                        {
                            seriesParallels[parallelTag] = seriesParallels[parallelTag] + subresistor.GetResistance();
                        }
                        
                    }

                    //If a subseries is reached that is not utilized, delete it from the list of parallel branches and stop the collection of series elements
                    if (seriesParallels[parallelTag] == 0f)
                    {
                        seriesParallels.Remove(parallelTag);
                        series = false;
                    }
                    
                }

                //adds the float values of the indidual paths to a list of float values
                foreach (KeyValuePair<string, float> seriesResistance in seriesParallels)
                {
                    parallelResistances.Add(seriesResistance.Value);
                }



                //Gets the total resistance of all of the path's values from the TotalParallelResistance method
                totResistance += TotalParallelResistance(parallelResistances);

            }
            //resets the circuit to look for the next group of parallel resistors
            i++;
            parallelResistors.Clear();
        }
        //adds series resistances to the total resistance of the circuit
        foreach (BusStopController resistor in resistors)
        {
            if (resistor.gameObject.tag == "series")
            {
                totResistance += resistor.GetResistance();
            }
        }
        //Returns the total resistance in the circuit
        return totResistance;
    }


    //Returns the sum of the resistances given to it
    private static float TotalSeriesResistance(List<float> resistances)
    {
        float tot = 0;
        foreach (float r in resistances)
            tot += r;
        return tot;

    }

    //Uses the parallel resistor equation to find the total resistance over a set of parallel resistances
    public static float TotalParallelResistance(List<float> resistances)
    {
        float tot = 0;
        foreach (float r in resistances)
            tot += (1 / r);
        tot = 1 / tot;
        return tot;
    }

    //uses Ohm's Law to find the total current in a linear system with a known voltage and resistance
    private static float FindCurrentThrough(float resistance, float voltage)
    {
        return voltage / resistance;
    }

    //Custom rounding method used to consistantly round up at a value of .5
    public static float MyRound(float value)
    {
        if (value % 0.5f == 0)
            return Mathf.Ceil(value);
        else
            return Mathf.Round(value);
    }

  
}
