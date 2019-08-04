using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaborAllocationMenu : MonoBehaviour {

    public int AllocateNum { get; set; }
    public Text allocationLabel;
    public Text populationLabel;
    public LaborController laborController;
    public UIObjectDatabase uiObjectDatabase;
    public GameObject grid;
    public List<GameObject> Divisions { get; set; }

    private void Start() {

        AllocateNum = 5;

    }

    public void NewButtons() {

        Divisions = new List<GameObject>();

        for (int x = 0; x < (int)LaborDivision.END; x++) {

            GameObject go = Instantiate(uiObjectDatabase.laborDivision);
            go.transform.SetParent(grid.transform);
            Divisions.Add(go);

            LaborDivisionUI ld = go.GetComponent<LaborDivisionUI>();
            ld.laborController = laborController;
            ld.allocationMenu = this;
            ld.index = x;

            if (laborController.LaborMax[x] == 0)
                go.SetActive(false);


        }

    }

    private void Update() {

        if (laborController.Labor != null && Divisions == null)
            NewButtons();

        else
            for (int x = 0; x < (int)LaborDivision.END; x++) {

                if (laborController.LaborMax[x] == 0 && Divisions[x].activeSelf)
                    Divisions[x].SetActive(false);
                else if(laborController.LaborMax[x] != 0 && !Divisions[x].activeSelf)
                    Divisions[x].SetActive(true);

            }

        

        allocationLabel.text = "Allocate " + AllocateNum + " workers";
        populationLabel.text = laborController.Unemployed + " Unemployed";

    }

    public void ChangeAllocateNum(float n) {

        AllocateNum = (int)n;

    }

}
