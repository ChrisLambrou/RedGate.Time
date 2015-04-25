using System;
using NUnit.Framework;
using RedGate.Time.Test;

namespace RedGate.Time.Tests
{
    public abstract class TestTimeServiceTestsBase
    {
        protected TestTimeService TimeService;
        protected readonly DateTime ServiceStartTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [SetUp]
        public virtual void SetUp()
        {
            TimeService = new TestTimeService(ServiceStartTime, TimeSpan.FromMilliseconds(50));
        }

        [TearDown]
        public void TearDown()
        {
            TimeService.Dispose();
        }
    }
}
