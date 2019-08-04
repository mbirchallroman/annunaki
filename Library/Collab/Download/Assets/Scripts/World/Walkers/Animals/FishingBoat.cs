using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingBoat : Livestock {

    void Update() {
        TimeDelta += Time.deltaTime;

        if(WalkTime != 0)
            Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Origin == null || !CanGoTo(X, Y))
                Kill();

            string spot = X + "_" + Y;
            if (!VisitedSpots.Contains(spot))
                VisitedSpots.Add(spot);

            //random movement if walktime > 0
            if (lifeTime > 0) {
                lifeTime--;

                if (WalkTime > 0) {
                    UpdateRandomMovement();
                    WalkTime--;
                }
                else if (RestTime > 0) {
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

    public override void ReturnHome() {

        List<Node> entrances = Origin.CheckAdjWater();
        if (entrances.Count == 0) {
            Kill();
            return;
        }
        Path = FindPath(new Node(X, Y), entrances[0]);
        ReturningHome = true;
        Stuck = true;

    }

    public override void Kill() {

        base.Kill();

        if (Origin is Stable) {

            Stable ls = (Stable)Origin;
            ls.Yield += VisitedSpots.Count;
            ls.Spawned--;

        }

    }

    public override bool CanGoTo(int a, int b) {

        World map = world.Map;

        if (map.OutOfBounds(a, b))
            return false;

        if (map.IsBuildingAt(a, b) && !map.IsRoadAt(a, b))
            return false;

        return map.terrain[a, b] == (int)Terrain.Water;


    }

}
