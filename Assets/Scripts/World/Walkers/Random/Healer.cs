using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : RandomWalker {

    public int healthToGive;

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

        //if healthtogive is greater than what's needed, just give what's needed
        if (healthToGive >= h.HealthNeeded)
            h.Health += h.HealthNeeded;
            
        //otherwise, give health
        else
            h.Health += healthToGive;

    }

}
