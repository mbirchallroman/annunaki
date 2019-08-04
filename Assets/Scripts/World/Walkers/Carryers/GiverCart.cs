using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiverCart : Carryer {

    public override void OnceAtDestination() {

        base.OnceAtDestination();
        Destination.ReceiveItem(Order);

    }

}
