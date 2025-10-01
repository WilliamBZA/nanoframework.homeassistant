namespace TimeTrigger;

using NanoFramework.HomeAssistant;
using System;
using System.Threading;

public class TimeSwitch
{
    public delegate void StateChangeAction(bool isOn);

    public TimeSwitch(string onTime, string offTime, StateChangeAction stateChangeCallback)
    {
        this.onTimeString = onTime;
        this.offTimeString = offTime;
        this.stateChangeCallback = stateChangeCallback;

        UpdateTodayTimes();
        CalculateIsOn();

        previousState = IsOn;
    }

    private void UpdateTodayTimes()
    {
        var now = DateTime.UtcNow.AddHours(2);
        var today = now.Date;
        onTime = today.Add(StringExtentionMethods.ParseTimeString(onTimeString));
        offTime = today.Add(StringExtentionMethods.ParseTimeString(offTimeString));

        if (onTime < now && offTime < now)
        {
            onTime = onTime.AddDays(1);
            offTime = offTime.AddDays(1);
        }
    }

    void CalculateIsOn()
    {
        var now = DateTime.UtcNow.AddHours(2).AddSeconds(5);

        IsOn = now >= onTime && now < offTime;

        if (now.Date > onTime.Date)
        {
            UpdateTodayTimes();
        }
    }

    public void Start()
    {
        if (timer != null)
        {
            Console.WriteLine($"OnTime: {onTime} OffTime: {offTime}");
            timer.Change(CalculateTimerInterval(), Timeout.Infinite);
        }
        else
        {
            Console.WriteLine($"OnTime: {onTime} OffTime: {offTime}");
            timer = new Timer(TimerCallback, null, CalculateTimerInterval(), Timeout.Infinite);
        }
    }

    private void TimerCallback(object state)
    {
        CalculateIsOn();

        if (stateChangeCallback != null && previousState != IsOn)
        {
            previousState = IsOn;
            stateChangeCallback(IsOn);
        }

        timer.Change(CalculateTimerInterval(), Timeout.Infinite);
        Console.WriteLine($"OnTime: {onTime} OffTime: {offTime}");
    }

    private int CalculateTimerInterval()
    {
        var now = DateTime.UtcNow.AddHours(2);
        var timeUntilOn = (int)(onTime - now).TotalMilliseconds;
        var timeUntilOff = (int)(offTime - now).TotalMilliseconds;
        var timeUntilOneMinute = 60000;

        if (timeUntilOn < 0 && timeUntilOff < 0)
        {
            UpdateTodayTimes();
            timeUntilOn = (int)(onTime - now).TotalMilliseconds;
            timeUntilOff = (int)(offTime - now).TotalMilliseconds;
        }

        if (timeUntilOn > 0)
        {
            return timeUntilOn > timeUntilOneMinute ? timeUntilOneMinute : timeUntilOn;
        }

        if (timeUntilOff > 0)
        {
            return timeUntilOff > timeUntilOneMinute ? timeUntilOneMinute : timeUntilOff;
        }

        return 1000;
    }

    public void UpdateOffTime(string newOffTime)
    {
        offTimeString = newOffTime;

        UpdateTodayTimes();
        TimerCallback(null);
        Start();
    }

    public void UpdateOnTime(string newOnTime)
    {
        onTimeString = newOnTime;

        UpdateTodayTimes();
        TimerCallback(null);
        Start();
    }

    public bool IsOn { get; private set; }

    bool previousState;
    private string onTimeString;
    private string offTimeString;
    private DateTime onTime;
    private DateTime offTime;
    private Timer timer;

    private readonly StateChangeAction stateChangeCallback;
}