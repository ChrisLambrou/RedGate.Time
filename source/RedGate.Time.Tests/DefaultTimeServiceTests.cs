using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RedGate.Time.Impl;

namespace RedGate.Time.Tests
{
    [TestFixture]
    public class DefaultTimeServiceTests
    {
        /// <summary>
        /// The duration of the real delay tasks used in the tests, in milliseconds.
        /// </summary>
        private static readonly int DelayTimeSpanInMilliseconds = 800;

        /// <summary>
        /// The duration of the real delay tasks used in the tests, equivalent to <see cref="DelayTimeSpanInMilliseconds"/>.
        /// </summary>
        private static readonly TimeSpan DelayTimeSpan = TimeSpan.FromMilliseconds(DelayTimeSpanInMilliseconds);

        /// <summary>
        /// The leeway given to the OS to reschedule the current thread once a delay task expires or is cancelled.
        /// </summary>
        private static readonly TimeSpan Leeway = TimeSpan.FromMilliseconds(80);

        /// <summary>
        /// The time span after which a cancellation occurs whilst waiting for a delay task.
        /// Equivalent to half of <see cref="DelayTimeSpan"/>.
        /// </summary>
        private static readonly TimeSpan CancelTimeSpan = TimeSpan.FromMilliseconds(DelayTimeSpanInMilliseconds >> 1);

        [Test]
        public void UtcNow_ShouldReflectTheCurrentTime()
        {
            var service = new DefaultTimeService();

            var before = DateTime.UtcNow;
            var now = service.UtcNow;
            var after = DateTime.UtcNow;

            Assert.That(now, Is.GreaterThanOrEqualTo(before));
            Assert.That(now, Is.LessThanOrEqualTo(after));
        }

        [Test]
        public void UtcNow_ShouldReturnADateTimeWithUtcKind()
        {
            var service = new DefaultTimeService();
            var now = service.UtcNow;
            Assert.That(now.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void Now_ShouldReflectTheCurrentTime()
        {
            var service = new DefaultTimeService();

            var before = DateTime.Now;
            var now = service.Now;
            var after = DateTime.Now;

            Assert.That(now, Is.GreaterThanOrEqualTo(before));
            Assert.That(now, Is.LessThanOrEqualTo(after));
        }

        [Test]
        public void Now_ShouldReturnADateTimeWithLocalKind()
        {
            var service = new DefaultTimeService();
            var now = service.Now;
            Assert.That(now.Kind, Is.EqualTo(DateTimeKind.Local));
        }

        private IEnumerable<TestCaseData> TestCases()
        {
            Func<ITimeService, Task> func;

            func = service => service.Delay(DelayTimeSpan);
            yield return new TestCaseData(func).SetName("Delay as TimeSpan");

            func = service => service.Delay(DelayTimeSpanInMilliseconds);
            yield return new TestCaseData(func).SetName("Delay in milliseconds");

            func = service => service.Delay(DelayTimeSpan, new CancellationToken());
            yield return new TestCaseData(func).SetName("Delay as TimeSpan, with cancellation token");

            func = service => service.Delay(DelayTimeSpanInMilliseconds, new CancellationToken());
            yield return new TestCaseData(func).SetName("Delay in milliseconds, with cancellation token");
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void Delay_ShouldReturnATaskThatEffectsTheRequestedDelay(Func<ITimeService, Task> createTask)
        {
            var service = new DefaultTimeService();

            var start = DateTime.UtcNow;
            var task = createTask(service);
            var end = DateTime.UtcNow;

            task.Wait();
            var now = DateTime.UtcNow;

            Assert.That(now, Is.GreaterThanOrEqualTo(start + DelayTimeSpan - Leeway));
            Assert.That(now, Is.LessThanOrEqualTo(end + DelayTimeSpan + Leeway));
        }

        private IEnumerable<TestCaseData> CancellationTestCases()
        {
            Func<ITimeService, CancellationToken, Task> func;

            func = (service, token) => service.Delay(DelayTimeSpan, token);
            yield return new TestCaseData(func).SetName("Delay as TimeSpan, with cancellation token");

            func = (service, token) => service.Delay(DelayTimeSpanInMilliseconds, token);
            yield return new TestCaseData(func).SetName("Delay in milliseconds, with cancellation token");
        }

        [Test]
        [TestCaseSource("CancellationTestCases")]
        public void Delay_WhenCreatedWithACancellationToken_ShouldBeCancellable(
            Func<ITimeService, CancellationToken, Task> createTask)
        {
            var service = new DefaultTimeService();

            var tokenSource = new CancellationTokenSource();
            Task.Delay(CancelTimeSpan).ContinueWith(_ => tokenSource.Cancel());

            var start = DateTime.UtcNow;
            var task = createTask(service, tokenSource.Token);
            var end = DateTime.UtcNow;

            var aggregateException = Assert.Throws<AggregateException>(task.Wait);
            var now = DateTime.UtcNow;

            Assert.That(aggregateException.InnerExceptions.First(), Is.InstanceOf<TaskCanceledException>());
            Assert.That(now, Is.GreaterThanOrEqualTo(start + CancelTimeSpan - Leeway));
            Assert.That(now, Is.LessThanOrEqualTo(end + CancelTimeSpan + Leeway));
        }
    }
}