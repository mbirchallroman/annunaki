using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatModeController : MonoBehaviour {

    public TimeController timeController;

    public Text timeLabel;

    public void Update() {

        timeLabel.text = timeController.ToString();
        
    }

}
