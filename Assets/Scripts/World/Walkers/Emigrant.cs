using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emigrant : Walker {

    public override void Activate() {

        base.Activate();

        if (Origin != null) {
            Origin.ActiveImmigrant = true;
            if(Origin is House) {
                House h = (House)Origin;
                h.RemoveResidents(h.Residents - h.ResidentsMax);
            }
        }
            

    }

    private void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            if (!CanGoTo(X, Y))
                Kill();

            if (Path.Count > 0)
                UpdateAStarMovement();
            else
                Kill();
        }

    }

    public override void Kill() {

        base.Kill();

        if (Origin != null)
            Origin.ActiveImmigrant = false;

    }
}
