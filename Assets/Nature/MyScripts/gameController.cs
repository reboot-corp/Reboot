using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public GameObject startButton;
    public GameObject dropdown;
    public GameObject UIConcentration;
    public bool menu = true;
    public void startGame()
    {
        if (menu == true)
        {
            startButton.SetActive(false);
            dropdown.SetActive(false);
            UIConcentration.SetActive(true);
            gameStartClass.gameStart = true;
            menu = false;
        }
        else
        {
            startButton.SetActive(true);
            dropdown.SetActive(true);
            UIConcentration.SetActive(false);
            gameStartClass.gameStart = false;
            menu = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
           startGame();
            print("Showing MENU");
        }
    }

 
}
