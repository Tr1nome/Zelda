/// <summary>
/// CURRENT VERSION: 2.0 (Nov '16)
/// This script was originally written by Yan Dawid of Zelig Games.
/// 
/// KEY (for the annotations in this script):
/// -The one referred to as 'User' is the person who uses/edits this asset
/// -The one referred to as 'Player' is the person who plays the project build
/// -The one referred to as 'Character' is the in-game character that the player controls
/// 
/// This script is to NOT be redistributed and can only be used by the person that has purchased this asset.
/// Editing or altering this script does NOT make redistributing this script valid.
/// This asset can be used in both personal and commercial projects.
/// The user is free to edit/alter this script to their hearts content.
/// You can contact the author for support via the Zelig Games official website: http://zeliggames.weebly.com/contact.html
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ToggleFeatures : MonoBehaviour {

    public GameObject parentButton;
	public GameObject controlsParent;
    private ThirdPersonController tpc;

    private GameObject curButton;

	bool firstPass;

    void Update () {
		if (!firstPass) {
			if (GameObject.FindWithTag ("Player").gameObject != null) {
				tpc = GameObject.FindWithTag ("Player").GetComponent<ThirdPersonController> ();
				Button0 ();
				Button0 ();
				Button1 ();
				Button1 ();
				Button2 ();
				Button2 ();
				Button3 ();
				Button3 ();
				Button4 ();
				Button4 ();
				Button5 ();
				Button5 ();
				Button6 ();
				Button6 ();
				Button7 ();
				Button7 ();
				Button8 ();
				Button8 ();
				Button9 ();
				Button9 ();
				Button10 ();
				Button10 ();
				Button11 ();
				Button11 ();
				Button12 ();
				Button12 ();
				Button13 ();
				Button13 ();
				Button14 ();
				Button14 ();
				Button15 ();
				Button15 ();
				Button16 ();
				Button16 ();
				Button17 ();
				Button17 ();
				Button18 ();
				Button18 ();
				Button19 ();
				Button19 ();
				Button20 ();
				Button20 ();
				Button21 ();
				Button21 ();
				Button22 ();
				Button22 ();
				Button23 ();
				Button23 ();
				Button24 ();
				Button24 ();
				Button25 ();
				Button25 ();
				Button26 ();
				Button26 ();
				Button27 ();
				Button27 ();
				Button28 ();
				Button28 ();
			}
			if (parentButton != null)
				parentButton.gameObject.SetActive (false);
			if (controlsParent != null)
				controlsParent.gameObject.SetActive (false);
			firstPass = true;
		}
	}

    public void ActiveButtons(){
        parentButton.gameObject.SetActive(!parentButton.gameObject.activeInHierarchy);
		if (this.GetComponent<Settings> ().parentButton.gameObject.activeInHierarchy)
			this.GetComponent<Settings> ().parentButton.gameObject.SetActive (false);
		if (controlsParent.gameObject.activeInHierarchy)
			controlsParent.gameObject.SetActive (false);
		PauseTime (parentButton);
    }

	public void ActiveControls(){
		controlsParent.gameObject.SetActive (!controlsParent.gameObject.activeInHierarchy);
		if (this.GetComponent<Settings> ().parentButton.gameObject.activeInHierarchy)
			this.GetComponent<Settings> ().parentButton.gameObject.SetActive (false);
		if (parentButton.gameObject.activeInHierarchy)
			parentButton.gameObject.SetActive (false);
		PauseTime (controlsParent);
	}

	public void PauseTime(GameObject active){
		if (active.gameObject.activeInHierarchy)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;
	}

    public void Button0(){
        tpc.sprintFunction = !tpc.sprintFunction;
        curButton = parentButton.gameObject.transform.Find("Button").gameObject;
        if (tpc.sprintFunction)
            curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button1(){
        tpc.rollFunction = !tpc.rollFunction;
        curButton = parentButton.gameObject.transform.Find("Button (1)").gameObject;
        if (tpc.rollFunction)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button2(){
        tpc.crouchFunction = !tpc.crouchFunction;
        curButton = parentButton.gameObject.transform.Find("Button (2)").gameObject;
        if (tpc.crouchFunction)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button3(){
        tpc.jumpFunction = !tpc.jumpFunction;
        curButton = parentButton.gameObject.transform.Find("Button (3)").gameObject;
        if (tpc.jumpFunction)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button4(){
        tpc.midAirMovement = !tpc.midAirMovement;
        curButton = parentButton.gameObject.transform.Find("Button (4)").gameObject;
        if (tpc.midAirMovement)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button5(){
        tpc.autoLedgeJumping = !tpc.autoLedgeJumping;
        curButton = parentButton.gameObject.transform.Find("Button (5)").gameObject;
        if (tpc.autoLedgeJumping)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button6(){
        tpc.onButtonJumping = !tpc.onButtonJumping;
        curButton = parentButton.gameObject.transform.Find("Button (6)").gameObject;
        if (tpc.onButtonJumping)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button7(){
        tpc.tripleGroundJumps = !tpc.tripleGroundJumps;
        curButton = parentButton.gameObject.transform.Find("Button (7)").gameObject;
        if (tpc.tripleGroundJumps)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button8(){
        tpc.sidewaysJump = !tpc.sidewaysJump;
        curButton = parentButton.gameObject.transform.Find("Button (8)").gameObject;
        if (tpc.sidewaysJump)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button9(){
        tpc.canJumpFromGrab = !tpc.canJumpFromGrab;
        curButton = parentButton.gameObject.transform.Find("Button (9)").gameObject;
        if (tpc.canJumpFromGrab)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button10(){
        tpc.canJumpFromClimb = !tpc.canJumpFromClimb;
        curButton = parentButton.gameObject.transform.Find("Button (10)").gameObject;
        if (tpc.canJumpFromClimb)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button11(){
        tpc.jumpCancelRoll = !tpc.jumpCancelRoll;
        curButton = parentButton.gameObject.transform.Find("Button (11)").gameObject;
        if (tpc.jumpCancelRoll)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button12(){
        tpc.ledgeGrabbing = !tpc.ledgeGrabbing;
        curButton = parentButton.gameObject.transform.Find("Button (12)").gameObject;
        if (tpc.ledgeGrabbing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button13(){
        tpc.ledgeClimbing = !tpc.ledgeClimbing;
        curButton = parentButton.gameObject.transform.Find("Button (13)").gameObject;
        if (tpc.ledgeClimbing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button14(){
        tpc.interactWithWater = !tpc.interactWithWater;
        curButton = parentButton.gameObject.transform.Find("Button (14)").gameObject;
        if (tpc.interactWithWater)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button15(){
        tpc.canDiveIfNoSwim = !tpc.canDiveIfNoSwim;
        curButton = parentButton.gameObject.transform.Find("Button (15)").gameObject;
        if (tpc.canDiveIfNoSwim)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button16(){
        tpc.ladderClimbing = !tpc.ladderClimbing;
        curButton = parentButton.gameObject.transform.Find("Button (16)").gameObject;
        if (tpc.ladderClimbing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button17(){
        tpc.fenceClimbing = !tpc.fenceClimbing;
        curButton = parentButton.gameObject.transform.Find("Button (17)").gameObject;
        if (tpc.fenceClimbing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button18(){
        tpc.boxPushing = !tpc.boxPushing;
        curButton = parentButton.gameObject.transform.Find("Button (18)").gameObject;
        if (tpc.boxPushing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button19(){
        tpc.oldBoxPushing = !tpc.oldBoxPushing;
        curButton = parentButton.gameObject.transform.Find("Button (19)").gameObject;
        if (tpc.oldBoxPushing)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button20(){
        tpc.onlyRollInTargetMode = !tpc.onlyRollInTargetMode;
        curButton = parentButton.gameObject.transform.Find("Button (20)").gameObject;
        if (tpc.onlyRollInTargetMode)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button21(){
        tpc.freeCamera = !tpc.freeCamera;
        curButton = parentButton.gameObject.transform.Find("Button (21)").gameObject;
        if (tpc.freeCamera)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button22(){
        tpc.targetCamera = !tpc.targetCamera;
        curButton = parentButton.gameObject.transform.Find("Button (22)").gameObject;
        if (tpc.targetCamera)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button23(){
        tpc.focusOnTargets = !tpc.focusOnTargets;
        curButton = parentButton.gameObject.transform.Find("Button (23)").gameObject;
        if (tpc.focusOnTargets)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button24(){
        tpc.fpvCamera = !tpc.fpvCamera;
        curButton = parentButton.gameObject.transform.Find("Button (24)").gameObject;
        if (tpc.fpvCamera)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button25(){
        tpc.levitateWithSelect = !tpc.levitateWithSelect;
        curButton = parentButton.gameObject.transform.Find("Button (25)").gameObject;
        if (tpc.levitateWithSelect)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

    public void Button26(){
        tpc.canSwim = !tpc.canSwim;
        curButton = parentButton.gameObject.transform.Find("Button (26)").gameObject;
        if (tpc.canSwim)
           curButton.GetComponent<Button>().image.color = Color.green;
        else
           curButton.GetComponent<Button>().image.color = Color.red;
    }

	public void Button27(){
		tpc.cam.isometricMode = !tpc.cam.isometricMode;
		curButton = parentButton.gameObject.transform.Find("Button (27)").gameObject;
		if (tpc.cam.isometricMode)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button28(){
		tpc.onButtonLedgeClimb = !tpc.onButtonLedgeClimb;
		curButton = parentButton.gameObject.transform.Find("Button (28)").gameObject;
		if (tpc.onButtonLedgeClimb)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}
}
