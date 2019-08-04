using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : Carryer {

    private void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Origin == null || !CanGoTo(X, Y))
                Kill();

            if (Destination == null && !ReturningHome)
                GoToNextFire();

            if (Path.Count > 0)
                UpdateAStarMovement();

            else {

                //if not returning home yet (and therefore just reached destination), 
                if (!ReturningHome) {

                    Stuck = true;

                    //perform action at destination
                    OnceAtDestination();
                    GoToNextFire();

                    //only procede if there's a way back home, otherwise don't continue
                    if (Path.Count > 0)
                        UpdateAStarMovement();

                }

                else if (ReturningHome)
                    OnceBackHome();

            }
        }

    }

    void GoToNextFire() {

        Node f = FindClosestFire();

        if (f == null)
            ReturnHome();

        else {

            Structure fire = world.Map.GetBuildingAt(f).GetComponent<Structure>();

            Destination = fire;

            Node start = new Node(this);

            Path = FindPath(new Node(this), f);

            if (Path.Count > 15)
                ReturnHome();

            Stuck = true;

        }

    }

    public override void OnceAtDestination() {

        Destination.OnFire = false;
        Destination.TurnToRubble();

    }

    public override bool CanGoTo(int a, int b) {

        World map = world.Map;

        //if out of bounds, can't go
        if (map.OutOfBounds(a, b))
            return false;

        //ADD WHEN MAP ENTRANCES ARE ADDED
        if (map.IsBuildingAt(a, b)) {

            if (map.GetBuildingNameAt(a, b).Contains("Rubble"))
                return true;

            if (map.roads[a, b] > 0)
                return true;

            return false;

        }

        //return false if there's water
        return map.terrain[a, b] != (int)Terrain.Water;
    }

    public override void Kill() {

        base.Kill();
        
    }

}
