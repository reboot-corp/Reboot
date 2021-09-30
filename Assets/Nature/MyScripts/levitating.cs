using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levitating : MonoBehaviour
{
    public float max = 1.0f;
    public float min;
    // concentration is a value between 0 and 1
    //public float concentration = 1f;
    float elevation = 0.1f;
    float concentration;
    // Start is called before the first frame update
    void Start()
    {
        min = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //   if (concentration > 1)
        //       {
        //       concentration = 1;
        //   }
        //   if (concentration < 0)
        //   {
        //       concentration = 0;
        //   }
  
        if (gameStartClass.ml == true)
        {
            concentration = concentrationClass.concentration_lvl;
        }
        if (gameStartClass.bp == true)
        {
            concentration = concentrationClass.band_power;
        }


      //  var concentration = concentrationClass.concentration_lvl;

        transform.position = new Vector3(transform.position.x, min - concentration * (max - min), transform.position.z);
        // Mathf.Lerp(min, max, concentration);
      //  float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
       // if (x > 0)
       // {
       //     if (concentration < 1)
       //         concentration += .05f;
       // }
       // if (x < 0)
       // {
       //     if (concentration > 0)
       //         concentration -= .05f;
       // }


        if(Input.GetKey(KeyCode.W))
        {
            concentration += elevation;
        }
        if(Input.GetKey(KeyCode.S))
        {
            concentration -= elevation;
        }
    }
}