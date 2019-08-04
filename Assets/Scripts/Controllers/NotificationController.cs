using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationController : MonoBehaviour {

    public NotificationUI[] buttons = new NotificationUI[(int)NotificationType.END];
    public List<Notification> Events { get; set; }
    public GameObject eventListGrid;
    public UIObjectDatabase uiDatabase;

    int bannerDisplayTime;

    public void Start() {

        CloseBanners();

    }

    float timeDelta = 0;
    private void Update() {

        timeDelta += Time.deltaTime;

        if(timeDelta >= 0) {

            timeDelta = 0;
            UpdateBanner();

        }

    }

    public void FreshEvents() {

        Events = new List<Notification>();

    }

    public void NewNotification(Notification n) {

        if (eventListGrid == null)
            return;

        Events.Add(n);
        bannerDisplayTime = 5;
        UpdateBanner();

        GameObject go = Instantiate(uiDatabase.notificationListItem);
        go.transform.SetParent(eventListGrid.transform);
        go.transform.SetAsFirstSibling();
        go.transform.localScale = new Vector3(1, 1, 1);

        NotificationUI banner = go.GetComponent<NotificationUI>();
        banner.gameObject.SetActive(true);
        banner.Controller = this;
        banner.Event = n;
        banner.Index = Events.Count - 1;

    }

    public void UpdateBanner() {

        CloseBanners();
        
        if(bannerDisplayTime > 0) {

            Notification n = Events[Events.Count - 1];
            NotificationUI banner = buttons[(int)n.type];
            banner.gameObject.SetActive(true);
            banner.Controller = this;
            banner.Event = n;
            banner.Index = Events.Count - 1;
            bannerDisplayTime--;

        }

    }

    public void CloseBanners() {

        if (buttons == null)
            return;
        foreach (NotificationUI ui in buttons)
            ui.gameObject.SetActive(false);

    }

    public void TestNotification() {

        NotificationType t = (NotificationType)(Random.Range(0, (int)NotificationType.END));
        Notification n = new Notification(t, "Test notification!");
        NewNotification(n);

    }


}

[System.Serializable]
public class Notification {

    public NotificationType type;
    public string desc;

    public Notification(NotificationType t, string d) {

        type = t;
        desc = d;

    }

}