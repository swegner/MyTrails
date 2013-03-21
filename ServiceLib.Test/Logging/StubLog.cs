namespace MyTrails.ServiceLib.Test.Logging
{
    using System;
    using log4net;
    using log4net.Core;

    /// <summary>
    /// Stub <see cref="ILog"/> implementation to use for testing.
    /// </summary>
    public class StubLog : ILog
    {
        /// <summary>
        /// Returns null, always.
        /// </summary>
        public ILogger Logger
        {
            get { return null; }
        }

        /// <summary>
        /// Returns false, always.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false, always.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false, always.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false, always.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false, always.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        public void Debug(object message)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        /// <param name="exception">The parameter is not used.</param>
        public void Debug(object message, Exception exception)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void DebugFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        public void DebugFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        public void DebugFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        /// <param name="arg2">The parameter is not used.</param>
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="provider">The parameter is not used.</param>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        public void Info(object message)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        /// <param name="exception">The parameter is not used.</param>
        public void Info(object message, Exception exception)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void InfoFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        public void InfoFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        public void InfoFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        /// <param name="arg2">The parameter is not used.</param>
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="provider">The parameter is not used.</param>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        public void Warn(object message)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        /// <param name="exception">The parameter is not used.</param>
        public void Warn(object message, Exception exception)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void WarnFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        public void WarnFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        public void WarnFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        /// <param name="arg2">The parameter is not used.</param>
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="provider">The parameter is not used.</param>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        public void Error(object message)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        /// <param name="exception">The parameter is not used.</param>
        public void Error(object message, Exception exception)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void ErrorFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        public void ErrorFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        public void ErrorFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        /// <param name="arg2">The parameter is not used.</param>
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="provider">The parameter is not used.</param>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        public void Fatal(object message)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="message">The parameter is not used.</param>
        /// <param name="exception">The parameter is not used.</param>
        public void Fatal(object message, Exception exception)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void FatalFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        public void FatalFormat(string format, object arg0)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        public void FatalFormat(string format, object arg0, object arg1)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="arg0">The parameter is not used.</param>
        /// <param name="arg1">The parameter is not used.</param>
        /// <param name="arg2">The parameter is not used.</param>
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="provider">The parameter is not used.</param>
        /// <param name="format">The parameter is not used.</param>
        /// <param name="args">The parameter is not used.</param>
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
        }
    }
}
