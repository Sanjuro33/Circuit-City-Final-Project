using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DifficultyPicker : MonoBehaviour
{
    //Dropdown menu component
    [SerializeField] TMP_Dropdown dropDownMenu;

//Main Methods
    // Start is called before the first frame update
    void Start()
    {
        dropDownMenu = GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(dropDownMenu.value);
        SceneController.difficulty = (dropDownMenu.value + 1);
    }


}
