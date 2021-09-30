using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject gameMenu;

    public GameObject PC;
    public GameObject VR;

    private bool didSetMuse = false;

    void Start() {
        if (PlayerPrefs.GetInt("muse", -5) == -5)
            PlayerPrefs.SetInt("muse", -1);
        if (PlayerPrefs.GetInt("pc", -1) == -1)
            PlayerPrefs.SetInt("pc", 1);
    }

    public void changePC() {
        PlayerPrefs.SetInt("pc", 1);
        PC.SetActive(false);
        VR.SetActive(true);
    }
    public void changeVR() {
        PlayerPrefs.SetInt("pc", 0);
        PC.SetActive(true);
        VR.SetActive(false);
    }

    public void changeMenuMain() {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        gameMenu.SetActive(false);
        didSetMuse = false;
    }
    public void changeMenuSetting() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
    public void changeMenuGame() {
        if (PlayerPrefs.GetInt("muse") == -1) {
            didSetMuse = true;
            this.changeMenuSetting();
            return;
        }
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameMenu.SetActive(true);
    }
    public void quit() {
        Application.Quit();
    }
    public void setMuse() {
        PlayerPrefs.SetInt("muse", 1);
        if (!didSetMuse) {
            this.changeMenuMain();
        } else {
            this.changeMenuGame();
        }
    }
    public void setOpenBCI() {
        PlayerPrefs.SetInt("muse", 0);
        if (!didSetMuse) {
            this.changeMenuMain();
        } else {
            this.changeMenuGame();
        }
    }

    // example of starting a scene (must be loaded in project first - ask Andrew if unsure)
    public void startScene1() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void startScene2() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void startScene3() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
}
