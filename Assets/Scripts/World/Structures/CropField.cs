using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropField : Generator {

    [Header("Crop Field")]
    public List<Month> startTimes;

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (HasEnoughStaff && !ActiveCarryerWalker) {

            if (Producing)
                ProductionTimer();

            else if (startTimes.Contains(time.currentMonth))
                Producing = true;


        }

    }
    
}
