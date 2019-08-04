using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorsTower : Workplace {

    public override void DoEveryDay() {

        base.DoEveryDay();

        if(!ActiveCarryerWalker)
            SearchForFire();

    }

    void SearchForFire() {

        Node fire = FindClosestFire();
        if (fire != null)
            SendFiremanTo(fire);

    }

    void SendFiremanTo(Node fire) {
        
        List<Node> entrances = CheckAdjRoads();
        Structure str = world.Map.GetBuildingAt(fire).GetComponent<Structure>();

        if (entrances.Count == 0)
            return;

        Node start = entrances[0];

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Fireman"));
        go.transform.position = start.GetVector3();
        go.name = "FiremanFrom_" + name;

        Carryer c = go.GetComponent<Carryer>();
        c.world = world;
        c.Origin = this;
        c.Destination = str;
        c.Path = c.FindPath(start, fire);
        c.Activate();

    }

}
