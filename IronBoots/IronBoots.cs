using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronBoots : MonoBehaviour {

    public ThirdPersonController tpc;
    public GameObject water;
    
	// Use this for initialization
	void Start () {
        water = GameObject.FindWithTag("Water");
	}
	
	// Update is called once per frame
	void Update () {
		if(tpc.onWaterSurface && Input.GetKeyDown(KeyCode.I))
        {
            water.layer = 0;
            tpc.onWaterSurface = false;
            tpc.gravityIntesnity = 0.15f;
        }

        else if(!tpc.onWaterSurface && Input.GetKeyDown(KeyCode.I))
        {
            water.layer = 4;
            tpc.onWaterSurface = true;
            tpc.gravityIntesnity = 2f;
            tpc.acceleration = 0.25f;
            
        }
	}
}
