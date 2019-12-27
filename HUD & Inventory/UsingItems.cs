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
using System.Collections.Generic;

public class UsingItems : MonoBehaviour {

	public int rightHandAnimatorLayerID;			//The animation layer of the right hand of the character that is animated (as a mask) when handling an item
	public int leftHandAnimatorLayerID;				//The animation layer of the left hand of the character that is animated (as a mask) when handling an item
	private bool hasSetLayerWeight;
	private float lerpTo;

	[HideInInspector] public Item currentItem;		//The item the character is currently using
	[HideInInspector] public Item rightHandItem;	//The item the character is holding in their right hand
	[HideInInspector] public Item leftHandItem;		//The item the character is holding in their left hand
	[HideInInspector] public HUD hudScript;
	
	[HideInInspector] public GamePadInputs gPI;
	[HideInInspector] public Animations currentAnims;
	[HideInInspector] public bool buttonBeingHeld;
	private string itemBeingHeld;
	[HideInInspector] public ThirdPersonController character;
	[HideInInspector] public Camera cam;
	private CameraFollower camF;
	[HideInInspector] public int hotkeysAmount;
	[HideInInspector] public Dictionary<int, GameObject> modelObjPool = new Dictionary<int, GameObject>();
	[HideInInspector] public GameObject poolObj;
	[HideInInspector] public GameObject poolProjectile;

	[HideInInspector] public bool canUseItems;
	[HideInInspector] public bool isUnequipping;

	private Dictionary<int, GameObject> arrowsPool = new Dictionary<int, GameObject>();
	private Dictionary<int, GameObject> bombPool = new Dictionary<int, GameObject>();
	[HideInInspector] public Dictionary<int, GameObject> explosionPool = new Dictionary<int, GameObject>();
	private float bowAngle;

	void Start(){
		currentItem = null;
		rightHandItem = null;
		leftHandItem = null;
		buttonBeingHeld = false;
		itemBeingHeld = "";
		cam = Camera.main;
		camF = cam.GetComponent<CameraFollower> ();
		hudScript = this.GetComponent<HUD> ();
		gPI = this.GetComponent<GamePadInputs> ();
		currentAnims = this.GetComponent<Animations> ();
		canUseItems = true;
		lerpTo = 1f;
	}

	void Update(){
	////Set miscelaneous settings here for while certain items are equipped

		/** BOW **/
		if (character.anim.GetBool ("useBow")) {
			if(character.canFPV)
				character.canFPV = false;
			if(camF.fpvMode){
				bowAngle = cam.transform.localEulerAngles.x + 90f;
				if(bowAngle > 180f)
					bowAngle -= 360f;
                if (character.fPVing == false)
                    character.fPVing = true;
			}
			if(camF.targetMode){
                if (character.fPVing)
                    character.fPVing = false;
				if(character.currentTarget != null){
                    bowAngle = Vector3.Angle(character.currentTarget.transform.position - character.transform.position, Vector3.up);
					if(bowAngle > 180f)
						bowAngle -= 360f;
				}else
					bowAngle = 90f;
                if (bowAngle > 165f)
                    bowAngle = 165f;
                if (bowAngle < 15f)
                    bowAngle = 15f;
			}
			character.anim.SetFloat ("bowAngle", bowAngle);
			if(camF.behindMode || camF.freeMode){
				camF.behindMode = false;
                camF.freeMode = false;
			}
			if(!camF.targetMode && !camF.fpvMode){
				character.fPVing = true;
				camF.fpvMode = true;
			}
			if(camF.fpvMode && !hudScript.aimGUIElement.gameObject.activeSelf)
				ToggleAimGUIElement(true);
			if(!camF.fpvMode && hudScript.aimGUIElement.gameObject.activeSelf)
				ToggleAimGUIElement(false);
		}
		if(!character.anim.GetBool("useBow") && hudScript.aimGUIElement.gameObject.activeSelf)
			ToggleAimGUIElement(false);
		/** BOW **/

	////Unequipping
		if (gPI.LH == 0f && gPI.LV == 0f && gPI.pressAction && !currentAnims.inALiftingAnimation) {
			if(rightHandItem != null || leftHandItem != null){
				isUnequipping = true;
				character.anim.SetTrigger("reach");
				currentItem = null;
				
			}
		}

	////Animation Override
		if (currentAnims.inALedgeClimbAnim || currentAnims.inAClimbingAnim || currentAnims.inASwimmingAnimation || currentAnims.grabbingBlend ||
		    currentAnims.inABoxPushingAnim || currentAnims.inATargJumpAnim ||
		    character.onLedge || character.grabbing) {
			if(!hasSetLayerWeight){
				hasSetLayerWeight = true;
				lerpTo = 0f;
			}
		}else{
			if(hasSetLayerWeight){
				hasSetLayerWeight = false;
				lerpTo = 1f;
			}
		}

		if(Mathf.Abs(currentAnims.anim.GetLayerWeight (rightHandAnimatorLayerID) - lerpTo) > 0.1f){
			currentAnims.anim.SetLayerWeight (rightHandAnimatorLayerID, Mathf.Lerp(currentAnims.anim.GetLayerWeight (rightHandAnimatorLayerID), lerpTo, Time.deltaTime*5f));
			currentAnims.anim.SetLayerWeight (leftHandAnimatorLayerID, Mathf.Lerp(currentAnims.anim.GetLayerWeight (leftHandAnimatorLayerID), lerpTo, Time.deltaTime*5f));
		}else{
			if(currentAnims.anim.GetLayerWeight (rightHandAnimatorLayerID) != lerpTo){
				currentAnims.anim.SetLayerWeight (rightHandAnimatorLayerID, lerpTo);
				currentAnims.anim.SetLayerWeight (leftHandAnimatorLayerID, lerpTo);
			}
		}

	////Restrictions
		if (character.onLedge || character.grabbing || character.climbing || character.inWater || character.isCrouching || character.inACutscene) {
			if(canUseItems){
				canUseItems = false;
				currentItem = null;
				
			}
		}else{
			if(!canUseItems)
				canUseItems = true;
		}

	}

/// <summary>
/// Equips the item.
/// </summary>
	public void EquipItem(GameObject item){
		Item itemI = item.GetComponent<Item> ();
		if (currentItem != item) {
			if(currentItem == null || currentItem!= itemI){
				if(currentItem != null && character.isLifting)
					character.liftAction = true;
				if(itemI.dualWield){
					if(rightHandItem == null && leftHandItem == null ||
					   itemI.rightHandWield && rightHandItem == null && leftHandItem != null && leftHandItem.dualWield && leftHandItem.dualWieldItemName == itemI.itemName ||
					   itemI.rightHandWield && rightHandItem == null && leftHandItem != null && !leftHandItem.dualWield ||
					   !itemI.rightHandWield && leftHandItem == null && rightHandItem != null && rightHandItem.dualWield && rightHandItem.dualWieldItemName == itemI.itemName ||
					   !itemI.rightHandWield && leftHandItem == null && rightHandItem != null && !rightHandItem.dualWield ||
					   itemI.rightHandWield && rightHandItem != null && rightHandItem != itemI ||
					   !itemI.rightHandWield && leftHandItem != null && leftHandItem != itemI){
						currentItem = itemI;
						character.anim.SetTrigger("reach");
						
					}
				}else{
					currentItem = itemI;
					character.anim.SetTrigger("reach");
					
				}
				ToggleAimGUIElement(false);
			}
		}
	}

/// <summary>
/// Uses the item of the pressed hotkey
/// </summary>
	public void UseItemPress(Item item){
	////CUSTOM ITEM CODE GOES HERE - COMPARE 'item.itemName' TO AN ITEM NAME e.g. if(item.itemName == "Sword") ////
		if (item.itemName == "Sword") {
			character.anim.SetTrigger("swingSword");
		}
		if (item.itemName == "Bow") {
			if(!character.anim.GetBool("useBow")){
				character.anim.SetBool("useBow", true);
			}else{
				if(camF.fpvMode && item.quantity > 0){
					CheckProjectilePool(arrowsPool, "Arrows");
					GameObject arrow;
					if(poolProjectile == null){
						arrow = Instantiate(item.projectileModel, cam.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
						arrow.name = item.projectileModel.name;
						arrowsPool.Add(arrowsPool.Count, arrow);
					}else{
						arrow = poolProjectile;
						arrow.transform.position = cam.transform.position;
					}
					arrow.transform.forward = cam.transform.forward;
					arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * 20f;
					
				}
				if(camF.targetMode && item.quantity > 0){
					CheckModelPool(item);
					CheckProjectilePool(arrowsPool, "Arrows");
					GameObject arrow;
					if(poolProjectile == null){
						arrow = Instantiate(item.projectileModel, poolObj.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
						arrow.name = item.projectileModel.name;
						arrowsPool.Add(arrowsPool.Count, arrow);
					}else{
						arrow = poolProjectile;
						arrow.transform.position = poolObj.transform.position;
					}
					if(character.currentTarget != null)
						arrow.transform.LookAt(character.currentTarget.transform.position);
					else
						arrow.transform.forward = new Vector3(character.transform.forward.x, 0f, character.transform.forward.z);
					arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * 20f;
					
				}
			}
		}
		if (item.itemName == "Bombs")
			character.liftAction = true;

		//**TEMPLATE**//
		/*
		if (item.itemName == "XXX"){
			DO THIS WHEN THE BUTTON FOR THE ITEM HAS BEEN PRESSED
		}
		*/
	}

/// <summary>
/// Uses the item of the held-down hotkey
/// </summary>
	public void UseItemHold(Item item){
		////CUSTOM ITEM CODE GOES HERE - COMPARE 'item.itemName' TO AN ITEM NAME e.g. if(item.itemName == "Sword") ////
		if (item.itemName == "Sword") {
			if(!character.anim.GetBool("chargeSword")){
				character.anim.SetBool("chargeSword", true);
				itemBeingHeld = item.itemName;
			}
		}
		if (item.itemName == "Shield") {
			if(!character.anim.GetBool("defending")){
				character.anim.SetBool("defending", true);
				itemBeingHeld = item.itemName;
			}
		}
		//**TEMPLATE**//
		/*
		if (item.itemName == "XXX"){
			DO THIS WHEN THE BUTTON FOR THE ITEM IS BEING HELD DOWN
		}
		*/
	}

/// <summary>
/// Uses the item of the hotkey that was held down and has now been released
/// </summary>
	public void UseItemRelease(){
		if (itemBeingHeld == "Sword") {
			if(character.anim.GetBool("chargeSword"))
				character.anim.SetBool("chargeSword", false);
		}
		if (itemBeingHeld == "Shield") {
			if(character.anim.GetBool("defending"))
				character.anim.SetBool("defending", false);
		}

		//**TEMPLATE**//
		/*
		if (item.itemName == "XXX"){
			DO THIS WHEN THE BUTTON FOR THE ITEM HAS BEEN RELEASED
		}
		*/
	}


/// <summary>
/// The function that gets item models from an object pool
/// </summary>
	public void CheckModelPool(Item item){
		int p = 0;
		bool found = false;
		while (p < modelObjPool.Count && modelObjPool.Count > 0 && !found) {
			if(modelObjPool[p] != null){
				if(modelObjPool[p].name == item.itemName + " Model"){
					found = true;
					poolObj = modelObjPool[p].gameObject;
				}
			}
			p++;
		}
		if (!found)
			poolObj = null;
	}

/// <summary>
/// The function that gets projectiles from an object pool
/// </summary>
	public void CheckProjectilePool(Dictionary<int,GameObject> objectPool, string projectileName){
		int p = 0;
		bool found = false;
		while (p < objectPool.Count && objectPool.Count > 0 && !found) {
			if(objectPool[p] != null){
				if(objectPool[p].name == projectileName || objectPool[p].name == projectileName + "(Clone)"){
					if(!objectPool[p].activeSelf){
						found = true;
						objectPool[p].gameObject.SetActive(true);
						poolProjectile = objectPool[p].gameObject;
					}
				}
			}
			p++;
		}
		if (!found)
			poolProjectile = null;
	}

/// <summary>
/// The function that puts items in the characters right hand
/// </summary>
	void PutItemInRightHand(Item item){
		rightHandItem = item;
		GameObject rObj = null;
		CheckModelPool (item);
		rObj = poolObj;
		if (!item.displayModelOnChar) {
			CheckModelPool(item);
			rObj = poolObj;
			if(rObj == null){
				rObj = Instantiate (item.itemModel, character.characterRightHand.transform.position, character.characterRightHand.transform.rotation) as GameObject;
				rObj.name = item.itemName + " Model";
				modelObjPool.Add(modelObjPool.Count, rObj);
			}else
				rObj.SetActive(true);
		}
		rObj.transform.position = character.characterRightHand.transform.position;
		rObj.transform.rotation = character.characterRightHand.transform.rotation;
		rObj.transform.SetParent (character.characterRightHand.transform);
		rObj.transform.localPosition = item.modelCentrePos;
		rObj.transform.localRotation = Quaternion.Euler (item.modelRotations);
		character.anim.SetBool ("equippedRight", true);
	}

/// <summary>
/// The function that puts items in the characters left hand
/// </summary>
	void PutItemInLeftHand(Item item){
		leftHandItem = item;
		GameObject lObj = null;
		CheckModelPool (item);
		lObj = poolObj;
		if (!item.displayModelOnChar) {
			if (lObj == null) {
				lObj = Instantiate (item.itemModel, character.characterLeftHand.transform.position, character.characterLeftHand.transform.rotation) as GameObject;
				lObj.name = item.itemName + " Model";
				modelObjPool.Add (modelObjPool.Count, lObj);
			} else
				lObj.SetActive (true);
		}
		lObj.transform.position = character.characterLeftHand.transform.position;
		lObj.transform.rotation = character.characterLeftHand.transform.rotation;
		lObj.transform.SetParent (character.characterLeftHand.transform);
		lObj.transform.localPosition = item.modelCentrePos;
		lObj.transform.localRotation = Quaternion.Euler (item.modelRotations);
		character.anim.SetBool ("equippedLeft", true);
	}


/// <summary>
/// The function that turns on the aiming element on the GUI Canvas
/// </summary>
	public void ToggleAimGUIElement(bool turnOn){
		hudScript.aimGUIElement.gameObject.SetActive (turnOn);
	}

	void OnUnequip(Item item){
		if (item.itemName == "Bow") {
			character.anim.SetBool("useBow", false);
			character.canFPV = true;
			if(camF.fpvMode){
				character.fPVing = false;
				camF.fpvMode = false;
				camF.exitFPV = true;
			}
		}
		if(item == rightHandItem)
			character.anim.SetBool("equippedRight", false);
		if(item == leftHandItem)
			character.anim.SetBool("equippedLeft", false);
	}

	
}