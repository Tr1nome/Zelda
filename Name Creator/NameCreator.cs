using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ORKFramework;
using ORKFramework.Behaviours;

public class NameCreator : MonoBehaviour {

    public InputField input;
    private string lastLetter;
    public string PlayerName;
    private AudioSource aud;
    public AudioClip letterClip;
    public AudioClip confirmClip;
    public AudioClip cancelClip;
    public SceneChanger sceneChanger;
    public void Start()
    {
        aud = GetComponent<AudioSource>();
    }
    public void SetLetter(string letter)
    {
        StartCoroutine(PrintLetter(letter));
        
    }
    public void Update()
    {
        PlayerName = input.text;
        if(Input.GetButtonDown("Cancel"))
        {
            RemoveLetter();
        }
    }
    public IEnumerator PrintLetter(string letter)
    {
        input.text += letter;
        aud.PlayOneShot(letterClip);
        yield return new WaitForSeconds(1f);
        lastLetter = letter;
        //letter = "";
    }

    public void RemoveLetter()
    {
        aud.PlayOneShot(cancelClip);
        input.text = input.text.Substring(0, input.text.Length - 1);
    }

    public void SetPlayerName()
    {
        ORK.Game.Variables.Set("playerName", PlayerName);
        aud.PlayOneShot(letterClip);
        sceneChanger.enabled = true;
    }
}
