using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageItem_adv : MonoBehaviour {

    public StorageBuilding sb { get; set; }
    public int index { get; set; }
    public Text itemLabel;
    public Text willAcceptLabel;
    public Toggle getToggle;

    void Start() {

        UpdateLabel();
        UpdateAcceptButton();
        getToggle.isOn = sb.WillGet[index];

    }

    void UpdateLabel() {

        string s = sb.Inventory[index] + " " + Enums.GetItemName(index, sb.typeStored);

        itemLabel.text = s;

    }

    string GetItemName() {

        //if granary, display food label
        if (sb.typeStored == ItemType.Food)
            return (FoodType)index + "";

        //if warehouse, display goods label
        else if (sb.typeStored == ItemType.Good)
            return (GoodType)index + "";

        //if storage yard, display resource label
        else if (sb.typeStored == ItemType.Resource)
            return (ResourceType)index + "";

        return "???";

    }

    //switch from 0 to 1/4 to 1/2 to 3/4 to 1. if over 1, go to 0
    public void ChangeAccept() {

        sb.WillAccept[index] += .25f;
        if (sb.WillAccept[index] > 1)
            sb.WillAccept[index] = 0;

        UpdateAcceptButton();

    }

    void UpdateAcceptButton() {

        float f = sb.WillAccept[index];
        if (f == 0)
            willAcceptLabel.text = "NONE";
        else if (f == .25f)
            willAcceptLabel.text = "1/4";
        else if (f == .5f)
            willAcceptLabel.text = "1/2";
        else if (f == .75f)
            willAcceptLabel.text = "3/4";
        else if (f == 1)
            willAcceptLabel.text = "ALL";
        else
            willAcceptLabel.text = "???";

    }

    public void SetGet(bool b) {

        sb.WillGet[index] = b;

    }

}
