using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UniStormHUDWrapper : MonoBehaviour {

    private UniStormSystem UniStorm;
    [Header("UniStorm Attributes")]
    public Text Hour;
    public Text Minute;
    public Text Day;
    public Text Month;
    public Text Year;

	// Use this for initialization
	void Start () {
        UniStorm = GameObject.FindWithTag("Unistorm").GetComponent<UniStormSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        Hour.text = UniStorm.Hour.ToString("00");
        Minute.text = UniStorm.Minute.ToString("00");
        Day.text = UniStorm.Day.ToString("00");
        Month.text = UniStorm.Month.ToString("00");
        Year.text = UniStorm.Year.ToString();
	}
}
