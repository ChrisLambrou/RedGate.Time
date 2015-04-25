using System;
using System.Threading;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    public abstract class TestTimeService_MoveForwardTestsBase : TestTimeServiceTestsBase
    {
        protected DateTime TestStartTime;

        public override void SetUp()
        {
            base.SetUp();
            TestStartTime = ServiceStartTime;
        }

        [Test]
        public void UtcNow_ShouldReturnAUtcTime()
        {
            Assert.That(TimeService.UtcNow.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void UtcNow_ShouldReturnTheStartTime()
        {
            Assert.That(TimeService.UtcNow, Is.EqualTo(TestStartTime));
        }

        [Test]
        public void UtcNow_ShouldReturnTheStartTime_EvenAfterARealTimeDelay()
        {
            Thread.Sleep(10);
            Assert.That(TimeService.UtcNow, Is.EqualTo(TestStartTime));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void MoveForwardByMilliseconds_ShouldProgressUtcNow(int millisecondsDelta)
        {
            TimeService.MoveForwardBy(millisecondsDelta);
            Assert.That(
                TimeService.UtcNow,
                Is.EqualTo(TestStartTime + TimeSpan.FromMilliseconds(millisecondsDelta)));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void MoveForwardByMilliseconds_WhenPassedANegativeTimeSpan_ShouldError(int millisecondsDelta)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.MoveForwardBy(millisecondsDelta));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void MoveForwardByTimeSpan_ShouldProgressUtcNow(int millisecondsDelta)
        {
            var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
            TimeService.MoveForwardBy(delta);
            Assert.That(TimeService.UtcNow, Is.EqualTo(TestStartTime + delta));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void MoveForwardByTimeSpan_WhenPassedANegativeTimeSpan_ShouldError(int millisecondsDelta)
        {
            var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.MoveForwardBy(delta));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void MoveForwardToMilliseconds_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
        {
            millisecondsOffsetFromStartTime += (int) (TestStartTime - ServiceStartTime).TotalMilliseconds;
            TimeService.MoveForwardTo(millisecondsOffsetFromStartTime);
            Assert.That(
                TimeService.UtcNow,
                Is.EqualTo(ServiceStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime)));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void MoveForwardToMilliseconds_WithANegativeTimeSpan_ShouldError(
            int millisecondsOffsetFromStartTime)
        {
            millisecondsOffsetFromStartTime += (int) (TestStartTime - ServiceStartTime).TotalMilliseconds;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => TimeService.MoveForwardTo(millisecondsOffsetFromStartTime));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void MoveForwardToTimeSpan_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
        {
            var offsetFromStartTime = TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime) +
                                      (TestStartTime - ServiceStartTime);
            TimeService.MoveForwardTo(offsetFromStartTime);
            Assert.That(TimeService.UtcNow, Is.EqualTo(ServiceStartTime + offsetFromStartTime));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void MoveForwardToTimeSpan_WithANegativeTimeSpan_ShouldError(int millisecondsOffsetFromStartTime)
        {
            var offsetFromStartTime = TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime) +
                                      (TestStartTime - ServiceStartTime);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.MoveForwardTo(offsetFromStartTime));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void MoveForwardToDateTime_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
        {
            var newTime = TestStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime);
            TimeService.MoveForwardTo(newTime);
            Assert.That(TimeService.UtcNow, Is.EqualTo(newTime));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void MoveForwardToDateTime_WithANegativeTimeSpan_ShouldError(int millisecondsOffsetFromStartTime)
        {
            var newTime = TestStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeService.MoveForwardTo(newTime));
        }
    }
}
