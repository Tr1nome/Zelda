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
using UnityStandardAssets.ImageEffects;

public class Settings : MonoBehaviour {

	public GameObject gSpeed;
	public GameObject sSpeed;
	public GameObject gravIntsty;
	public GameObject gAccel;
	public GameObject rSpeed;
	public GameObject hCSpeed;
	public GameObject vCSpeed;
	public GameObject hMSpeed;
	public GameObject cSpeed;
	public GameObject airMo;
	public GameObject airMoveSns;
	public GameObject wlkSTol;
	public GameObject throwForce;
	public GameObject amtOfJumps;
	public GameObject dfltJumpIntsty;
	public GameObject autoLdgJump;
	public GameObject sideJumpIntsty;
	public GameObject crchJumpIntsty;
	public GameObject divingSpd;
	public GameObject fstSwimSpd;
	public GameObject slwSwimSpd;
	public GameObject camDAway;
	public GameObject camDUp;
	public GameObject camMxDA;
	public GameObject freeCamRX;
	public GameObject freeCamRY;
	public GameObject freeCamDX;
	public GameObject freeCamDY;
	public GameObject camWskSns;
	public GameObject swimRSpd;
	public GameObject isoCamX;
	public GameObject camSpdDmp;

	public GameObject parentButton;
	private ThirdPersonController tpc;
	private CameraFollower camF;

	private GameObject curButton;

	BloomOptimized bloomCS;
	SunShafts shaftsCS;
	DepthOfField dofCS;

	bool firstPass;

	void Update () {
		if (!firstPass) {		
			if (GameObject.FindWithTag ("Player").gameObject != null) {
				tpc = GameObject.FindWithTag ("Player").GetComponent<ThirdPersonController> ();
				camF = tpc.cam;
				if (camF.gameObject.GetComponent<SunShafts> () != null)
					shaftsCS = camF.gameObject.GetComponent<SunShafts> ();
				if (camF.gameObject.GetComponent<DepthOfField> () != null)
					dofCS = camF.gameObject.GetComponent<DepthOfField> ();
				if (camF.gameObject.GetComponent<BloomOptimized> () != null)
					bloomCS = camF.gameObject.GetComponent<BloomOptimized> ();

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
				if (shaftsCS != null) {
					Button44 ();
					Button44 ();
				}
				if (dofCS != null) {
					Button45 ();
					Button45 ();
				}
				if (bloomCS != null) {
					Button46 ();
					Button46 ();
				}

				gSpeed.GetComponent<InputField> ().text = tpc.maxSpeed.ToString ();
				sSpeed.GetComponent<InputField> ().text = tpc.additionalSprintSpeed.ToString ();
				gravIntsty.GetComponent<InputField> ().text = tpc.gravityIntesnity.ToString ();
				gAccel.GetComponent<InputField> ().text = tpc.acceleration.ToString ();
				rSpeed.GetComponent<InputField> ().text = tpc.rollSpeed.ToString ();
				hCSpeed.GetComponent<InputField> ().text = tpc.horizontalClimbSpeed.ToString ();
				vCSpeed.GetComponent<InputField> ().text = tpc.verticalClimbSpeed.ToString ();
				hMSpeed.GetComponent<InputField> ().text = tpc.hangingMoveSpeed.ToString ();
				cSpeed.GetComponent<InputField> ().text = tpc.crawlSpeed.ToString ();
				airMo.GetComponent<InputField> ().text = tpc.airMomentum.ToString ();
				airMoveSns.GetComponent<InputField> ().text = tpc.airMoveSensitivity.ToString ();
				wlkSTol.GetComponent<InputField> ().text = tpc.walkableSlopeTolerence.ToString ();
				throwForce.GetComponent<InputField> ().text = tpc.throwForce.ToString ();
				amtOfJumps.GetComponent<InputField> ().text = tpc.amountOfJumps.ToString ();
				dfltJumpIntsty.GetComponent<InputField> ().text = tpc.jumpForces [3].forceAmount.ToString ();
				autoLdgJump.GetComponent<InputField> ().text = tpc.jumpForces [0].forceAmount.ToString ();
				sideJumpIntsty.GetComponent<InputField> ().text = tpc.jumpForces [1].forceAmount.ToString ();
				crchJumpIntsty.GetComponent<InputField> ().text = tpc.jumpForces [2].forceAmount.ToString ();
				divingSpd.GetComponent<InputField> ().text = tpc.divingForce.ToString ();
				fstSwimSpd.GetComponent<InputField> ().text = tpc.swimmingForce.ToString ();
				slwSwimSpd.GetComponent<InputField> ().text = tpc.swimSpeed.ToString ();
				camDAway.GetComponent<InputField> ().text = camF.distanceAway.ToString ();
				camDUp.GetComponent<InputField> ().text = camF.distanceUp.ToString ();
				camMxDA.GetComponent<InputField> ().text = camF.maxDistanceAway.ToString ();
				freeCamRX.GetComponent<InputField> ().text = camF.freeRotateSpeed.ToString ();
				freeCamRY.GetComponent<InputField> ().text = camF.yFreeSpeed.ToString ();
				freeCamDX.GetComponent<InputField> ().text = camF.freecamXDamp.ToString ();
				freeCamDY.GetComponent<InputField> ().text = camF.freecamYDamp.ToString ();
				camWskSns.GetComponent<InputField> ().text = camF.whiskeringSensitivity.ToString ();
				swimRSpd.GetComponent<InputField> ().text = tpc.swimmingRotateSpeed.ToString ();
				isoCamX.GetComponent<InputField> ().text = camF.isometricCameraAngle.ToString ();
				camSpdDmp.GetComponent<InputField> ().text = camF.cameraSpeedDamp.ToString ();
			}
			if (parentButton != null)
				parentButton.gameObject.SetActive (false);
			firstPass = true;
		}
	}

	public void Toggle(){
		parentButton.gameObject.SetActive (!parentButton.gameObject.activeInHierarchy);
		if (this.GetComponent<ToggleFeatures> ().parentButton.gameObject.activeInHierarchy)
			this.GetComponent<ToggleFeatures> ().parentButton.gameObject.SetActive (false);
		if (this.GetComponent<ToggleFeatures> ().controlsParent.gameObject.activeInHierarchy)
			this.GetComponent<ToggleFeatures> ().controlsParent.gameObject.SetActive (false);
		if (parentButton.gameObject.activeInHierarchy)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;
	}

	public void Button0(){
		tpc.useBlobShadow = !tpc.useBlobShadow;
		curButton = parentButton.gameObject.transform.Find("Button").gameObject;
		if (tpc.useBlobShadow)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button1(){
		tpc.invertYSwim = !tpc.invertYSwim;
		curButton = parentButton.gameObject.transform.Find("Button (1)").gameObject;
		if (tpc.invertYSwim)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button2(){
		tpc.invertXSwim = !tpc.invertXSwim;
		curButton = parentButton.gameObject.transform.Find("Button (2)").gameObject;
		if (tpc.invertXSwim)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button3(){
		tpc.targetHoldMode = !tpc.targetHoldMode;
		curButton = parentButton.gameObject.transform.Find("Button (3)").gameObject;
		if (tpc.targetHoldMode)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button4(){
		camF.fpvInvertYAxis = !camF.fpvInvertYAxis;
		curButton = parentButton.gameObject.transform.Find("Button (4)").gameObject;
		if (camF.fpvInvertYAxis)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button5(){
		camF.freeInvertYAxis = !camF.freeInvertYAxis;
		curButton = parentButton.gameObject.transform.Find("Button (5)").gameObject;
		if (camF.freeInvertYAxis)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button6(){
		camF.freeInvertXAxis = !camF.freeInvertXAxis;
		curButton = parentButton.gameObject.transform.Find("Button (6)").gameObject;
		if (camF.freeInvertXAxis)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button7(){
		camF.whiskeringFunction = !camF.whiskeringFunction;
		curButton = parentButton.gameObject.transform.Find("Button (7)").gameObject;
		if (camF.whiskeringFunction)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button8(){
		camF.autoBehindPlayer = !camF.autoBehindPlayer;
		curButton = parentButton.gameObject.transform.Find("Button (8)").gameObject;
		if (camF.autoBehindPlayer)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button9(){
		camF.hillAdjusting = !camF.hillAdjusting;
		curButton = parentButton.gameObject.transform.Find("Button (9)").gameObject;
		if (camF.hillAdjusting)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button10(){
		camF.altUnderWaterTargetMode = !camF.altUnderWaterTargetMode;
		curButton = parentButton.gameObject.transform.Find("Button (10)").gameObject;
		if (camF.altUnderWaterTargetMode)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button44(){
		shaftsCS.enabled = !shaftsCS.enabled;
		curButton = parentButton.gameObject.transform.Find("Button (44)").gameObject;
		if (shaftsCS.enabled)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button45(){
		dofCS.enabled = !dofCS.enabled;
		curButton = parentButton.gameObject.transform.Find("Button (45)").gameObject;
		if (dofCS.enabled)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button46(){
		bloomCS.enabled = !bloomCS.enabled;
		curButton = parentButton.gameObject.transform.Find("Button (46)").gameObject;
		if (bloomCS.enabled)
			curButton.GetComponent<Button>().image.color = Color.green;
		else
			curButton.GetComponent<Button>().image.color = Color.red;
	}

	public void Button11(){
		tpc.maxSpeed = float.Parse(gSpeed.GetComponent<InputField>().text);
		tpc.originalMoveSpeed = tpc.maxSpeed;
	}

	public void Button12(){
		tpc.additionalSprintSpeed = float.Parse(sSpeed.GetComponent<InputField>().text);
	}

	public void Button13(){
		tpc.gravityIntesnity = float.Parse(gravIntsty.GetComponent<InputField>().text);
	}

	public void Button14(){
		tpc.acceleration = float.Parse(gAccel.GetComponent<InputField>().text);
	}

	public void Button15(){
		tpc.rollSpeed = float.Parse(rSpeed.GetComponent<InputField>().text);
	}

	public void Button16(){
		tpc.horizontalClimbSpeed = float.Parse(hCSpeed.GetComponent<InputField>().text);
	}

	public void Button17(){
		tpc.verticalClimbSpeed = float.Parse(vCSpeed.GetComponent<InputField>().text);
	}

	public void Button18(){
		tpc.hangingMoveSpeed = float.Parse(hMSpeed.GetComponent<InputField>().text);
	}

	public void Button19(){
		tpc.crawlSpeed = float.Parse(cSpeed.GetComponent<InputField>().text);
	}

	public void Button20(){
		tpc.airMomentum = float.Parse(airMo.GetComponent<InputField>().text);
	}

	public void Button21(){
		tpc.airMoveSensitivity = float.Parse(airMoveSns.GetComponent<InputField>().text);
	}

	public void Button22(){
		tpc.walkableSlopeTolerence = float.Parse(wlkSTol.GetComponent<InputField>().text);
	}

	public void Button23(){
		tpc.throwForce = float.Parse(throwForce.GetComponent<InputField>().text);
	}

	public void Button24(){
		tpc.amountOfJumps = int.Parse(amtOfJumps.GetComponent<InputField>().text);
	}

	public void Button25(){
		tpc.jumpForces[3].forceAmount = float.Parse(dfltJumpIntsty.GetComponent<InputField>().text);
	}

	public void Button26(){
		tpc.jumpForces[0].forceAmount = float.Parse(autoLdgJump.GetComponent<InputField>().text);
	}

	public void Button27(){
		tpc.jumpForces[1].forceAmount = float.Parse(sideJumpIntsty.GetComponent<InputField>().text);
	}

	public void Button28(){
		tpc.jumpForces[2].forceAmount = float.Parse(crchJumpIntsty.GetComponent<InputField>().text);
	}

	public void Button29(){
		tpc.divingForce = float.Parse(divingSpd.GetComponent<InputField>().text);
	}

	public void Button30(){
		tpc.swimmingForce = float.Parse(fstSwimSpd.GetComponent<InputField>().text);
	}

	public void Button31(){
		tpc.swimSpeed = float.Parse(slwSwimSpd.GetComponent<InputField>().text);
	}

	public void Button32(){
		camF.distanceAway = float.Parse(camDAway.GetComponent<InputField>().text);
		camF.origDA = camF.distanceAway;
	}

	public void Button33(){
		camF.distanceUp = float.Parse(camDUp.GetComponent<InputField>().text);
		camF.origDU = camF.distanceUp;
	}

	public void Button34(){
		camF.maxDistanceAway = float.Parse(camMxDA.GetComponent<InputField>().text);
	}

	public void Button35(){
		camF.freeRotateSpeed = float.Parse(freeCamRX.GetComponent<InputField>().text);
	}

	public void Button36(){
		camF.yFreeSpeed = float.Parse(freeCamRY.GetComponent<InputField>().text);
	}

	public void Button37(){
		camF.freecamXDamp = float.Parse(freeCamDX.GetComponent<InputField>().text);
	}

	public void Button38(){
		camF.freecamYDamp = float.Parse(freeCamDY.GetComponent<InputField>().text);
	}

	public void Button39(){
		camF.whiskeringSensitivity = float.Parse(camWskSns.GetComponent<InputField>().text);
	}

	public void Button40(){
		tpc.swimmingRotateSpeed = float.Parse(swimRSpd.GetComponent<InputField>().text);
	}

	public void Button42(){
		camF.isometricCameraAngle = float.Parse (isoCamX.GetComponent<InputField> ().text);
	}

	public void Button43(){
		camF.cameraSpeedDamp = float.Parse (camSpdDmp.GetComponent<InputField> ().text);
	}
}
