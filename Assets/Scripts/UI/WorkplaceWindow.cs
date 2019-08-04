using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkplaceWindow : ObjWindow {

    public Toggle toggle;

    public override void Open() {

        base.Open();

        Workplace wp = (Workplace)obj;
        toggle.isOn = wp.ActiveStaff;

    }

    public override void DoDuringUpdate() {

        Workplace wp = (Workplace)obj;
        if (wp.ActiveStaff)
            title.text = obj.DisplayName + " (" + (int)wp.Workers + "/" + wp.WorkersNeeded + ")";
        else
            title.text = obj.DisplayName + " (Closed)";

    }

    public void PlayerToggleLabor(bool b) {
        
        Workplace wp = (Workplace)obj;
        wp.ClosedByPlayer = !b;
        wp.ToggleLabor(b);

    }

}
