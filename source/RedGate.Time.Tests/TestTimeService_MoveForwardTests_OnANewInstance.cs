using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_MoveForwardTests_OnANewInstance : TestTimeService_MoveForwardTestsBase
    {
        public override void SetUp()
        {
            base.SetUp();
            TestStartTime = ServiceStartTime;
        }
    }
}
