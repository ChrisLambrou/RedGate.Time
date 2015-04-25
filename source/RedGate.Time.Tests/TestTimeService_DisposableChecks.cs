using System;
using System.Threading;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_DisposableChecks : TestTimeServiceTestsBase
    {
        public override void SetUp()
        {
            base.SetUp();
            TimeService.Dispose();
        }

        private static void NoOp(object arg)
        {}

        [Test]
        public void Now_ShouldRaiseAnError()
        {
            Assert.Throws<ObjectDisposedException>(() => NoOp(TimeService.Now));
        }

        [Test]
        public void UtcNow_ShouldRaiseAnError()
        {
            Assert.Throws<ObjectDisposedException>(() => NoOp(TimeService.UtcNow));
        }

        [Test]
        public void Delay_FromTimeStamp_ShouldRaiseAnError()
        {
            var delay = TimeSpan.FromSeconds(1);
            Assert.Throws<ObjectDisposedException>(() => TimeService.Delay(delay));
        }

        [Test]
        public void Delay_FromTimeStampAndCancellationToken_ShouldRaiseAnError()
        {
            var delay = TimeSpan.FromSeconds(1);
            var cancellationToken = new CancellationTokenSource().Token;
            Assert.Throws<ObjectDisposedException>(() => TimeService.Delay(delay, cancellationToken));
        }

        [Test]
        public void Delay_FromMillisecondsDelay_ShouldRaiseAnError()
        {
            var delay = 1000;
            Assert.Throws<ObjectDisposedException>(() => TimeService.Delay(delay));
        }

        [Test]
        public void Delay_FromMillisecondsDelayAndCancellationToken_ShouldRaiseAnError()
        {
            var delay = 1000;
            var cancellationToken = new CancellationTokenSource().Token;
            Assert.Throws<ObjectDisposedException>(() => TimeService.Delay(delay, cancellationToken));
        }

        [Test]
        public void MoveForwardBy_TimeSpanDelta_ShouldRaiseAnError()
        {
            var delta = TimeSpan.FromSeconds(1);
            Assert.Throws<ObjectDisposedException>(() => TimeService.MoveForwardBy(delta));
        }

        [Test]
        public void MoveForwardBy_MillisecondsDelta_ShouldRaiseAnError()
        {
            var delta = 1000;
            Assert.Throws<ObjectDisposedException>(() => TimeService.MoveForwardBy(delta));
        }

        [Test]
        public void MoveForwardTo_MillisecondsSpanOffset_ShouldRaiseAnError()
        {
            var offsetFromStartTime = 1000;
            Assert.Throws<ObjectDisposedException>(() => TimeService.MoveForwardTo(offsetFromStartTime));
        }

        [Test]
        public void MoveForwardTo_TimeSpanOffset_ShouldRaiseAnError()
        {
            var offsetFromStartTime = TimeSpan.FromSeconds(1);
            Assert.Throws<ObjectDisposedException>(() => TimeService.MoveForwardTo(offsetFromStartTime));
        }

        [Test]
        public void MoveForwardTo_NewDateTime_ShouldRaiseAnError()
        {
            var newTime = ServiceStartTime + TimeSpan.FromSeconds(1);
            Assert.Throws<ObjectDisposedException>(() => TimeService.MoveForwardTo(newTime));
        }
    }
}