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

public class Pickup : MonoBehaviour {

	private HUD handler;			//The Handler.cs script that this pickup will search for to find your items
	//private Inventory inventory;

	public string pickupType;		//Must have same string value as the item name it will replenish (e.g. if for "Bow" then use "Bow")
	public int quantity;			//Amount to replenish

	private bool cannotPickup;		//A delay used for testing in the editor, can be removed
	private bool firstPass;
	public bool respawnAfter4s;

	void Start(){
		handler = GameObject.FindWithTag ("Handler").GetComponent<HUD>();
		//inventory = GameObject.FindWithTag ("Handler").GetComponent<Inventory> ();
		cannotPickup = false;
		firstPass = true;
	}

	void Update(){
		if (firstPass) {
			bool hasError = false;
			if (handler == null) {
				handler = GameObject.FindWithTag ("Handler").GetComponent<HUD> ();
				if (handler == null) {
					Debug.LogError ("Pickup.cs: There is no instance of the Handler.cs script in your scene. Please use the 'Handler' gameobject prefab in the 'Prefabs' folder and drag it into the hierarchy.");
					hasError = true;
				}
			}
	////Error Handling
			if (hasError) {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
				Debug.DebugBreak ();
				Application.Quit ();
				Debug.Break ();
                #endif
            }
			firstPass = false;
		} else {
	////Rotation for effect
			this.transform.RotateAround (this.transform.position, Vector3.up, Time.deltaTime * 100f);
		}
	}

	/*void OnTriggerEnter(Collider col){
		bool finished = false;
		if(col.gameObject.tag == "Player" && !cannotPickup && handler.useInventory){
			int p = 0;
			while(p < inventory.itemDictionary.Length && !finished){
				Item currentItem = inventory.itemDictionary [p].item.GetComponent<Item> ();
				if(this.pickupType == currentItem.itemName){
		////Item Replenishing code; adds the quantity in this pickup to the item in your inventory
					currentItem.quantity += this.quantity;
					if(currentItem.quantity > currentItem.maxQuantity)
						currentItem.quantity = currentItem.maxQuantity;
					inventory.UpdateHotkeyQuantities();
					finished = true;
				}
				p++;
			}
		}
		if (!finished) {
		////Health Replenishing code
			if(this.pickupType == "Health"){
				StartCoroutine(handler.ChangeHealth(this.quantity));
				finished = true;
			}
		////Money Replenishing code
			if(this.pickupType == "Money"){
				StartCoroutine(handler.ChangeMoney(this.quantity));
				finished = true;
			}
		}
		if (finished) {
			////Added Respawn feature for testing in Editor; can remove this statement and the IEnumerator function below
			if (respawnAfter4s) {
				cannotPickup = true;
				Renderer[] renderers = GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in renderers) {
					r.enabled = false;
				}
				StartCoroutine (RespawnPickup ());
			}
		}
	}

	/// <summary>
	/// Respawns the pickup for testing mainly
	/// </summary>
	IEnumerator RespawnPickup(){
		yield return new WaitForSeconds(4f);
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in renderers)
		{
			r.enabled = true;
		}
		yield return new WaitForSeconds(1f);
		cannotPickup = false;
	}
}
*/
}