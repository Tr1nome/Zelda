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
using System.Collections;

public class GamePadInputs : MonoBehaviour {

	public bool triggersAreButtons;					//Are the triggers on the gamepad buttons (as opposed to Axes)
	[HideInInspector] public bool pressAction;		//is the Action Button PRESSED?
	[HideInInspector] public bool holdAction;		//is the Action Button HELD?
	[HideInInspector] public bool pressRightB;
	[HideInInspector] public bool holdRightB;
	[HideInInspector] public bool pressLeftB;
	[HideInInspector] public bool holdLeftB;
	[HideInInspector] public bool pressTopB;
	[HideInInspector] public bool holdTopB;
	[HideInInspector] public bool pressLBump;
	[HideInInspector] public bool holdLBump;
	[HideInInspector] public bool pressRBump;
	[HideInInspector] public bool holdRBump;
	[HideInInspector] public bool pressSelect;
	[HideInInspector] public bool holdSelect;
	[HideInInspector] public bool pressStart;
	[HideInInspector] public bool holdStart;
	[HideInInspector] public bool pressLStick;
	[HideInInspector] public bool holdLStick;
	[HideInInspector] public bool pressRStick;
	[HideInInspector] public bool holdRStick;
	[HideInInspector] public float DH;				//The Horizontal Axis for the D-Pad
	[HideInInspector] public float DV;				//The Vertical Axis for the D-Pad
	[HideInInspector] public float RH;				//The Horizontal Axis for the RIGHT analoge stick
	[HideInInspector] public float RV;				//The Vertical Axis for the RIGHT analoge stick
	[HideInInspector] public float LH;				//The Horizontal Axis for the LEFT analoge stick
	[HideInInspector] public float LV;				//The Vertical Axis for the LEFT analoge stick
	[HideInInspector] public float LT;				//The Axis for the LEFT Trigger
	[HideInInspector] public float RT;				//The Axis for the RIGHT Trigger

	void Update () {
		pressAction = Input.GetButtonDown ("ActionButton");
		holdAction = Input.GetButton ("ActionButton");
		pressLeftB = Input.GetButtonDown ("LeftButton");
		holdLeftB = Input.GetButton ("LeftButton");
		pressRightB = Input.GetButtonDown ("RightButton");
		holdRightB = Input.GetButton ("RightButton");
		pressTopB = Input.GetButtonDown ("TopButton");
		holdTopB = Input.GetButton ("TopButton");
		pressLBump = Input.GetButtonDown ("LBump");
		holdLBump = Input.GetButton ("LBump");
		pressRBump = Input.GetButtonDown ("RBump");
		holdRBump = Input.GetButton ("RBump");
		pressStart = Input.GetButtonDown ("Start");
		holdStart = Input.GetButton ("Start");
		pressSelect = Input.GetButtonDown ("Select");
		holdSelect = Input.GetButton ("Select");
		pressLStick = Input.GetButtonDown ("LStickPress");
		holdLStick = Input.GetButton ("LStickPress");
		pressRStick = Input.GetButtonDown ("RStickPress");
		holdRStick = Input.GetButton ("RStickPress");
		
		LH = Input.GetAxis ("Horizontal");
		LV = Input.GetAxis ("Vertical");

		if (Input.GetAxis ("DH") != 0f)
			DH = Input.GetAxis ("DH");
		else {
			if(Input.GetKey(KeyCode.L))
				DH = 1f;
			else{
				if(Input.GetKey(KeyCode.J))
					DH = -1f;
				else
					DH = 0f;
			}
		}
		if (Input.GetAxis ("DV") != 0f)
			DV = Input.GetAxis ("DV");
		else {
			if(Input.GetKey(KeyCode.I))
				DV = 1f;
			else{
				if(Input.GetKey(KeyCode.K))
					DV = -1f;
				else
					DV = 0f;
			}
		}

		if (Input.GetAxis ("ZHorizontal") != 0f)
			RH = Input.GetAxis ("ZHorizontal");
		else {
			RH = Input.GetAxis ("Mouse X");
		}
		if (Input.GetAxis ("ZVertical") != 0f)
			RV = Input.GetAxis ("ZVertical");
		else {
			RV = Input.GetAxis ("Mouse Y");
		}

		if (triggersAreButtons) {
			if (Input.GetButton ("LTrig"))
				LT = Input.GetAxis ("LTrig");
			else {
				if (Input.GetMouseButton (1))
					LT = 1f;
				else
					LT = 0f;
			}
			if (Input.GetButton ("RTrig"))
				RT = Input.GetAxis ("RTrig");
			else {
				if (Input.GetKey (KeyCode.X))
					RT = 1f;
				else
					RT = 0f;
			}
		} else {
			if (Input.GetAxis ("LTrig") > 0f)
				LT = Input.GetAxis ("LTrig");
			else {
				if (Input.GetMouseButton (1))
					LT = 1f;
				else
					LT = 0f;
			}
			if (Input.GetAxis ("RTrig") > 0f)
				RT = Input.GetAxis ("RTrig");
			else {
				if (Input.GetKey (KeyCode.X))
					RT = 1f;
				else
					RT = 0f;
			}
		}
	}
}
