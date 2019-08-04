using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StableSave : GeneratorSave {

    public int spawned;

    public StableSave(GameObject go) : base(go) {

        Stable s = go.GetComponent<Stable>();
        spawned = s.Spawned;

    }

}


public class Stable : Generator {

    [Header("Stable")]
    public bool spawnOnWater;
    public int spawnMax;
    public int Spawned { get; set; }
    

    public override void Load(ObjSave o) {

        base.Load(o);

        StableSave s = (StableSave)o;
        Spawned = s.spawned;

    }

    public override void Activate() {
        base.Activate();
    }

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (HasEnoughStaff) {

            if (Yield > 0 && !ActiveCarryerWalker) {

                Product.amount = Yield;
                ExportProduct();

                if (ActiveCarryerWalker)
                    Yield = 0;

            }

        }

    }

    public override void DoEveryMonth() {
        base.DoEveryMonth();

        if (HasEnoughStaff && !ActiveCarryerWalker) {

            if (Spawned < spawnMax) {
                if (spawnOnWater)
                    SpawnBoat();
                else
                    SpawnLivestock();
            }

        }
    }

    public void SpawnLivestock() {

        //spawn if no active walker and the building has a walker
        if (!string.IsNullOrEmpty(RandomWalker)) {

            List<Node> entrances = CheckAdjGround();

            //proceed only if there are available roads
            if (entrances.Count == 0)
                return;

            GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Animals/" + RandomWalker));
            go.transform.position = entrances[0].GetVector3();

            Walker w = go.GetComponent<Walker>();
            w.world = world;
            w.Origin = this;
            w.Activate();
        }

    }

    public void SpawnBoat() {

        //spawn if no active walker and the building has a walker
        if (!string.IsNullOrEmpty(RandomWalker)) {

            List<Node> entrances = CheckAdjWater();

            //proceed only if there are available roads
            if (entrances.Count == 0)
                return;

            GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Animals/" + RandomWalker));
            go.transform.position = entrances[0].GetVector3();

            Walker w = go.GetComponent<Walker>();
            w.world = world;
            w.Origin = this;
            w.Activate();
        }

    }
}
