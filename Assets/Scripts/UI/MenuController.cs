using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    public bool pauseWhenOpen;
    public bool dontOpenWhenPaused;
    public bool closeIfAlreadyOpen = true;
    public GameObject menu;
    public TimeController timeController;
    public TooltipController tooltipController;
    public ModeController modeController;
    public List<Mode> openOnModes;

    public GameObject mainPage;
    public List<GameObject> pages;

    public List<GameObject> hideUI;
    public Dictionary<GameObject, bool> wasVisible = new Dictionary<GameObject, bool>();

    public bool wasPaused { get; set; }
    public bool wasOpen { get; set; }

    private void Start() {
        
        //CloseMenu();

    }

    public virtual void OpenMenu() {

        if (mainPage != null)
            OpenMenu(mainPage);
    }

    public virtual void OpenMenu(GameObject page) {

        if (timeController != null)
            if (Time.timeScale == 0 && dontOpenWhenPaused)
                return;

        //only open if correct mode
        if(modeController != null)
            if (!openOnModes.Contains(modeController.currentMode))
                return;
        

        //if menu was already open
        if (menu.activeSelf) {

            wasOpen = true;
            
        }

        //if should close when button is pressed again, and the intended page is not the main page
        if (page.activeSelf && closeIfAlreadyOpen && page != mainPage) {

            CloseMenu();
            return;

        }



        //close all pages but desired page
        foreach (GameObject go in pages)
            go.SetActive(false);
        page.SetActive(true);

        //check if the game was paused before
        if(timeController != null) {

            if(!wasOpen)
                wasPaused = timeController.IsPaused;

            //if the game must be paused while open, pause it
            if (pauseWhenOpen)
                timeController.Pause();

        }
            
        menu.SetActive(true);

        if (tooltipController != null)
            tooltipController.SetText("");

        if(modeController != null)
            if (!openOnModes.Contains(modeController.currentMode))
                return;

        foreach (GameObject go in hideUI) {

            //add to dictionary which says if it was active or inactive when reached
            if (!wasOpen)
                wasVisible[go] = go.activeSelf;

            go.SetActive(false);

        }

    }

    public virtual void CloseMenu() {

        //if the game was supposed to be paused but was not paused before, start playing
        if (timeController != null && pauseWhenOpen && (!wasPaused || wasOpen)) {
            timeController.Play();
        }

        //otherwise reset setting
        else
            wasPaused = false;

        //close all pages
        foreach (GameObject go in pages)
            go.SetActive(false);

        menu.SetActive(false);

        foreach (GameObject go in hideUI)
            if (wasVisible.ContainsKey(go))
                if (wasVisible[go])
                    go.SetActive(true);

        wasOpen = false;

    }

}
