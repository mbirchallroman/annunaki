using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkplaceSave : StructureSave {

    public int workers, timeToSpawnWalker, access, yield;
    public bool activeStaff, closedByPlayer;
    public bool[] activeSchedule;

    public WorkplaceSave(GameObject go) : base(go) {

        Workplace w = go.GetComponent<Workplace>();

        workers = w.Workers;
        
        timeToSpawnWalker = w.TimeToSpawnWalker;
        access = w.Access;
        yield = w.Yield;

        
        activeStaff = w.ActiveStaff;
        closedByPlayer = w.ClosedByPlayer;

        activeSchedule = w.ActiveSchedule;

    }

}


public class Workplace : Structure {

    [Header("Workplace")]
    public int timeToSpawnWalkerMax;
    public int WorkersNeeded;
    public LaborDivision laborDivision;

    public int TimeToSpawnWalker { get; set; }
    
    public int Workers { get; set; }

    public bool ActiveStaff { get; set; }
    public bool ClosedByPlayer { get; set; }

    public bool[] ActiveSchedule { get; set; }

    public int Access { get; set; }
    public int Yield { get; set; }

    public override void Load(ObjSave o) {
        base.Load(o);

        //load vars for workplaces
        WorkplaceSave w = (WorkplaceSave)o;

        Workers = w.workers;
        TimeToSpawnWalker = w.timeToSpawnWalker;
        Access = w.access;
        Yield = w.yield;
        
        ActiveStaff = w.activeStaff;
        ClosedByPlayer = w.closedByPlayer;

        ActiveSchedule = w.activeSchedule;

        //add workers back to laborcontroller
        ToggleLabor(ActiveStaff);
    }

    //vars that come from other vars
    public float PercentEmployed { get { return (float)Workers / WorkersNeeded; } }
    public bool HasEnoughStaff { get { return PercentEmployed > .6f && ActiveStaff; } }

    public override void Activate() {
        base.Activate();
        
        TimeToSpawnWalker = timeToSpawnWalkerMax;
        labor.LaborDivisions[(int)laborDivision].Add(this);
        ActiveSchedule = new bool[4];
        for (int a = 0; a < ActiveSchedule.Length; a++)
            ActiveSchedule[a] = true;
        ToggleLabor(true);

    }

    public override void UponDestruction() {

        base.UponDestruction();

        ToggleLabor(false);
        labor.LaborDivisions[(int)laborDivision].Remove(this);

    }

    public override void DoEveryDay() {

        //spawn walker or laborseeker process
        if (!ActiveRandomWalker) {

            if (HasEnoughStaff && timeToSpawnWalkerMax != 0)
                RandomWalkerCounter();

            else if(!HasEnoughStaff)
                SpawnLaborSeeker();

        }

        if (HasEnoughStaff && radiusActive)
            VisitBuildings();
            

        //open/close labor if season changes and not closed by player
        if(!ClosedByPlayer)
            ToggleLabor(ActiveSchedule[time.Seasons]);
    }

    public void RandomWalkerCounter() {

        TimeToSpawnWalker--;

        if (TimeToSpawnWalker <= 0) {

            SpawnRandomWalker();
            TimeToSpawnWalker = timeToSpawnWalkerMax;

        }

    }

    //ADD OVERALL LABOR STUFF
    public void ToggleLabor(bool b) {

        if (b && !ActiveStaff) {
            ActiveStaff = true;
            labor.AddLaborReq((int)laborDivision, WorkersNeeded);
        }

        else if (!b && ActiveStaff) {
            ActiveStaff = false;
            labor.RemoveLaborReq((int)laborDivision, WorkersNeeded);
        }

        if(labor != null)
            labor.CalculateWorkers();
    }

    public override void OpenWindow() {

        OpenWindow(UIObjects.workplaceWindow);

    }

    public void SpawnLaborSeeker() {

        List<Node> entrances = CheckAdjRoads();

        //proceed only if there are available roads
        if (entrances.Count == 0)
            return;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/RandomWalkers/LaborSeeker"), entrances[0].GetVector3(), Quaternion.Euler(new Vector3(0, 0, 0)));

        Walker w = go.GetComponent<Walker>();
        w.world = world;
        w.Origin = this;
        w.Activate();

    }

}
