using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjWindow : MonoBehaviour {

    public Obj obj;
    public Text title;
    public WorldController WorldController { get; set; }
    public TimeController TimeController { get; set; }
    public bool WasPaused { get; set; }

    public virtual void Open() {

        title.text = obj.DisplayName;
        transform.localPosition = new Vector3(0, 0, 0);
        TimeController = WorldController.timeController;
        TimeController.Pause();

    }

    private void Update() {

        DoDuringUpdate();

    }

    public virtual void DoDuringUpdate() {

        

    }

    public void Close() {

        TimeController.Play();
        Destroy(gameObject);

    }

}
