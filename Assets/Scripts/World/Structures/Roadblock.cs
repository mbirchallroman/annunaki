using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roadblock : Structure {

    public override void Activate() {

        base.Activate();

        world.Map.roads[X, Y] = 1;

    }

}
