using System;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_MoveForwardTests_OnAnInstanceWithAPendingDelayTask :
        TestTimeService_MoveForwardTestsBase
    {
        public override void SetUp()
        {
            base.SetUp();

            TimeService.MoveForwardBy(TimeSpan.FromSeconds(0.5));
            TimeService.Delay(TimeSpan.FromSeconds(2));
            TimeService.MoveForwardBy(TimeSpan.FromSeconds(0.5));
            TestStartTime += TimeSpan.FromSeconds(1);
        }
    }
}
