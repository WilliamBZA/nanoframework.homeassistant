using nanoFramework.TestFramework;
using System;
using System.Threading;
using TimeTrigger;

namespace SwitchTests
{
    [TestClass]
    public class TimeSwitchTests
    {
        [TestMethod]
        public void If_Time_is_between_on_and_off_then_switch_is_on()
        {
            var now = DateTime.UtcNow;
            var onTime = now.AddMinutes(-1).ToString("HH:mm");
            var offTime = now.AddMinutes(1).ToString("HH:mm");
            var timeManager = new TimeSwitch(onTime, offTime, null);

            Assert.IsTrue(timeManager.IsOn);
        }

        [TestMethod]
        public void If_Time_is_not_between_on_and_off_then_switch_is_off()
        {
            var now = DateTime.UtcNow;
            var onTime = now.AddMinutes(1).ToString("HH:mm");
            var offTime = now.AddMinutes(3).ToString("HH:mm");
            var timeManager = new TimeSwitch(onTime, offTime, null);

            Assert.IsFalse(timeManager.IsOn);
        }

        [TestMethod]
        public void If_Time_moves_from_before_to_during_then_switch_turns_on()
        {
            var now = DateTime.UtcNow;
            var onTime = now.AddMinutes(1).ToString("HH:mm");
            var offTime = now.AddMinutes(2).ToString("HH:mm");
            var timeManager = new TimeSwitch(onTime, offTime, null);
            timeManager.Start();

            Assert.IsFalse(timeManager.IsOn);

            Thread.Sleep(61000);
            Assert.IsTrue(timeManager.IsOn);
        }

        [TestMethod]
        public void Changing_the_on_time_uses_the_new_time()
        {
            var now = DateTime.UtcNow;
            var onTime = now.AddMinutes(10).ToString("HH:mm");
            var offTime = now.AddMinutes(12).ToString("HH:mm");
            var timeManager = new TimeSwitch(onTime, offTime, null);
            timeManager.Start();

            Assert.IsFalse(timeManager.IsOn);

            timeManager.UpdateOnTime(now.AddMinutes(1).ToString("HH:mm"));

            Thread.Sleep(61000);
            Assert.IsTrue(timeManager.IsOn);
        }
    }
}