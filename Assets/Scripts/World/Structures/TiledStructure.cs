﻿using UnityEngine;
using System.Collections;

public class TiledStructure : Structure {

    [Header("Tiled Structure")]
    public GameObject fourway;
    public GameObject threeway;
    public GameObject twoway;
    public GameObject oneway;
    public GameObject corner;
    public GameObject noway;

    public int Neighbors { get; set; }

    public void Update() {
        UpdateTiling();
    }

    public void UpdateTiling() {
        int n = FindNeighbors();
        if (Neighbors != n && n != 0) {
            Neighbors = n;
            UpdateGraphic();
        }
    }

    public virtual int FindNeighbors() {
        //int n = 0;
        //if (core.IsRoadAt(x - 1, y))
        //    n += 1;
        //if (core.IsRoadAt(x, y + 1))
        //    n += 2;
        //if (core.IsRoadAt(x + 1, y))
        //    n += 4;
        //if (core.IsRoadAt(x, y - 1))
        //    n += 8;
        //return n;

        return 0;
    }

    public void UpdateGraphic() {
        
        int neighbors = FindNeighbors();

        if (threeway == null || corner == null || fourway == null || twoway == null || oneway == null || noway == null)
            return;
        threeway.SetActive(false);
        corner.SetActive(false);
        fourway.SetActive(false);
        twoway.SetActive(false);
        oneway.SetActive(false);
        noway.SetActive(false);

        //Cross roads
        if (neighbors == 15) {
            fourway.SetActive(true);
            transform.eulerAngles = new Vector3(0, Random.Range(0, 4) * 90, 0);
        }

        //T-shaped roads
        else if (neighbors == 11) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (neighbors == 13) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (neighbors == 14) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (neighbors == 7) {
            threeway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //Corner roads
        else if (neighbors == 9) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (neighbors == 12) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (neighbors == 6) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (neighbors == 3) {
            corner.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }

        //Straight roads
        else if (neighbors == 10) {
            twoway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (neighbors == 5) {
            twoway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        //End roads
        else if (neighbors == 1) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (neighbors == 2) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (neighbors == 4) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (neighbors == 8) {
            oneway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 270, 0);
        }

        //default
        else {
            noway.SetActive(true);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

}
