using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalSave : WalkerSave {

    public int walkTime, walkTimeMax, restTime, restTimeMax, yield;

    public AnimalSave(GameObject go) : base(go) {

        Animal a = go.GetComponent<Animal>();
        walkTime = a.WalkTime;
        walkTimeMax = a.walkTimeMax;
        restTime = a.RestTime;
        restTimeMax = a.restTimeMax;
        yield = a.yield;

    }

}


public class Animal : Walker {

    public int walkTimeMax;
    public int restTimeMax;
    public int yield;

    public int WalkTime { get; set; }
    public int RestTime { get; set; }

    public override void Load(ObjSave o) {

        base.Load(o);

        AnimalSave a = (AnimalSave)o;
        WalkTime = a.walkTime;
        walkTimeMax = a.walkTimeMax;
        restTimeMax = a.restTime;
        restTimeMax = a.restTimeMax;
        yield = a.yield;

    }

    void Update() {

        TimeDelta += Time.deltaTime;

        Move();

        if (TimeDelta > MovementTime) {

            TimeDelta = 0;

            UpdateRandomMovement();

            //delete animal if on tile it's not supposed to be on
            if (!CanGoTo(X, Y))
                Kill();
            
        }

    }

}
