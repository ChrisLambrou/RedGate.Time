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
    }
}
