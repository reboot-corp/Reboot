using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class dropdownSelection : MonoBehaviour
{
    public TMP_Dropdown model_dropdown;
   // public TextMeshProUGUI textBox;
    // Start is called before the first frame update
    void Start()
    {
   //     var dropdown = transform.GetComponent<Dropdown>();
   //     dropdown.options.Clear();
   //
   //     List<string> items = new List<string>();
   //     items.Add("ML Model");
   //     items.Add("Alpha/Beta Power");
   //
   //     foreach(var item in items)
   //     {
   //         dropdown.options.Add(new Dropdown.OptionData() { text = item });
   //     }
   //
   //     DropdownItemSelected(dropdown);
   //
   //     dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });

    }

    public void DropdownItemSelected()
    {
        int val = model_dropdown.value;
        if (val == 0)
        {
            gameStartClass.ml = true;
            gameStartClass.bp = false;
        }
        else
        {
            gameStartClass.bp = true;
            gameStartClass.ml = false;
        }

        //int index = dropdown.value;
    //    textBox.text = dropdown.options[index].text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
