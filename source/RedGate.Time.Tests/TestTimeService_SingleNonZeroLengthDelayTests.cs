using System.Threading.Tasks;
using NUnit.Framework;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class TestTimeService_SingleNonZeroLengthDelayTests : TestTimeServiceTestsBase
    {
        private Task _delayTask;

        public override void SetUp()
        {
            base.SetUp();
            _delayTask = TimeService.Delay(1000);
        }

        [Test]
        public void MustInitiallyNotBeCompleted()
        {
            Assert.That(_delayTask.IsCompleted, Is.False);
        }

        [Test]
        public void MustInitiallyNotBeCancelled()
        {
            Assert.That(_delayTask.IsCanceled, Is.False);
        }

        [Test]
        public void MustInitiallyNotBeFaulted()
        {
            Assert.That(_delayTask.IsFaulted, Is.False);
        }

        [Test]
        public void WhenTimeProgresses_ButNotBeyondTheDelayPeriod_MustNotNotComplete()
        {
            TimeService.MoveForwardTo(999);
            Assert.That(_delayTask.IsCompleted, Is.False);
        }

        [Test]
        public void WhenTimeProgresses_ToTheDelayCompletionTime_MustBeCompleted()
        {
            TimeService.MoveForwardTo(1000);
            Assert.That(_delayTask.IsCompleted, Is.True);
        }
    }
}
