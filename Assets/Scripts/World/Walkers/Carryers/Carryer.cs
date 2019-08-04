using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryer : Walker {

    public override void Activate() {
        base.Activate();

        if(Origin != null)
            Origin.ActiveCarryerWalker = true;
    }

    private void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if(TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Origin == null || Destination == null || !CanGoTo(X, Y))
                Kill();

            if (Path.Count > 0)
                UpdateAStarMovement();

            else {

                //if not returning home yet (and therefore just reached destination), 
                if (!ReturningHome) {

                    Stuck = true;

                    //find path back home
                    ReturnHome();

                    //only procede if there's a way back home, otherwise don't continue
                    if (Path.Count == 0)
                        return;

                    //perform action at destination
                    OnceAtDestination();
                    UpdateAStarMovement();

                }

                else if (ReturningHome)
                    OnceBackHome();

            }
        }

    }

    public override void Kill() {
        base.Kill();

        if (Origin != null)
            Origin.ActiveCarryerWalker = false;

    }

    public virtual void OnceAtDestination() {

        //do stuff at destination

    }

    public virtual void OnceBackHome() {

        //do stuff back at home location
        Kill();

    }

}
