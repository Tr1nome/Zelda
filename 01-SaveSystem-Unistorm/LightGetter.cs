using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);

    }
	
	// Update is called once per frame
	void Update () {
        RenderSettings.sun = GameObject.Find("UniStorm Sun").GetComponent<Light>();
	}
}
