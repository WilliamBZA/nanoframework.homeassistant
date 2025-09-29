namespace TimeTrigger;

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
    }

    private void UpdateTodayTimes()
    {
        var now = DateTime.UtcNow.AddHours(2);
        var today = now.Date;
        onTime = today.Add(ParseTimeString(onTimeString));
        offTime = today.Add(ParseTimeString(offTimeString));

        if (onTime < now)
        {
            onTime = onTime.AddDays(1);
        }

        if (offTime < now || offTime < onTime)
        {
            offTime = offTime.AddDays(1);
        }
    }

    private TimeSpan ParseTimeString(string timeString)
    {
        var parts = timeString.Split(':');
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid time format. Expected HH:mm, got {timeString}");
        }

        if (!int.TryParse(parts[0], out int hours) || !int.TryParse(parts[1], out int minutes))
        {
            throw new ArgumentException($"Invalid time format. Expected HH:mm, got {timeString}");
        }

        if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
        {
            throw new ArgumentException($"Invalid time values. Hours must be 0-23, minutes must be 0-59");
        }

        return new TimeSpan(hours, minutes, 0);
    }

    void CalculateIsOn()
    {
        var now = DateTime.UtcNow.AddHours(2).AddSeconds(5);
        
        if (now.Date > onTime.Date)
        {
            UpdateTodayTimes();
        }

        IsOn = now >= onTime && now < offTime;
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

        if (stateChangeCallback != null)
        {
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

        if (timeUntilOn < 0 && timeUntilOff < 0)
        {
            UpdateTodayTimes();
            timeUntilOn = (int)(onTime - now).TotalMilliseconds;
            timeUntilOff = (int)(offTime - now).TotalMilliseconds;
        }

        if (timeUntilOn > 0)
        {
            return timeUntilOn;
        }

        if (timeUntilOff > 0)
        {
            return timeUntilOff;
        }

        return 1000;
    }

    public void UpdateOffTime(string newOffTime)
    {
        offTimeString = newOffTime;

        UpdateTodayTimes();
        CalculateIsOn();
        Start();
    }

    public void UpdateOnTime(string newOnTime)
    {
        onTimeString = newOnTime;

        UpdateTodayTimes();
        CalculateIsOn();
        Start();
    }

    public bool IsOn { get; private set; }

    private string onTimeString;
    private string offTimeString;
    private DateTime onTime;
    private DateTime offTime;
    private Timer timer;

    private readonly StateChangeAction stateChangeCallback;
}