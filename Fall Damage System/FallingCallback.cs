using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORKFramework;

public class FallingCallback : MonoBehaviour {

    private ThirdPersonController tpc;
    public float timeInAir;
    private Combatant _combatant;
    private GameObject player;
    public GameObject Ragdoll;
    public int health;
	// Use this for initialization
	void Start () {
        tpc = gameObject.GetComponent<ThirdPersonController>();
        player = GameObject.FindWithTag("Player");
        _combatant = ComponentHelper.GetCombatant(player);
    }
	
	// Update is called once per frame
	void Update () {
        health = _combatant.Status[1].GetValue();
		if(!tpc.onGround && !tpc.onFence && !tpc.onLadder && !tpc.onLedge && !tpc.onWaterSurface && !tpc.underWater)
        {
            timeInAir += 1.5f * Time.deltaTime;
            
        }
        else if (tpc.onGround && timeInAir > 3 && _combatant.Dead == false)
        {
            StartCoroutine(GetDamage());
            timeInAir = 0;
        }

        
        else if (tpc.onGround && timeInAir < 5 && timeInAir > 0 || tpc.onFence || tpc.onLadder || tpc.onLedge ||tpc.inWater ||tpc.underWater)
        {
            StartCoroutine(Reset());
            //tpc.gravityIntesnity = 1;
            timeInAir = 0;
        }
    }

    public IEnumerator GetDamage()
    {
        GameObject.Find("YoungLink").GetComponent<Animator>().Play("Damage Landing", 0);
        tpc.isPaused = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        health -= 1;
        _combatant.Status[1].SetValue(health, false, true, false, true, true, false);
        yield return new WaitForSeconds(1f);
        timeInAir = 0;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        tpc.isPaused = false;
    }

    public IEnumerator Reset()
    {

        yield return null;
    }
}
