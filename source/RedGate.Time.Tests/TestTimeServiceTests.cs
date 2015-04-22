using System;
using System.Threading;
using NUnit.Framework;
using RedGate.Time.Test;

namespace RedGate.Time.Tests
{
    public static class TestTimeServiceTests
    {
        [TestFixture]
        public class ANewInstance : MoveForwardTestsBase
        {
        }

        [TestFixture]
        public class ANewInstanceProgressedOnceToAFutureTime : MoveForwardTestsBase
        {
            public override void SetUp()
            {
                base.SetUp();

                _timeService.MoveForwardBy(TimeSpan.FromSeconds(1));
                _testStartTime += TimeSpan.FromSeconds(1);
            }
        }

        [TestFixture]
        public class AnInstanceWithPendingDelayTasks : MoveForwardTestsBase
        {
            public override void SetUp()
            {
                base.SetUp();

                _timeService.MoveForwardBy(TimeSpan.FromSeconds(0.5));
                _timeService.Delay(TimeSpan.FromSeconds(2));
                _timeService.MoveForwardBy(TimeSpan.FromSeconds(0.5));
                _testStartTime += TimeSpan.FromSeconds(1);
            }
        }

        public abstract class MoveForwardTestsBase
        {
            private readonly DateTime _serviceStartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            protected DateTime _testStartTime;
            protected TestTimeService _timeService;

            [SetUp]
            public virtual void SetUp()
            {
                _timeService = new TestTimeService(_serviceStartTime);
                _testStartTime = _serviceStartTime;
            }

            [TearDown]
            public void TearDown()
            {
                _timeService.Dispose();
            }

            [Test]
            public void UtcNow_ShouldReturnTheStartTime()
            {
                Assert.That(_timeService.UtcNow, Is.EqualTo(_testStartTime));
            }

            [Test]
            public void UtcNow_ShouldReturnTheStartTime_EvenAfterARealTimeDelay()
            {
                Thread.Sleep(10);
                Assert.That(_timeService.UtcNow, Is.EqualTo(_testStartTime));
            }

            [Test]
            [TestCase(1500, TestName = "Non-zero")]
            [TestCase(0, TestName = "Zero")]
            public void MoveForwardByMilliseconds_ShouldProgressUtcNow(int millisecondsDelta)
            {
                _timeService.MoveForwardBy(millisecondsDelta);
                Assert.That(_timeService.UtcNow, Is.EqualTo(_testStartTime + TimeSpan.FromMilliseconds(millisecondsDelta)));
            }

            [Test]
            [TestCase(-1, TestName = "-1")]
            [TestCase(-10, TestName = "-10")]
            public void MoveForwardByMilliseconds_WhenPassedANegativeTimeSpan_ShouldError(int millisecondsDelta)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardBy(millisecondsDelta));
            }

            [Test]
            [TestCase(1500, TestName = "Non-zero")]
            [TestCase(0, TestName = "Zero")]
            public void MoveForwardByTimeSpan_ShouldProgressUtcNow(int millisecondsDelta)
            {
                var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
                _timeService.MoveForwardBy(delta);
                Assert.That(_timeService.UtcNow, Is.EqualTo(_testStartTime + delta));
            }

            [Test]
            [TestCase(-1, TestName = "-1")]
            [TestCase(-10, TestName = "-10")]
            public void MoveForwardByTimeSpan_WhenPassedANegativeTimeSpan_ShouldError(int millisecondsDelta)
            {
                var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
                Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardBy(delta));
            }

            [Test]
            [TestCase(1500, TestName = "Non-zero")]
            [TestCase(0, TestName = "Zero")]
            public void MoveForwardToMilliseconds_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
            {
                millisecondsOffsetFromStartTime += (int) (_testStartTime - _serviceStartTime).TotalMilliseconds;
                _timeService.MoveForwardTo(millisecondsOffsetFromStartTime);
                Assert.That(
                    _timeService.UtcNow,
                    Is.EqualTo(_serviceStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime)));
            }

            [Test]
            [TestCase(-1, TestName = "-1")]
            [TestCase(-10, TestName = "-10")]
            public void MoveForwardToMilliseconds_WithANegativeTimeSpan_ShouldError(
                int millisecondsOffsetFromStartTime)
            {
                millisecondsOffsetFromStartTime += (int) (_testStartTime - _serviceStartTime).TotalMilliseconds;
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => _timeService.MoveForwardTo(millisecondsOffsetFromStartTime));
            }

            [Test]
            [TestCase(1500, TestName = "Non-zero")]
            [TestCase(0, TestName = "Zero")]
            public void MoveForwardToTimeSpan_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
            {
                var offsetFromStartTime = TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime) +
                                          (_testStartTime - _serviceStartTime);
                _timeService.MoveForwardTo(offsetFromStartTime);
                Assert.That(_timeService.UtcNow, Is.EqualTo(_serviceStartTime + offsetFromStartTime));
            }

            [Test]
            [TestCase(-1, TestName = "-1")]
            [TestCase(-10, TestName = "-10")]
            public void MoveForwardToTimeSpan_WithANegativeTimeSpan_ShouldError(int millisecondsOffsetFromStartTime)
            {
                var offsetFromStartTime = TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime) +
                                          (_testStartTime - _serviceStartTime);
                Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardTo(offsetFromStartTime));
            }

            [Test]
            [TestCase(1500, TestName = "Non-zero")]
            [TestCase(0, TestName = "Zero")]
            public void MoveForwardToDateTime_ShouldProgressUtcNow(int millisecondsOffsetFromStartTime)
            {
                var newTime = _testStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime);
                _timeService.MoveForwardTo(newTime);
                Assert.That(_timeService.UtcNow, Is.EqualTo(newTime));
            }

            [Test]
            [TestCase(-1, TestName = "-1")]
            [TestCase(-10, TestName = "-10")]
            public void MoveForwardToDateTime_WithANegativeTimeSpan_ShouldError(int millisecondsOffsetFromStartTime)
            {
                var newTime = _testStartTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime);
                Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardTo(newTime));
            }
        }
    }
}
