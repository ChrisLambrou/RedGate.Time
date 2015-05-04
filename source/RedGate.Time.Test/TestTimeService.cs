using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedGate.Time.Test
{
    /// <summary>
    ///     Implementation of <see cref="ITimeService" /> that can be used by unit tests to manually and
    ///     instantaneously progress &quot;current time&quot; forwards during tests.
    /// </summary>
    public sealed class TestTimeService : ITimeService, IDisposable
    {
        private static readonly TimeSpan DefaultYieldDuration = TimeSpan.FromMilliseconds(10);

        /// <summary>
        ///     Synchronisation lock object used to guard access to <see cref="_pendingTasks" /> and
        ///     <see cref="_currentTime" />.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        ///     All ongoing delay tasks, keyed by expiration time.
        /// </summary>
        private readonly IDictionary<DateTime, IList<TaskCompletionSource<object>>> _pendingTasks =
            new SortedDictionary<DateTime, IList<TaskCompletionSource<object>>>();

        /// <summary>
        ///     When manually progressing the current time via calls to
        ///     <c>MoveForwardBy</c> or <c>MoveForwardTo</c>, this is the duration that the calling thread
        ///     yields in order to allow any continuation tasks to execute after each completed delay task.
        /// </summary>
        private readonly TimeSpan _yieldDuration;

        /// <summary>
        ///     The initial start time for the service.
        /// </summary>
        private readonly DateTime _startTime;

        /// <summary>
        ///     The current time for the service.
        /// </summary>
        private DateTime _currentTime;

        /// <summary>
        ///     A count of how often <see cref="Dispose" /> has been invoked. Used to ensure that clean-up only occurs once.
        /// </summary>
        private long _disposalCount;

        /// <summary>
        ///     Creates a new instance that uses the actual current time as the initial start time, and a default
        ///     yield duration to allow continuation tasks to execute when delay tasks complete.
        /// </summary>
        public TestTimeService() : this(DateTime.UtcNow, DefaultYieldDuration)
        {}

        /// <summary>
        ///     Creates a new instance that uses the specified <paramref name="startTime" />, and a default
        ///     yield duration to allow continuation tasks to execute when delay tasks complete.
        /// </summary>
        /// <param name="startTime">The initial value of &quot;current time&quot;.</param>
        public TestTimeService(DateTime startTime) : this(startTime, DefaultYieldDuration)
        {}

        /// <summary>
        ///     Creates a new instance that uses the actual current time as the initial start time, and
        ///     the specified yield duration to allow continuation tasks to execute when delay tasks complete.
        /// </summary>
        /// <param name="yieldDuration">
        ///     When manually progressing the current time via calls to
        ///     <c>MoveForwardBy</c> or <c>MoveForwardTo</c>, this is the duration that the calling thread
        ///     yields in order to allow any continuation tasks to execute after each completed delay task.
        /// </param>
        public TestTimeService(TimeSpan yieldDuration) : this(DateTime.UtcNow, yieldDuration)
        {}

        /// <summary>
        ///     Creates a new instance that uses the specified <paramref name="startTime" />, and a default
        ///     yield duration to allow continuation tasks to execute when delay tasks complete.
        /// </summary>
        /// <param name="startTime">The initial value of &quot;current time&quot;.</param>
        /// <param name="yieldDuration">
        ///     When manually progressing the current time via calls to
        ///     <c>MoveForwardBy</c> or <c>MoveForwardTo</c>, this is the duration that the calling thread
        ///     yields in order to allow any continuation tasks to execute after each completed delay task.
        /// </param>
        public TestTimeService(DateTime startTime, TimeSpan yieldDuration)
        {
            if (yieldDuration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("yieldDuration", "Value cannot be negative or infinite");
            }

            _startTime = _currentTime = startTime.ToUniversalTime();
            _yieldDuration = yieldDuration;
        }

        /// <summary>
        ///     Dispses of this instamce, attempting to cancel all outstanding delay tasks.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposalCount) == 1)
            {
                lock (_lock)
                {
                    foreach (var taskCompletionSource in _pendingTasks.SelectMany(pair => pair.Value))
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    _pendingTasks.Clear();
                }
            }
        }

        /// <summary>
        ///     Checks that this instance hasn't been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Raised if this instance has beeen disposed.</exception>
        private void CheckDisposed()
        {
            if (Interlocked.Read(ref _disposalCount) > 0)
            {
                throw new ObjectDisposedException("TestTimeService has been disposed");
            }
        }

        /// <summary>
        ///     Gets the system's current date and time, expressed in local time.
        /// </summary>
        public DateTime Now
        {
            get { return UtcNow.ToLocalTime(); }
        }

        /// <summary>
        ///     Gets the system's current date and time, expressed in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime UtcNow
        {
            get
            {
                CheckDisposed();

                lock (_lock)
                {
                    return _currentTime;
                }
            }
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
            CheckDisposed();

            // This mirrors the implementation of Task.Delay(TimeSpan).
            return Delay(delay, default(CancellationToken));
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
            CheckDisposed();

            // This mirrors the implementation of Task.Delay(TimeSpan, CancellationToken).
            var totalMilliseconds = (long) delay.TotalMilliseconds;
            if (totalMilliseconds < -1 || totalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("delay");
            }
            return Delay((int) totalMilliseconds, cancellationToken);
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
            CheckDisposed();

            // This mirrors the implementation of Task.Delay(int).
            return Delay(millisecondsDelay, default(CancellationToken));
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
            CheckDisposed();

            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }
            if (millisecondsDelay == 0)
            {
                return Task.FromResult<object>(null);
            }

            var taskCompletionSource = CeateTaskCompletionSource(cancellationToken);
            lock (_lock)
            {
                var dueTime = millisecondsDelay == -1
                                  ? DateTime.MaxValue
                                  : _currentTime + TimeSpan.FromMilliseconds(millisecondsDelay);

                StoreTaskCompletionSource(dueTime, taskCompletionSource);
            }

            return taskCompletionSource.Task;
        }

        private static TaskCompletionSource<object> CeateTaskCompletionSource(CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.SetCanceled();
            }
            else if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            }
            return taskCompletionSource;
        }

        private void StoreTaskCompletionSource(DateTime dueTime, TaskCompletionSource<object> completionSource)
        {
            Debug.Assert(Monitor.IsEntered(_lock));

            IList<TaskCompletionSource<object>> list;
            if (!_pendingTasks.TryGetValue(dueTime, out list))
            {
                list = new List<TaskCompletionSource<object>>();
                _pendingTasks.Add(dueTime, list);
            }
            list.Add(completionSource);
        }

        /// <summary>
        ///     Increases the &quot;current time&quot; by the specified amount.
        /// </summary>
        /// <param name="millisecondsDelta">The amount of time in milliseconds to move &quot;current time&quot; forward by.</param>
        public void MoveForwardBy(int millisecondsDelta)
        {
            CheckDisposed();

            if (millisecondsDelta < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelta", "Negative time span not permitted");
            }
            lock (_lock)
            {
                MoveForwardToImpl(_currentTime + TimeSpan.FromMilliseconds(millisecondsDelta));
            }
        }

        /// <summary>
        ///     Increases the &quot;current time&quot; by the specified amount.
        /// </summary>
        /// <param name="delta">The amount of time to move &quot;current time&quot; forward by.</param>
        public void MoveForwardBy(TimeSpan delta)
        {
            CheckDisposed();

            if (delta < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("delta", "Negative time span not permitted");
            }
            lock (_lock)
            {
                MoveForwardToImpl(_currentTime + delta);
            }
        }

        /// <summary>
        ///     Increases the &quot;current time&quot; to the specified point after the initial start time.
        /// </summary>
        /// <param name="millisecondsOffsetFromStartTime">The number of milliseconds elapsed since the start time.</param>
        public void MoveForwardTo(int millisecondsOffsetFromStartTime)
        {
            CheckDisposed();

            if (millisecondsOffsetFromStartTime < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "millisecondsOffsetFromStartTime",
                    "Negative time span not permitted");
            }
            lock (_lock)
            {
                var newTime = _startTime + TimeSpan.FromMilliseconds(millisecondsOffsetFromStartTime);
                if (newTime < _currentTime)
                {
                    throw new ArgumentOutOfRangeException(
                        "millisecondsOffsetFromStartTime",
                        "Specified time is in the past");
                }
                MoveForwardToImpl(newTime);
            }
        }

        /// <summary>
        ///     Increases the &quot;current time&quot; to the specified point after the initial start time.
        /// </summary>
        /// <param name="offsetFromStartTime">The offset after the start time to move forward to.</param>
        public void MoveForwardTo(TimeSpan offsetFromStartTime)
        {
            CheckDisposed();

            if (offsetFromStartTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("offsetFromStartTime", "Negative time span not permitted");
            }
            lock (_lock)
            {
                var newTime = _startTime + offsetFromStartTime;
                if (newTime < _currentTime)
                {
                    throw new ArgumentOutOfRangeException("offsetFromStartTime", "Specified time is in the past");
                }
                MoveForwardToImpl(newTime);
            }
        }

        /// <summary>
        ///     Increases the &quot;current time&quot; to the specified point in time.
        /// </summary>
        /// <param name="newTime">The the new time to move forward to.</param>
        public void MoveForwardTo(DateTime newTime)
        {
            CheckDisposed();

            lock (_lock)
            {
                newTime = newTime.ToUniversalTime();
                if (newTime < _currentTime)
                {
                    throw new ArgumentOutOfRangeException("newTime", "Specified time is in the past");
                }
                MoveForwardToImpl(newTime);
            }
        }

        private void MoveForwardToImpl(DateTime newTime)
        {
            Debug.Assert(newTime.Kind == DateTimeKind.Utc);
            Debug.Assert(Monitor.IsEntered(_lock));

            while (true)
            {
                // Fetch the next set of due tasks.
                var pair = _pendingTasks.FirstOrDefault();

                // If there are no due tasks, or the next set aren't due yet, we're done.
                if (pair.Value == null || pair.Key > newTime)
                {
                    _currentTime = newTime;
                    return;
                }

                // Update the current time and deal with the current set of due tasks.
                _currentTime = pair.Key;
                foreach (var taskCompletionSource in pair.Value)
                {
                    taskCompletionSource.TrySetResult(null);
                }
                _pendingTasks.Remove(pair.Key);

                // Briefly yield to any tasks that have just completed.
                Monitor.Wait(_lock, _yieldDuration);
            }
        }
    }
}
