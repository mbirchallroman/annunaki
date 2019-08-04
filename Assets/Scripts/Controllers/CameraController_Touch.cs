using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController_Touch : CameraController {


    public TouchController touchController;

    public float zoomSpeed = 1;

    public bool WasOnUI { get; set; }

    public override void UpdateCameraDrag() {

        if (Input.touchCount != 1)
            return;

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
            return;

        Touch touch = Input.GetTouch(0);

        //record current frame and world position
        currFramePosition = touch.position;
        currWorldPosition = Camera.main.ScreenToWorldPoint(currFramePosition);

        // Handle screen panning
        if (touch.phase == TouchPhase.Moved && !touchController.PlacingStructure) {

            Vector3 diff = lastWorldPosition - currWorldPosition;
            Vector3 pos = transform.position;

            transform.position = diff + pos;

        }

        //record last frame and world position
        lastFramePosition = touch.position;
        lastWorldPosition = Camera.main.ScreenToWorldPoint(lastFramePosition);

    }

    public override void ScreenZoom() {

        if (Input.touchCount != 2)
            return;

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
            return;

        Touch t1 = Input.GetTouch(0);
        Touch t2 = Input.GetTouch(1);

        Vector2 t1prev = t1.position - t1.deltaPosition;
        Vector2 t2prev = t2.position - t2.deltaPosition;

        float touchDeltaMag = (t1.position - t2.position).magnitude;
        float prevTouchDeltaMag = (t1prev - t2prev).magnitude;

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        if (!touchController.PlacingStructure) {

            mapCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
            mapCamera.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);

        }


    }


}
