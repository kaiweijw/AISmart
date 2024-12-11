using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

public class MockLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<string> Logs { get; } = new ConcurrentQueue<string>();

    public ILogger CreateLogger(string categoryName)
    {
        return new MockLogger(Logs);
    }

    public void Dispose()
    {
    }

    private class MockLogger : ILogger
    {
        private readonly ConcurrentQueue<string> _logs;

        public MockLogger(ConcurrentQueue<string> logs)
        {
            _logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logs.Enqueue(formatter(state, exception));
        }
    }
}