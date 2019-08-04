using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Livestock : Animal {

    public override void Activate() {

        base.Activate();

        if (Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.Spawned++;

        }

        UpdateRandomMovement();
    }

    void Update() {
        TimeDelta += Time.deltaTime;

        if (WalkTime != 0)
            Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Origin == null || !CanGoTo(X, Y))
                Kill();

            //random movement if walktime > 0
            if (lifeTime > 0) {
                lifeTime--;

                if(WalkTime > 0) {
                    UpdateRandomMovement();
                    WalkTime--;
                }
                else if(RestTime > 0) {
                    Stuck = true;
                    RestTime--;
                }
                else {
                    Stuck = false;
                    WalkTime = walkTimeMax;
                    RestTime = restTimeMax;
                }
                
            }

            //if time is up and there's no path, find one
            else if (Path == null)
                ReturnHome();

            //if a path is supposed to exist
            else if (ReturningHome) {

                //if the path is still there, follow it
                if (Path.Count > 0)
                    UpdateAStarMovement();

                //otherwise kill it
                else
                    Kill();
            }
            

        }

    }

    public override void Kill() {

        base.Kill();

        if(Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.Yield += yield;
            ls.Spawned--;

        }

    }

    public override void ReturnHome() {

        List<Node> entrances = Origin.CheckAdjGround();
        if (entrances.Count == 0) {
            Kill();
            return;
        }
        Path = FindPath(new Node(X, Y), entrances[0]);
        ReturningHome = true;
        Stuck = true;

    }

}
