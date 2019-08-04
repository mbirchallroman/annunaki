using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : Walker {

    public override void Activate() {

        base.Activate();

        if (Origin != null)
            Origin.ActiveRandomWalker = true;

        UpdateRandomMovement();

    }

    void Update() {
        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (Origin == null || !CanGoTo(X, Y))
                Kill();

            VisitBuildings();

            //random movement if walktime > 0
            if (lifeTime > 0) {
                lifeTime--;
                UpdateRandomMovement();
            }

            //if time is up and there's no path, find one
            else if (Path == null)
                ReturnHome();

            //if a path is supposed to exist
            else if(ReturningHome) {

                //if the path is still there, follow it
                if (Path.Count > 0)
                    UpdateAStarMovement();

                //otherwise kill it
                else
                    Kill();
            }

            //if origin is a workplace and does not have activestaff, kill
            if (Origin is Workplace)
                if (!((Workplace)Origin).ActiveStaff)
                    Kill();

        }

    }

    public void VisitBuildings() {
        for (int a = X - radiusOfInfluence; a <= X + radiusOfInfluence; a++)
            for (int b = Y - radiusOfInfluence; b <= Y + radiusOfInfluence; b++)
                if(world.Map.IsBuildingAt(a,b) && !world.Map.IsRoadAt(a,b))
                    VisitBuilding(a, b);

    }

    public override void VisitBuilding(int a, int b) {

        Structure s = world.Map.GetBuildingAt(a, b).GetComponent<Structure>();

        if (laborSeeker && Origin is Workplace)
            if (s is House)
                LaborPoints++;
            

    }


    public override void Kill() {

        base.Kill();

        if(Origin != null)
            Origin.ActiveRandomWalker = false;

        if (laborSeeker && Origin is Workplace) {
            Workplace w = (Workplace)Origin;
            w.Access = LaborPoints;
            labor.CalculateWorkers();
        }
            
            
    }
}
