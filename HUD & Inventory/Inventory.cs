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

/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	private GamePadInputs gPI;
	private HUD hudScript;											//The HUD.cs script attached to the Handler game object in the scene
	private UsingItems uItems;

	[SerializeField] public EventSystem eventSys;					//The Event System that the buttons in the inventory canvas will use
	[SerializeField] private Canvas inventoryCanvas;				//The GUI Canvas that you would like the inventory to be drawn to (recommended: use a separate canvas than the HUD one)
	[SerializeField] private bool useNextPrevButtons;				//If the user wishes to use the next/previous GUI Buttons
	[SerializeField] private bool useItemInfoDisplay;				//If the user wishes to display the information of the currently selected item
	[SerializeField] private Button inventoryPositionGUI;			//The GUI Button on the inventoryCanvas used to display all the items in the current page
	[SerializeField] private Text itemInfoPositionGUI;				//The GUI Text on the inventoryCanvas used to write out the information of an item
	[SerializeField] private Button previousPageGUIButton;			//The GUI Button on the inventoryCanvas that will move to the previous page of items being displayed
	[SerializeField] private Button nextPageGUIButton;				//The GUI Button on the inventoryCanvas that will move to the next page of items being displayed

	[System.Serializable] public class ItemDictionary{ public GameObject item; public bool hasItem; }
	public ItemDictionary[] itemDictionary;							//The collection of items that the character can acquire (despite whether they have been acquired or not
	[SerializeField] private int inventoryPages;					//The amount of pages of items you wish to have displayed in your inventoryCanvas
	[SerializeField] private int itemsPerPage;						//The amount of items that can be displayed per page
	[HideInInspector] public bool itemDictNotEmpty;					//A bool that checks if there are no elements in the item dictionary
	[SerializeField] private int maxItemsX;							//The amount of items that can be displayed in a row on each page of items in the inventory
	[SerializeField] private float inventoryYSpacing;				//How far apart should the items be displayed in the Vertical Axis
	[SerializeField] private float inventoryXSpacing;				//How far apart should the items be displayed in the Horizontal Axis
	[HideInInspector] public Dictionary<int, GameObject> guiHotkeyContent = new Dictionary<int, GameObject>();	//What item each hotkey on the HUDCanvas contains

	private int currentInventoryPage;								//The current page being viewed in the inventoryCanvas
	private GameObject selectedObject;								//The object that is currently being selected in the inventoryCanvas
	
	private Button[] inventoryButtons = new Button[0];				//The buttons behind each item in the inventory to signify a 'space' in the page of items
	private Button[] itemIcons = new Button[0];						//The buttons that represent the currently acquired items in the page of items and are what the player navigates through
	private Image[] hotkeyContent = new Image[0];					//The item icon that is displayed on the hotkeys of the HUDCanvas
	private GameObject[] itemsInDictionary = new GameObject[0];		//An array holding instantiations of the prefabs used in the item dictionary (makes physical instances of each item)

	private ThirdPersonController character;						//The ThirdPersonController.cs script attached to the character
	private bool canMoveSelected;									//A delay used for when changing the inventory page (so as not to flick through them too rapidly)

	private bool hasError;
	private bool firstPass;

	private bool holdingAButton;
	private CheckIfHoldingButton<bool> heldButton;

	void Start () {


		inventoryCanvas = GameObject.Find ("InventoryCanvas").GetComponent<Canvas> ();
		eventSys = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		gPI = this.GetComponent<GamePadInputs> ();
		hudScript = this.GetComponent<HUD> ();
		character = GameObject.FindWithTag ("Player").GetComponent<ThirdPersonController> ();
		uItems = this.GetComponent<UsingItems> ();
		uItems.inventory = this;
		uItems.character = this.character;
		uItems.hotkeysAmount = hudScript.hotkeysAmount;
		heldButton = () => false;
		holdingAButton = false;

	////Setting Bools
		itemDictNotEmpty = false;
		canMoveSelected = true;
		hasError = false;

	////Inventory Init
		if (itemDictionary.Length > 0) {
			itemDictNotEmpty = true;
			inventoryButtons = new Button[itemDictionary.Length];
			itemIcons = new Button[itemDictionary.Length];
			itemsInDictionary = new GameObject[itemDictionary.Length];
		}
		int i = 0;
		GameObject itemsInDict = new GameObject ();
		itemsInDict.name = "Items In Dictionary";
		while (i < itemDictionary.Length) {
			itemsInDictionary[i] = Instantiate(itemDictionary[i].item) as GameObject;
			itemsInDictionary[i].transform.SetParent(itemsInDict.transform);
			itemsInDictionary[i].name = itemDictionary[i].item.name;
			itemDictionary[i].item = itemsInDictionary[i];
			if(itemDictionary[i].hasItem)
				DisplayItemModel(i);
			i++;
		}
		if (inventoryPages < 1)
			inventoryPages = 1;
		currentInventoryPage = 1;
		if(itemDictNotEmpty){
			if(maxItemsX < 1){
				Debug.LogError ("Handler.cs: Please set 'maxItemsX' to a number higher than 0.");
				maxItemsX = 1;
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
	}

	void Update(){
		gPI = this.GetComponent<GamePadInputs> ();
		hudScript = this.GetComponent<HUD> ();
		character = GameObject.FindWithTag ("Player").GetComponent<ThirdPersonController> ();
		if (!firstPass) {
			if(hudScript.useInventory){
				hudScript.hotKeyGUIElement.gameObject.SetActive(true);
				hotkeyContent = new Image[hudScript.hotkeysAmount];
				int h = 0;
				while(h < hudScript.hotkeysAmount) {
					hotkeyContent[h] = Instantiate(hudScript.hotKeyGUIElement.GetComponent<Image>()) as Image;
					hotkeyContent[h].GetComponent<Button>().enabled = false;
					hotkeyContent[h].transform.SetParent(hudScript.hudCanvas.transform);
					hotkeyContent[h].rectTransform.anchoredPosition = hudScript.hotKeys[h].GetComponent<RectTransform>().anchoredPosition;
					hotkeyContent[h].sprite = null;
					hotkeyContent[h].color = Color.clear;
					hotkeyContent[h].name = "Hotkey Item Icon " + h;
					guiHotkeyContent[h] = null;
					h++;
				}
				hudScript.hotKeyGUIElement.gameObject.SetActive(false);
			}
			if(!useNextPrevButtons){
				previousPageGUIButton.gameObject.SetActive(false);
				nextPageGUIButton.gameObject.SetActive(false);
			}
			firstPass = true;
		}

		if(character.isPaused && hudScript.useInventory){
			if (inventoryPages > 1 && canMoveSelected) {
				if(gPI.LT > 0.5f && currentInventoryPage > 1){
					ChangePage(-1);
					canMoveSelected = false;
					StartCoroutine(MoveDelay());
				}
				if(gPI.RT > 0.5f && currentInventoryPage < inventoryPages){
					ChangePage(1);
					canMoveSelected = false;
					StartCoroutine(MoveDelay());
				}
			}
			if (eventSys.currentSelectedGameObject == null){
				selectedObject = null;
				if(gPI.LH != 0f || gPI.LV != 0f)
					itemIcons[(currentInventoryPage-1) * itemsPerPage].Select();
				if(useItemInfoDisplay){
					if(itemInfoPositionGUI.text != "")
						itemInfoPositionGUI.text = "";
				}
			}else {
				if(selectedObject == null || selectedObject.name != eventSys.currentSelectedGameObject.name){
					int f = 0;
					while (f < itemDictionary.Length) {
						if(itemDictionary[f].item.name == eventSys.currentSelectedGameObject.name){
							selectedObject = itemDictionary[f].item.gameObject;
						}
						f++;
					}
				}
				if(useItemInfoDisplay){
					if(itemInfoPositionGUI.text != selectedObject.GetComponent<Item>().itemInfo)
						itemInfoPositionGUI.text = selectedObject.GetComponent<Item>().itemInfo;
				}
				if(gPI.pressRBump)
					AssignHotKey(0);
				if(gPI.pressTopB)
					AssignHotKey(1);
				if(gPI.pressRightB)
					AssignHotKey(2);
			}
		}

		if (!character.isPaused && uItems.canUseItems && !uItems.currentAnims.inALiftingAnimation) {
			if(!uItems.currentAnims.inAnItemUsageAnim){
				if(gPI.pressRBump || gPI.pressTopB || gPI.pressRightB){
					int guiHotkeyID = -1;
					if(gPI.pressRBump)
						guiHotkeyID = 0;
					if(gPI.pressTopB)
						guiHotkeyID = 1;
					if(gPI.pressRightB)
						guiHotkeyID = 2;
					if(guiHotkeyID > -1)
						ItemUseInput(guiHotkeyID, false);
				}
				if(!holdingAButton){
					if(gPI.holdRBump || gPI.holdTopB || gPI.holdRightB){
						int guiHotkeyID = -1;
						if(gPI.holdRBump){
							guiHotkeyID = 0;
							heldButton = () => gPI.holdRBump;
						}
						if(gPI.holdTopB){
							guiHotkeyID = 1;
							heldButton = () => gPI.holdTopB;
						}
						if(gPI.holdRightB){
							guiHotkeyID = 2;
							heldButton = () => gPI.holdRightB;
						}
						if(guiHotkeyID > -1){
							holdingAButton = true;
							ItemUseInput(guiHotkeyID, true);
						}
					}
				}
			}
			if(!heldButton() && holdingAButton){
				uItems.UseItemRelease();
				holdingAButton = false;
			}
		}
	}

	public delegate TResult CheckIfHoldingButton<out TResult>();

	/// <summary>
	/// The function used for when the player presses a corresponding hotkey button to use the content of that hotkey (the item assigned to it)
	/// </summary>
	void ItemUseInput(int guiHotkeyID, bool holding){
		int itemIndx = 0;
		int itemFound = 0;
		bool found = false;
		while(itemIndx < itemsInDictionary.Length && !found){
			if(guiHotkeyContent[guiHotkeyID] != null &&
			   guiHotkeyContent[guiHotkeyID].GetComponent<Item>().itemName == itemsInDictionary[itemIndx].GetComponent<Item>().itemName){
				found = true;
				itemFound = itemIndx;
			}
			itemIndx++;
		}
		if(guiHotkeyContent[guiHotkeyID] != null){
			if(itemsInDictionary[itemFound].GetComponent<Item>() != uItems.rightHandItem &&
			   itemsInDictionary[itemFound].GetComponent<Item>() != uItems.leftHandItem &&
			   itemsInDictionary[itemFound].GetComponent<Item>() != uItems.currentItem)
				uItems.EquipItem (itemsInDictionary[itemFound].gameObject);
			else{
				if(holding)
					uItems.UseItemHold(itemsInDictionary[itemFound].GetComponent<Item>());
				else
					uItems.UseItemPress(itemsInDictionary[itemFound].GetComponent<Item>());
			}
		}
	}

/// <summary>
/// The function used for when the player equips and item and has the item physically spawn into the scene
/// </summary>
	public void DisplayItemModel(int itemDicIndex){
		Item itemScript = itemDictionary [itemDicIndex].item.GetComponent<Item> ();
		if (itemScript.displayModelOnChar && itemDictionary [itemDicIndex].hasItem) {
			if (itemScript.itemModel != null) {
				GameObject itemModelObj = Instantiate (itemScript.itemModel.gameObject, Vector3.zero, character.itemUnequippedPos.transform.localRotation) as GameObject;
				itemModelObj.name = itemScript.itemName + " Model";
				itemModelObj.transform.SetParent (character.itemUnequippedPos.transform);
				itemModelObj.transform.localPosition = Vector3.zero;
				itemModelObj.transform.localRotation = Quaternion.Euler (Vector3.zero);
				itemModelObj.transform.Rotate (itemScript.modelDisplayRotations);
				itemModelObj.transform.localPosition += itemScript.modelDisplayCentrePos;
				int index = uItems.modelObjPool.Count;
				uItems.modelObjPool.Add(index, itemModelObj);
			}
		}
	}

/// <summary>
/// The function used for when the player assigns an item in the inventory to a hotkey in the HUD
/// </summary>
	void AssignHotKey(int hotkeyIndex){
		int h = 0;
		while (h < hudScript.hotkeysAmount) {
			if(guiHotkeyContent[h] == selectedObject.gameObject){
				if(guiHotkeyContent[hotkeyIndex] != null){
					if(guiHotkeyContent[hotkeyIndex].GetComponent<Item>().usesQuantities)
						hotkeyContent[h].GetComponentInChildren<Text>().text = guiHotkeyContent[hotkeyIndex].GetComponent<Item>().quantity.ToString();
					else
						hotkeyContent[h].GetComponentInChildren<Text>().text = "";
					if(selectedObject.GetComponent<Item>().usesQuantities)
						hotkeyContent[hotkeyIndex].GetComponentInChildren<Text>().text = selectedObject.GetComponent<Item>().quantity.ToString();
					else
						hotkeyContent[hotkeyIndex].GetComponentInChildren<Text>().text = "";
					guiHotkeyContent[h] = guiHotkeyContent[hotkeyIndex];
					hotkeyContent[h].sprite = hotkeyContent[hotkeyIndex].sprite;
					hotkeyContent[h].color = Color.white;
					h = hudScript.hotkeysAmount;
				}else{
					hotkeyContent[h].GetComponentInChildren<Text>().text = "";
					guiHotkeyContent[h] = guiHotkeyContent[hotkeyIndex];
					hotkeyContent[h].sprite = null;
					hotkeyContent[h].color = Color.clear;
					h = hudScript.hotkeysAmount;
				}
			}
			h++;
		}
		hotkeyContent[hotkeyIndex].sprite = selectedObject.GetComponent<Item>().icon;
		hotkeyContent[hotkeyIndex].color = Color.white;
		if (selectedObject.GetComponent<Item> ().usesQuantities)
			hotkeyContent [hotkeyIndex].GetComponentInChildren<Text> ().text = selectedObject.GetComponent<Item> ().quantity.ToString ();
		else
			hotkeyContent [hotkeyIndex].GetComponentInChildren<Text> ().text = "";
		guiHotkeyContent [hotkeyIndex] = selectedObject.gameObject;
		UpdateHotkeyQuantities ();
		selectedObject = null;
	}

/// <summary>
/// The function used to draw the/a inventory page
/// </summary>
	public void DrawInventoryPage(){
		inventoryPositionGUI.interactable = true;
		inventoryPositionGUI.image.enabled = true;
		if (useNextPrevButtons) {
			if (currentInventoryPage == 1)
				previousPageGUIButton.interactable = false;
			else
				previousPageGUIButton.interactable = true;
			if (currentInventoryPage == inventoryPages)
				nextPageGUIButton.interactable = false;
			else
				nextPageGUIButton.interactable = true;
		}
		int i = (currentInventoryPage - 1) * itemsPerPage;
		int j = 0;
		int k = 0;
		int l = 1;
		Item currentItem;
		if(itemDictionary.Length > itemsPerPage * inventoryPages){
			Debug.LogError ("Handler.cs: There are more items in your item dictionary than the amount of inventory pages allows. Either add more pages or increase the amount of items per page.");
			hasError = true;
		}
		while(i < itemDictionary.Length && i < currentInventoryPage * itemsPerPage){
			if(itemDictionary[i].item.gameObject.GetComponent<Item>() != null){
				currentItem = itemDictionary[i].item.gameObject.GetComponent<Item>();
				if(k%maxItemsX == 0 && k != 0){
					k = 0;
					j++;
				}
				if(i == itemsPerPage * l){
					l++;
					k = 0;
					j = 0;
				}
				if(currentItem.name.Length < 1){
					Debug.LogError ("Handler.cs: Element " + i + " of your itemDictionary is missing a name for that gameObjects Item.cs script. Please name it.");
					hasError = true;
				}
				if(currentItem.itemInfo.Length < 1 && useItemInfoDisplay){
					Debug.LogError ("Handler.cs: Element " + i + " of your itemDictionary is missing item information for that gameObjects Item.cs script. Please write some information on the item.");
					hasError = true;
				}
				if(currentItem.icon == null){
					Debug.LogError ("Handler.cs: Element " + i + " of your itemDictionary is missing an icon for that gameObjects Item.cs script. Please use a Texture icon for that item.");
					hasError = true;
				}
				Vector2 guiSize = inventoryPositionGUI.GetComponent<RectTransform>().sizeDelta;
				if(!hasError){
					Vector2 buttonPos = inventoryPositionGUI.GetComponent<RectTransform>().anchoredPosition +
						new Vector2 ((guiSize.x + inventoryXSpacing)*k, 
						             -(guiSize.y + inventoryYSpacing)*j);
					if(inventoryButtons[i] == null)
						inventoryButtons[i] = Instantiate(inventoryPositionGUI) as Button;
					inventoryButtons[i].transform.SetParent(inventoryCanvas.transform);
					inventoryButtons[i].GetComponent<RectTransform>().anchoredPosition = buttonPos;
					inventoryButtons[i].gameObject.name = "Inventory Button " + (i+1);
					inventoryButtons[i].gameObject.SetActive(true);
					inventoryButtons[i].interactable = false;
					inventoryButtons[i].gameObject.GetComponent<Button>().enabled = false;
					if(itemIcons[i] == null)
						itemIcons[i] = Instantiate(inventoryPositionGUI) as Button;
					itemIcons[i].gameObject.SetActive(true);
					itemIcons[i].transform.SetParent(inventoryCanvas.transform);
					itemIcons[i].GetComponent<RectTransform>().anchoredPosition = buttonPos;
					itemIcons[i].gameObject.name = currentItem.name;
					itemIcons[i].GetComponent<Image>().sprite = currentItem.icon;
					if(currentItem.usesQuantities)
						itemIcons[i].GetComponentInChildren<Text>().text = currentItem.quantity.ToString();
					itemIcons[i].enabled = true;
					if(itemDictionary[i].hasItem){
						itemIcons[i].interactable = true;
						itemIcons[i].image.color = Color.white;
						itemIcons[i].navigation.mode.Equals(Navigation.Mode.Automatic);
						if(currentItem.usesQuantities)
							itemIcons[i].GetComponentInChildren<Text>().text = currentItem.quantity.ToString();
					}else{
						itemIcons[i].interactable = false;
						itemIcons[i].image.color = Color.clear;
					}
				}
			}else {
				Debug.LogError ("Handler.cs: Element " + i + " of your itemDictionary is missing an their Item.cs script. Please attach the Item.cs script to that gameObject.");
				hasError = true;
			}
			i++;
			k++;
		}
		eventSys.firstSelectedGameObject = itemIcons [(currentInventoryPage - 1) * itemsPerPage].gameObject;
		inventoryPositionGUI.interactable = false;
		inventoryPositionGUI.image.enabled = false;
		itemIcons[(currentInventoryPage-1) * itemsPerPage].Select();
	}

/// <summary>
/// The function used for when the character acquires a new item that is in the list of items but hasn't been "acquired" yet
/// </summary>
	public void AcquireItem(int itemIndex){
		itemDictionary [itemIndex].hasItem = true;
		if (itemDictionary [itemIndex].item.GetComponent<Item> ().displayModelOnChar)
			uItems.PutBackOnDisplay (itemDictionary [itemIndex].item.GetComponent<Item> ());
	}

/// <summary>
/// The function used for changing the current inventory page
/// </summary>
	public void ChangePage(int amount){
		int i = (currentInventoryPage - 1) * itemsPerPage;	
		while(i < itemDictionary.Length && i < currentInventoryPage * itemsPerPage){
			inventoryButtons[i].gameObject.SetActive(false);
			itemIcons[i].gameObject.SetActive(false);
			i++;
		}
		currentInventoryPage += Mathf.RoundToInt(Mathf.Sign(amount));
		DrawInventoryPage ();
	}

/// <summary>
/// The function used for when the quantity of an item on a hotkey changes at all
/// </summary>
	public void UpdateHotkeyQuantities(){
		int u = 0;
		while (u < hudScript.hotkeysAmount) {
			if(guiHotkeyContent[u] != null){
				int t = 0;
				while(t < itemDictionary.Length){
					if(guiHotkeyContent[u].name == itemDictionary[t].item.name &&
						itemDictionary[t].item.GetComponent<Item>().usesQuantities)
					hotkeyContent[u].GetComponentInChildren<Text>().text = itemDictionary[t].item.GetComponent<Item>().quantity.ToString();
					t++;
				}
			}
			u++;
		}
		u = 0;
	}

	IEnumerator MoveDelay(){
		System.DateTime timeToShowNextElement = System.DateTime.Now.AddSeconds(0.2f);
		while (System.DateTime.Now < timeToShowNextElement)
		{
			yield return null;
		}
		canMoveSelected = true;
	}
}
*/