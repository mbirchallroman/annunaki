using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImporterCaravan : Caravan {

    public override void OnceAtDestination() {

        base.OnceAtDestination();
        Destination.ReceiveItem(Order);
        money.Sheqels -= (int)Order.ExchangeValue();

    }

}
