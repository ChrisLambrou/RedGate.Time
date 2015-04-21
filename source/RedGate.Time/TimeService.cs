using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedGate.Time
{
    /// <summary>
    ///     Default implementation of <see cref="ITimeService" />.
    /// </summary>
    public sealed class TimeService : ITimeService
    {
        /// <summary>
        ///     Gets the system's current date and time, expressed in local time.
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        ///     Gets the system's current date and time, expressed in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        ///     Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The duration of <paramref name="delay" /> in milliseconds is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <remarks>
        ///     After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public Task Delay(TimeSpan delay)
        {
            return Task.Delay(delay);
        }

        /// <summary>
        ///     Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task.</param>
        /// <returns>A Task that represents the time delay.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The duration of <paramref name="delay" /> in milliseconds is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <paramref name="cancellationToken" /> has already been disposed.
        /// </exception>
        /// <remarks>
        ///     If the cancellation token is signaled before the specified time delay, then the Task is completed in
        ///     Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        ///     delay has expired.
        /// </remarks>
        public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            return Task.Delay(delay, cancellationToken);
        }

        /// <summary>
        ///     Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">
        ///     The number of milliseconds to wait before completing the returned Task.
        ///     0 returns a task that completes immediately. -1 returns a task with an infinite delay.
        /// </param>
        /// <returns>A Task that represents the time delay.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> is less than -1.
        /// </exception>
        /// <remarks>
        ///     After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public Task Delay(int millisecondsDelay)
        {
            return Task.Delay(millisecondsDelay);
        }

        /// <summary>
        ///     Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">
        ///     The number of milliseconds to wait before completing the returned Task.
        ///     0 returns a task that completes immediately. -1 returns a task with an infinite delay.
        /// </param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task.</param>
        /// <returns>A Task that represents the time delay.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> is less than -1.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <paramref name="cancellationToken" /> has already been disposed.
        /// </exception>
        /// <remarks>
        ///     If the cancellation token is signaled before the specified time delay, then the Task is completed in
        ///     Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        ///     delay has expired.
        /// </remarks>
        public Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            return Task.Delay(millisecondsDelay, cancellationToken);
        }
    }
}
