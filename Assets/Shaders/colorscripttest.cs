using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorscripttest : MonoBehaviour
{
    public Material material;
    
    public Color colortest = new Color(1.0f, 0.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        
        material = GetComponent<MeshRenderer>().sharedMaterial;
        
        material.SetColor("testcolor", colortest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
