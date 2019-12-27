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

public class Item : MonoBehaviour {

	public string itemName;					//The name that this item will be referred to and searched by
	public string itemInfo;					//The information that will be displayed when the item is selected in the inventory menu
	public bool usesQuantities;				//Does the item use quantities?
	public int maxQuantity;					//The maximum quantity
	public int quantity;					//The current quantity
	public Sprite icon;						//What icon should this item have when displayed in the inventory?

	public GameObject itemModel;			//What model should be used to represent the item in-game?
	public bool rightHandWield;				//Is the item held by the right hand? (if not -> left hand)
	public bool useWhenEquipped;			//Should the item be used as it's equipped? (if not -> the character will only get the item out and hold it)
	public bool dualWield;					//Is this item used in conjunction with another item?
	public string dualWieldItemName;		//What is the name of the item that can be held by the other hand at the same time?
	public Vector3 modelRotations;			//What should the models rotations be when being held by the character, relative to their parent transform
	public Vector3 modelCentrePos;			//What should the models position be when being held by the character, relative to their parent transform
	public bool positionInScript;			//Should the item be positioned through code when being used? (if not -> uses the two variables above)
	public bool displayModelOnChar;			//Should the item model be displayed on the character once it has been acquired regardless of whether it is equipped or even on a hotkey
	public Vector3 modelDisplayRotations;	//What should the models rotations be when being displayed on the character is 'displayModelOnChar' is true, relative to their parent transform
	public Vector3 modelDisplayCentrePos;	//What should the models position be when being displayed on the character is 'displayModelOnChar' is true, relative to their parent transform
	public GameObject projectileModel;
}
