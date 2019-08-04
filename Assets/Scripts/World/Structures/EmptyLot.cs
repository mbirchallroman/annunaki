using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLot : Structure {

    public string house;

    public override void DoEveryDay() {

        if (RoadAccess() && !ActiveImmigrant && !immigration.InQueue(this))
            RequestImmigrant();

    }

    public void TurnIntoHouse() {
        
        //demolish this and build new house
        world.Demolish(X, Y);
        world.SpawnStructure(house, X, Y, transform.position.y);

        House newHouse = world.Map.GetBuildingAt(X, Y).GetComponent<House>();
        newHouse.FreshHouse();
        newHouse.Activate();

    }

    bool RoadAccess() {
        for (int a = X - 2; a <= X + 2; a++)
            for (int b = Y - 2; b <= Y + 2; b++)
                if (world.Map.IsRoadAt(a, b))
                    if(!world.Map.structures[a,b].Contains("MapE"))
                        return true;
        return false;
    }

    public override void ReceiveImmigrant() {

        TurnIntoHouse();

    }

}
