using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementMenu : MonoBehaviour {
    
    public ActionController actionController;
    public GameObject menu;
    public Button rotateButton;
    public Button undo;
    public Toggle dragToggle;
    public Text priceTag;
    public ModeController modeController;
    public MoneyController moneyController;
    public InteractionController interactionController;

    bool wasOpen;

    // Update is called once per frame
    void Update () {

        if(Time.timeScale == 0 && menu.activeSelf) {
            wasOpen = true;
            menu.SetActive(false);
        }
        else if(Time.timeScale != 0 && wasOpen) {
            wasOpen = false;
            menu.SetActive(true);
        }

		
	}

    public void OpenClose() {

        Action a = actionController.currentAction;

        if (a != null) {

            menu.SetActive(true);
            UpdateMenu();

        }

        else {

            menu.SetActive(false);

        }

    }

    void UpdateMenu() {

        Action a = actionController.currentAction;

        //if you can drag the structure, disable rotation and enable drag toggling
        if (actionController.CanDrag(a)) {

            rotateButton.gameObject.SetActive(false);
            dragToggle.gameObject.SetActive(true);
            
            dragToggle.isOn = interactionController.dragEnabled;

        }

        else {

            rotateButton.gameObject.SetActive(true);
            dragToggle.gameObject.SetActive(false);

        }

        undo.interactable = actionController.undoActions.Count > 0;
        UpdatePriceTag();

    }

    void UpdatePriceTag() {

        Action a = actionController.currentAction;

        if (a.Do == "paint")
            priceTag.text = a.What;

        else if (modeController.currentMode == Mode.Construct && a.Do == "place") {

            int price = actionController.GetPrice(actionController.actionLocations, a);

            priceTag.text = price + " shql.";

            if (moneyController.Sheqels - price >= 0)
                priceTag.color = Color.white;
            else
                priceTag.color = Color.red;

        }

    }

}
