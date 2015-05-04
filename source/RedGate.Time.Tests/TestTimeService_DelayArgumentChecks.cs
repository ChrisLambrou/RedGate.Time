using System;
using System.Threading;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_DelayArgumentChecks : TestTimeServiceTestsBase
    {
        [Test]
        public void Delay_WhenCalledWithANegativeTimeSpan_ShouldRaiseAnException()
        {
            var delay = TimeSpan.FromMilliseconds(-2);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay));
        }

        [Test]
        public void Delay_WhenCalledWithANegativeTimeSpan_AndACancellationToken_ShouldRaiseAnException()
        {
            var delay = TimeSpan.FromMilliseconds(-2);
            var token = new CancellationToken();
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay, token));
        }

        [Test]
        public void Delay_WhenCalledWithANegativeTimeSpanInMilliseconds_ShouldRaiseAnException()
        {
            var delay = -2;
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay));
        }

        [Test]
        public void Delay_WhenCalledWithANegativeTimeSpanInMilliseconds_AndACancellationToken_ShouldRaiseAnException()
        {
            var delay = -2;
            var token = new CancellationToken();
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay, token));
        }

        [Test]
        public void Delay_WhenCalledWithATimeSpanGreaterThanIntMaxValue_ShouldRaiseAnException()
        {
            var delay = TimeSpan.FromMilliseconds(1 + (long) int.MaxValue);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay));
        }

        [Test]
        public void Delay_WhenCalledWithATimeSpanGreaterThanIntMaxValue_AndACancellationToken_ShouldRaiseAnException()
        {
            var delay = TimeSpan.FromMilliseconds(1 + (long) int.MaxValue);
            var token = new CancellationToken(); 
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.Delay(delay, token));
        }
    }
}