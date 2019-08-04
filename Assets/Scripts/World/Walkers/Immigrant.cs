using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Immigrant : Walker {

    public override void Activate() {

        base.Activate();

        if (Destination != null)
            Destination.ActiveImmigrant = true;

    }

    private void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (!CanGoTo(X, Y))
                Kill();

            //if there's no house to go to, go to map exit
            if (Destination == null && !ReturningHome)
                LeaveMap();

            if (Path.Count > 0)
                UpdateAStarMovement();

            else {

                //if found destination, give immigrant and kill self
                if (X == Destination.X && Y == Destination.Y) {
                    Destination.ReceiveImmigrant();
                    Kill();
                }

                //else if you're at the end of the path but there's no house, go to map exit
                else if (!ReturningHome)
                    LeaveMap();

                //once there, kill self
                else
                    Kill();
                    
            }
        }

    }
    

}
