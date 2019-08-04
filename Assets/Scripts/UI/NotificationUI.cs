using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour {

    public Text desc;
    public Notification Event { get; set; }
    public NotificationController Controller { get; set; }
    public int Index { get; set; }

    private void Update() {

        desc.text = Event.desc;

    }

    public void CloseNotification() {

        Controller.Events.RemoveAt(Index);
        Destroy(gameObject);

    }

}
