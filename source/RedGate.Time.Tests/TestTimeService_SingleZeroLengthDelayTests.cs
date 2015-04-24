using System.Threading.Tasks;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_SingleZeroLengthDelayTests : TestTimeServiceTestsBase
    {
        private Task _delayTask;

        public override void SetUp()
        {
            base.SetUp();
            _delayTask = TimeService.Delay(0);
        }

        [Test]
        public void MustBeCompletedImmediately()
        {
            Assert.That(_delayTask.IsCompleted, Is.True);
        }

        [Test]
        public void MustNotBeImmediatelyCancelled()
        {
            Assert.That(_delayTask.IsCanceled, Is.False);
        }

        [Test]
        public void MustNotBeImmediatelyFaulted()
        {
            Assert.That(_delayTask.IsFaulted, Is.False);
        }
    }
}
