using System;
using System.Linq;
using System.Threading;
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

        [Test]
        public void WhenTimeProgresses_ToTheDelayCompletionTime_AnyContinuationTaskMustExecute()
        {
            int executionCount = 0;
            _delayTask.ContinueWith(task => executionCount++);

            Assert.That(executionCount, Is.EqualTo(0));
            TimeService.MoveForwardTo(1000);
            Assert.That(executionCount, Is.EqualTo(1));
        }

        [Test]
        public void WhenDelayIsCalledWithACancelledCancellationToken_ItShouldReturnAnImmediatelyCancelledTask()
        {
            var token = new CancellationToken(true);
            var task = TimeService.Delay(1000, token);

            var aggregateException = Assert.Throws<AggregateException>(task.Wait);
            Assert.That(aggregateException.InnerExceptions.First(), Is.InstanceOf<TaskCanceledException>());
        }

        [Test]
        public void WhenDelayIsCalledWithACancellationToken_ThatIsSubsequentlyCancelled_TheTaskShouldRaiseAnException()
        {
            var source = new CancellationTokenSource();
            var delayTask = TimeService.Delay(1000, source.Token);
            TimeService.MoveForwardBy(500);

            source.Cancel();

            var aggregateException = Assert.Throws<AggregateException>(() => delayTask.Wait(10));
            Assert.That(aggregateException.InnerExceptions.First(), Is.InstanceOf<TaskCanceledException>());
        }
    }
}
