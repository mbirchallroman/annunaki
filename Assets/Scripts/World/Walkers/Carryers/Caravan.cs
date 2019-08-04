using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caravan : Carryer {

    public override void Activate() {

        base.Activate();

        trade.TradeOrders[Order] = true;

    }

    private void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Destination == null && !ReturningHome)
                LeaveMap();

            if (!CanGoTo(X, Y))
                Kill();

            if (Path.Count > 0)
                UpdateAStarMovement();

            else {

                //if not returning home yet (and therefore just reached destination), 
                if (!ReturningHome) {

                    Stuck = true;

                    //find path back home
                    LeaveMap();

                    //only procede if there's a way back home, otherwise don't continue
                    if (Path.Count == 0)
                        return;

                    //perform action at destination
                    OnceAtDestination();
                    UpdateAStarMovement();

                }
                else
                    Kill();

            }
        }

    }

    public override void Kill() {

        base.Kill();

        trade.TradeOrders[Order] = false;

    }

}
