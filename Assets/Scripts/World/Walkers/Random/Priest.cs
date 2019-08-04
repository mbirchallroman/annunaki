using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : RandomWalker {

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        Temple t = Origin.GetComponent<Temple>();
        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

        foreach (string god in t.gods)
            h.SetFaith(god, 6);

    }

}
