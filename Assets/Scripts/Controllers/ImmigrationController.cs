using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImmigrationSave {

    public float timeDelta;
    public int immigrantsThisMonth;

    public ImmigrationSave(ImmigrationController ic) {

        timeDelta = ic.TimeDelta;
        immigrantsThisMonth = ic.immigrantsThisMonth;

    }

}

public class ImmigrationController : MonoBehaviour {

    public WorldController worldController;
	public List<Structure> Requests { get; set; }
    public float TimeDelta { get; set; }
    public int immigrantsThisMonth;
    public float ImmigrationRate { get { return (TimeController.WeekTime * TimeController.MonthTime) / immigrantsThisMonth; } }

    public void Load(ImmigrationSave ic) {

        TimeDelta = ic.timeDelta;
        immigrantsThisMonth = ic.immigrantsThisMonth;

    }

    public void Start() {

        Requests = new List<Structure>();

    }

    public bool InQueue(Structure s) {

        return Requests.Contains(s);

    }

    
    private void Update() {

        TimeDelta += Time.deltaTime;

        if (TimeDelta >= ImmigrationRate) {

            TimeDelta = 0;
            if (Requests.Count > 0)
                NextImmigrant();

        }

    }

    public void NextImmigrant() {

        Structure s = Requests[Random.Range(0, Requests.Count)];
        if(s != null)
            SpawnImmigrant(s);
        Requests.Remove(s);

    }

    public void SpawnImmigrant(Structure requester) {

        Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();

        if (mapEntrance == null)
            return;

        Node start = new Node(mapEntrance.X, mapEntrance.Y);
        Node end = new Node(requester.X, requester.Y);

        List<Node> testpath = worldController.Map.pathfinder.FindPath(start, end);
        if (testpath.Count == 0)
            return;
        
        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Immigrant"), mapEntrance.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        go.name = "ImmigrantTo_" + name;

        Walker w = go.GetComponent<Walker>();
        w.world = worldController;
        w.Origin = mapEntrance;
        w.Destination = requester;
        w.Path = w.FindPath(start, end);
        w.Activate();

    }

}
