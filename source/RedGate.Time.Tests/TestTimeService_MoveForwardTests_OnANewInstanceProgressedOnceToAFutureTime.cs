using System;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_MoveForwardTests_OnANewInstanceProgressedOnceToAFutureTime :
        TestTimeService_MoveForwardTestsBase
    {
        public override void SetUp()
        {
            base.SetUp();
            TimeService.MoveForwardBy(TimeSpan.FromSeconds(1));
            TestStartTime += TimeSpan.FromSeconds(1);
        }
    }
}
