using FluentAssertions;
using Xunit;

namespace Passara.Desktop.Tests;

/// <summary>
/// Base class for all unit tests in the Passara.Desktop.Tests project.
/// Provides common setup and utility methods.
/// </summary>
public abstract class UnitTestBase : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitTestBase"/> class.
    /// </summary>
    protected UnitTestBase()
    {
        // Common setup for all tests
        Setup();
    }

    /// <summary>
    /// Performs common test setup.
    /// Override in derived classes for specific setup.
    /// </summary>
    protected virtual void Setup()
    {
    }

    /// <summary>
    /// Performs test cleanup.
    /// Override in derived classes for specific cleanup.
    /// </summary>
    protected virtual void Cleanup()
    {
    }

    /// <summary>
    /// Disposes resources used by the test.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the test and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Cleanup();
            }

            _disposed = true;
        }
    }
}

/// <summary>
/// Attribute to mark tests that are currently being worked on.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class WorkInProgressAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark integration tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class IntegrationTestAttribute : Attribute
{
}

/// <summary>
/// Attribute to mark security tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class SecurityTestAttribute : Attribute
{
}
