﻿using System;
using System.Threading.Tasks;

namespace Log4Net.Async
{
    public static class LoggingTaskExtension
    {
        /// <summary>
        ///     This task shouldn't be waited on (as it's not guaranteed to run), and you shouldn't wait on the parent task either (because it might throw an 
        ///     exception that doesn't get handled). If you want to be waiting on something, use LogErrorsWaitable instead.
        /// 
        ///     None of these methods are suitable for tasks that return a value. If you're wanting a result, you should probably be handling
        ///     errors yourself.
        /// </summary>
        public static Task LogErrors(this Task task, Action<string, Exception> logMethod)
        {
            return task.ContinueWith(t => LogErrorsInner(t, logMethod), TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        ///     This task can be waited on (as it's guaranteed to run), and you should wait on this rather than the parent task. Because it's
        ///     guaranteed to run, it may be slower than using LogErrors, and you should consider using that method if you don't want to wait.
        /// 
        ///     None of these methods are suitable for tasks that return a value. If you're wanting a result, you should probably be handling
        ///     errors yourself.
        /// </summary>
        public static Task LogErrorsWaitable(this Task task, Action<string, Exception> logMethod)
        {
            return task.ContinueWith(t => LogErrorsInner(t, logMethod));
        }

        private static void LogErrorsInner(Task task, Action<string, Exception> logAction)
        {
            if (task.Exception != null)
            {
                logAction("Aggregate Exception with " + task.Exception.InnerExceptions.Count + " inner exceptions: ", task.Exception);
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    logAction("Inner exception from aggregate exception: ", innerException);
                }
            }
        }
    }
}
