using System;

namespace Swift.Update
{
    /// <summary>
    /// Base class for actions that can be executed by the updater.
    /// </summary>
    public abstract class UpdateAction
    {
        /// <summary>
        /// Executes this action, reporting progress using the provided callback.
        /// </summary>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns>The result.</returns>
        public abstract UpdateResult Execute(ProgressCallback progressCallback);
    }

    /// <summary>
    /// A callback for providing progress messages.
    /// </summary>
    /// <param name="progress">The progress percentage (0-100 for percentages or -1 for indeterminate. Values will be clamped).</param>
    /// <param name="detailMessage">The detail message.</param>
    public delegate void ProgressCallback(int progress, string detailMessage);

    /// <summary>
    /// Represents the result of an UpdateAction.
    /// </summary>
    public class UpdateResult
    {
        /// <summary>
        /// Gets a value indicating whether the UpdateAction executed successfully.
        /// </summary>
        public bool IsSuccessful { get; private set; }
        /// <summary>
        /// Gets the exception that caused the action to fail.
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        /// Gets a value indicating whether updating can continue despite the exception.
        /// </summary>
        public bool CanContinueWithException { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateResult"/> class.
        /// </summary>
        /// <param name="isSuccessful"><c>true</c> if the action completed successfully.</param>
        /// <param name="exception">The exception that caused the action to fail, if any.</param>
        /// <param name="canContinueWithException">if set to <c>true</c>, updating will continue even with an exception thrown.</param>
        public UpdateResult(bool isSuccessful, Exception exception = null, bool canContinueWithException = true)
        {
            IsSuccessful = isSuccessful;
            Exception = exception;
            CanContinueWithException = canContinueWithException;
        }
    }
}
