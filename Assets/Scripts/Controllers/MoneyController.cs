using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MoneySave {

    public int sheqels;

    public MoneySave(MoneyController mc) {

        sheqels = mc.Sheqels;

    }

}

public class MoneyController : MonoBehaviour {

    public Text moneyLabel;

	public int Sheqels { get; set; }

    public void Load(MoneySave mc) {

        Sheqels = mc.sheqels;

    }

    public void Update() {

        moneyLabel.text = Sheqels + " Sheqels";

        if (Sheqels > 0)
            moneyLabel.color = Color.white;
        else
            moneyLabel.color = Color.red;

    }

}
