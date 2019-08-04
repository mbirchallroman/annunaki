using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LaborSave {

    public int population;
    public int[] labor, laborMax;

    public LaborSave(LaborController lc) {

        population = lc.Population;
        labor = lc.Labor;
        laborMax = lc.LaborMax;

    }
}

public class LaborController : MonoBehaviour {

    public int AllocateWorkers { get; set; }
    public int Population { get; set; }
    public int[] Labor { get; set; }
    public int[] LaborMax { get; set; }
    public List<Workplace>[] LaborDivisions { get; set; }

    public int LaborNeededAt(int d) { return LaborMax[d] - Labor[d]; }
    public int Unemployed { get { return Population - workforce(); } }
    public int UnemployedPercent { get { return (int)((float)Unemployed / Population * 100); } }
    public int EmployedPercent { get { return (int)((float)workforce() / Population * 100); } }

    public WorldController worldController;
    public Text popLabel;

    public void Update() {

        if (Population == 0)
            popLabel.text = "A Couple Ghosts?";
        else
            popLabel.text = Population + " Denizens (" + EmployedPercent + "%)";

    }

    public void Load(LaborSave lc) {

        //load these arrays and ints
        Population = lc.population;
        Labor = lc.labor;
        LaborMax = lc.laborMax;

        LaborDivisions = new List<Workplace>[Labor.Length];

        for (int d = 0; d < Labor.Length; d++)
            LaborDivisions[d] = new List<Workplace>();

    }

    public void InstantiateLabor() {

        Labor = new int[(int)LaborDivision.END];
        LaborMax = new int[Labor.Length];
        LaborDivisions = new List<Workplace>[Labor.Length];

        for (int d = 0; d < Labor.Length; d++)
            LaborDivisions[d] = new List<Workplace>();

    }

    //sum of all laborers
    public int workforce() {
        int sum = 0;
        foreach (int i in Labor)
            sum += i;
        return sum;
    }

    public void AddLaborReq(int d, int num) {
        LaborMax[d] += num;
        if (Population > workforce()) {

            //if the number of Workers needed is greater than unempoyed, add number of unemployed people
            if (LaborNeededAt(d) >= Unemployed)
                Labor[d] += Unemployed;

            //if the number of Workers needed is less than or equal to the num of unemployed, add as much as needed
            else if (LaborNeededAt(d) < Unemployed)
                Labor[d] += LaborNeededAt(d);

        }
        UpdateUnemployment();
        CalculateWorkers();
    }

    public void RemoveLaborReq(int d, int num) {
        LaborMax[d] -= num;

        if (LaborNeededAt(d) < 0) {
            Labor[d] = LaborMax[d];
        }
        UpdateUnemployment();
        CalculateWorkers();
    }

    public void UpdateUnemployment() {
        for (int d = 0; d < (int)LaborDivision.END; d++)
            if (Population > workforce()) {

                //if the number of Workers needed is greater than unempoyed, add number of unemployed people
                if (LaborNeededAt(d) >= Unemployed)
                    Labor[d] += Unemployed;

                //if the number of Workers needed is less than or equal to the num of unemployed, add as much as needed
                else if (LaborNeededAt(d) < Unemployed)
                    Labor[d] += LaborNeededAt(d);

            }
    }

	public void CalculateWorkers() {

		for (int d = 0; d < (int)LaborDivision.END; d++) {

			float totalAccess = 0;
			//float leftoverWorkers = 0;
			int workersToAllocate = Labor[d];
			SimplePriorityQueue<Workplace, int> pq = new SimplePriorityQueue<Workplace, int>();

			for (int b = 0; b < LaborDivisions[d].Count; b++) {

				Workplace w = LaborDivisions[d][b];
				int localAccess = w.Access;
				pq.Enqueue(w, localAccess);
				totalAccess += localAccess;

			}


			//NEW SYSTEM BELOW
			foreach (Workplace w in pq) {

				if (w.Access == 0)
					continue;
				if (!w.ActiveStaff)
					continue;

				//first allocate amount of workers appropriate for the building's relative access
				int workersWant = w.WorkersNeeded;
				int workersCanHave = (int)((float)Labor[d] * w.Access / totalAccess); //try switching Labor[d] with workersToAllocate

				//if this building can have more workers than it can have, don't let it do that
				if (workersCanHave > workersWant)
					workersCanHave = workersWant;

				//Debug.Log(w + " gets " + workersCanHave + " out of " + workersToAllocate);

				w.Workers = workersCanHave;
				workersToAllocate -= workersCanHave;

			}

			foreach (Workplace w in pq) {

				//if there are more workers to allocate, if a building has less workers than it can have
				if (workersToAllocate > 0 && w.Workers < w.WorkersNeeded && w.Access > 0) {

					int workersWant = w.WorkersNeeded - w.Workers;
					if (workersWant > workersToAllocate)
						workersWant = workersToAllocate;
					w.Workers += workersWant;
					workersToAllocate -= workersWant;

				}
			}
		}
	}

	public void SetAmountToAllocate(float n) {
        AllocateWorkers = (int)n;
    }

    //add people to total pop and to workforce
    public void AddPopulation(int num) {
        Population += num;
        for (int d = 0; d < (int)LaborDivision.END; d++) {

            if (num <= LaborNeededAt(d)) {
                Labor[d] += num;
                num = 0;
            }

            else if (num > LaborNeededAt(d)) {
                num -= LaborNeededAt(d);
                Labor[d] = LaborMax[d];
            }

            if (num == 0)
                break;
        }
        UpdateUnemployment();
        CalculateWorkers();
    }

    //remove people from total pop and workforce
    public void RemovePopulation(int num) {
        int currunemployed = Unemployed;

        Population -= num;

        if (num > currunemployed)
            num -= currunemployed;

        for (int d = (int)LaborDivision.END - 1; d > -1; d--) {

            if (num >= Labor[d]) {
                num -= Labor[d];
                Labor[d] = 0;
            }
            else if (num < Labor[d]) {
                Labor[d] -= num;
                num = 0;
            }

            if (num == 0)
                break;
        }
        UpdateUnemployment();
        CalculateWorkers();
    }

}
