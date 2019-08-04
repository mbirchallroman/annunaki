using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSave {

    public Node3d position, rotation;

    public CameraSave(CameraController cc) {

        position = new Node3d(cc.transform.position);
        rotation = new Node3d(cc.transform.eulerAngles);

    }

}

public class CameraController : MonoBehaviour {

    public float lowerClamp = 3;
    public float upperClamp = 25;

    public Vector3 lastFramePosition;
    public Vector3 currFramePosition;
    public Vector3 currWorldPosition;
    public Vector3 lastWorldPosition;
    public Camera mapCamera;

    public void Load(CameraSave cc) {

        transform.position = cc.position.GetVector3();
        transform.eulerAngles = cc.rotation.GetVector3();

    }

    void Start() {

        mapCamera.orthographicSize = 8;

    }

    // Update is called once per frame
    void Update() {

        UpdateCameraDrag();
        ScreenZoom();

    }

    public virtual void UpdateCameraDrag() {

        //record current frame and world position
        currFramePosition = Input.mousePosition;
        currWorldPosition = Camera.main.ScreenToWorldPoint(currFramePosition);

        // Handle screen panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) {   // Right or Middle Mouse Button

            Vector3 diff = lastWorldPosition - currWorldPosition;
            Vector3 pos = transform.position;

            transform.position = diff + pos;

        }

        //record last frame and world position
        lastFramePosition = Input.mousePosition;
        lastWorldPosition = Camera.main.ScreenToWorldPoint(lastFramePosition);

    }

    public virtual void ScreenZoom() {
        mapCamera.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        mapCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, lowerClamp, upperClamp);
    }

    public void RotateCamera(float deg) {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += deg;
        transform.eulerAngles = rotation;
    }

}