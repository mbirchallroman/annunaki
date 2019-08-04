using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TimeSave {

    public float timeDelta;
    public int days, weeks, months, years;

    public TimeSave(TimeController tc) {
        
        timeDelta = tc.TimeDelta;
        days = tc.Days;
        weeks = tc.Weeks;
        months = tc.Months;
        years = tc.Years;

    }

}

public class TimeController : MonoBehaviour {

    public Text timeLabel;
    public GameObject pause;
    public GameObject play;

    public bool IsPaused { get; set; }

    public float TimeDelta { get; set; }
    public int Days { get; set; }
    public int Weeks { get; set; }
    public int Months { get; set; }
    public int Seasons { get; set; }
    public int Years { get; set; }

    public static int DayTime { get { return 1; } }
    public static int WeekTime { get { return 4; } }
    public static int MonthTime { get { return 4; } }
    public static int SeasonTime { get { return 3; } }
    public static int YearTime { get { return 4; } }

    public static int DaysInAMonth { get { return WeekTime * MonthTime; } }
    public static int DaysInASeason { get { return DaysInAMonth * SeasonTime; } }
    public static int DaysInAYear { get { return DaysInASeason * YearTime; } }

    public float timescale { get; set; }
    public float maxSpeed { get { return 4f; } }
    public float speedUnit { get { return maxSpeed / 10; } }
    public float startSpeed { get { return 8 * speedUnit; } }

    public Month currentMonth { get { return (Month)(Months + Seasons * SeasonTime); } }
    public Season currentSeason { get { return (Season)Seasons; } }

    public void UpdateTimeLabel() {

        timeLabel.text = ToString();

    }

    public override string ToString() {
        return (1 + Days + Weeks * WeekTime) + " " + (Month)(Months + Seasons * SeasonTime) + ", Year " + (Years + 1);
    }

    public void Load(TimeSave w) {

        TimeDelta = w.timeDelta;
        Days = w.days;
        Weeks = w.weeks;
        Months = w.months;
        Years = w.years;

    }

    void Start() {
        //set starting timescale
        timescale = startSpeed;
        Time.timeScale = timescale;

        //update label
        UpdateTimeLabel();
    }

    void Update () {

        if (!IsPaused)
            Time.timeScale = timescale;

        TimeDelta += Time.deltaTime;

        if (TimeDelta >= DayTime) {

            //update label
            UpdateTimeLabel();

            //convert timedelta to day
            TimeDelta = 0;
            Days++;

            //convert days to week
            if(Days >= WeekTime) {

                Days = 0;
                Weeks++;
            }

            //convert weeks to month
            if (Weeks >= MonthTime) {

                Weeks = 0;
                Months++;
            }

            //convert months to year
            if (Months >= SeasonTime) {

                Months = 0;
                Seasons++;

            }

            //convert months to year
            if (Seasons >= YearTime) {

                Seasons = 0;
                Years++;

            }

        }

	}

    public void Pause() {
        Time.timeScale = 0;
        IsPaused = true;
    }
    public void Play() {
        Time.timeScale = timescale;
        IsPaused = false;
    }
    public void ChangeTimeScale(float t) {
        timescale = (t / 10) * maxSpeed;
        //isPaused = t == 0;
    }

}
