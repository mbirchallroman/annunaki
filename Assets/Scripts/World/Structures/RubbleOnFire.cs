using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleOnFire : Structure {

    

    public override void DoEveryWeek() {

        base.DoEveryWeek();

        World map = world.Map;

        if (map.IsBuildingAt(X + 1, Y))
            map.GetBuildingAt(X + 1, Y).GetComponent<Structure>().FireCounter();

        if (map.IsBuildingAt(X - 1, Y))
            map.GetBuildingAt(X - 1, Y).GetComponent<Structure>().FireCounter();

        if (map.IsBuildingAt(X, Y + 1))
            map.GetBuildingAt(X, Y + 1).GetComponent<Structure>().FireCounter();

        if (map.IsBuildingAt(X, Y - 1))
            map.GetBuildingAt(X, Y - 1).GetComponent<Structure>().FireCounter();

    }
}
