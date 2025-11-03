using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;

namespace UserService.Tests.FunctionalTests.Configurations.TestServices;

internal class TestBackgroundJobClient(IServiceProvider provider) : IBackgroundJobClient
{
    public string Create(Job job, IState state)
    {
        object? instance = null;

        if (!job.Method.IsStatic)
        {
            using var scope = provider.CreateAsyncScope();
            instance = scope.ServiceProvider.GetService(job.Type) ??
                       ActivatorUtilities.CreateInstance(provider, job.Type);
        }

        var result = job.Method.Invoke(instance, job.Args.ToArray());

        switch (result)
        {
            case Task task:
                task.GetAwaiter().GetResult();
                break;
            case ValueTask valueTask:
                valueTask.AsTask().GetAwaiter().GetResult();
                break;
        }

        return "test-job-id";
    }

    public bool ChangeState(string jobId, IState state, string expectedState)
    {
        return true;
    }
}