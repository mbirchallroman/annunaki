using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaborDivisionUI : MonoBehaviour {


    public Text label;
    public Text numWorkers;
    public Button lessWorkers;
    public Button moreWorkers;

    public LaborController laborController { get; set; }
    public LaborAllocationMenu allocationMenu { get; set; }
    public int index { get; set; }

    private void Start() {

        label.text = (LaborDivision)index + "";

    }

    private void Update() {

        numWorkers.text = laborController.Labor[index] + "/" + laborController.LaborMax[index];

        //if amount of workers is more than or equal to what you're removing, allow player to remove
        lessWorkers.interactable = laborController.Labor[index] >= allocationMenu.AllocateNum;

        //if amount of workers needed is more than or equal to what you're adding, allow player to add
        moreWorkers.interactable = laborController.LaborNeededAt(index) >= allocationMenu.AllocateNum && allocationMenu.AllocateNum <= laborController.Unemployed;

    }

    public void RemoveWorkers() {

        if (laborController.Labor[index] >= allocationMenu.AllocateNum) {
            laborController.Labor[index] -= allocationMenu.AllocateNum;
            laborController.CalculateWorkers();
        }
            

    }

    public void AddWorkers() {
        
        laborController.Labor[index] += allocationMenu.AllocateNum;
        laborController.CalculateWorkers();
            

    }

}
