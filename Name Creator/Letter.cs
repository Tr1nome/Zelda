using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Letter : MonoBehaviour, ISelectHandler {

    private Text text;
    private AudioSource aud;
    public AudioClip clip;
	// Use this for initialization
	void Start () {
        text = GetComponentInChildren<Text>();
        aud = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = gameObject.name;
	}

    public void OnSelect( BaseEventData eventData)
    {
        aud.PlayOneShot(clip);
    }
}
