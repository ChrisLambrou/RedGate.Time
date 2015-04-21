using System;
using NUnit.Framework;
using RedGate.Time.Test;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeServiceTests
    {
        private readonly DateTime _startTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private TestTimeService _timeService;

        [SetUp]
        public void SetUp()
        {
            _timeService = new TestTimeService(_startTime);
        }

        [Test]
        public void OnANewInstance_ConstructedWithASpecificStartTime_UtcNowShouldReturnThatTime()
        {
            Assert.That(_timeService.UtcNow, Is.EqualTo(_startTime));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void OnANewInstance_MoveForwardByMilliseconds_ShouldProgressUtcNow(int millisecondsDelta)
        {
            _timeService.MoveForwardBy(millisecondsDelta);
            Assert.That(_timeService.UtcNow, Is.EqualTo(_startTime + TimeSpan.FromMilliseconds(millisecondsDelta)));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void OnANewInstance_MoveForwardByNegativeMilliseconds_ShouldError(int millisecondsDelta)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardBy(millisecondsDelta));
        }

        [Test]
        [TestCase(1500, TestName = "Non-zero")]
        [TestCase(0, TestName = "Zero")]
        public void OnANewInstance_MoveForwardByTimeSpan_ShouldProgressUtcNow(int millisecondsDelta)
        {
            var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
            _timeService.MoveForwardBy(delta);
            Assert.That(_timeService.UtcNow, Is.EqualTo(_startTime + delta));
        }

        [Test]
        [TestCase(-1, TestName = "-1")]
        [TestCase(-10, TestName = "-10")]
        public void OnANewInstance_MoveForwardByNegativeTimeSpan_ShouldError(int millisecondsDelta)
        {
            var delta = TimeSpan.FromMilliseconds(millisecondsDelta);
            Assert.Throws<ArgumentOutOfRangeException>(() => _timeService.MoveForwardBy(delta));
        }
    }
}
