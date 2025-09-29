using System;

namespace TestSwitch
{
    public class CurrentState
    {
        public bool IsOn { get; set; } = false;
        public string OnTime { get; set; } = "11:30";
        public string OffTime { get; set; } = "12:30";
    }
}