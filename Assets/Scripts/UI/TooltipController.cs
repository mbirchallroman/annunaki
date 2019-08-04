using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour {

    public Text tooltipText;
    public GameObject tooltipBox;

    // Use this for initialization
    void Start() {
        tooltipText.text = "";
    }

    // Update is called once per frame
    void Update() {
        tooltipBox.transform.position = Input.mousePosition + new Vector3(0, 6, 0);

        //if text is empty, make bg invisible
        tooltipBox.SetActive(!string.IsNullOrEmpty(tooltipText.text));
    }

    public void SetText(string s) {
        tooltipText.text = s;
    }
}
