using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eegSelection : MonoBehaviour
{
    public GameObject museScript;
    public GameObject openBCIScript;

    // Start is called before the first frame update
    public void muse()
    {
        museScript.SetActive(true);
        openBCIScript.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void openBCI()
    {
        museScript.SetActive(false);
        openBCIScript.SetActive(true);
        gameObject.SetActive(false);
    }
}
