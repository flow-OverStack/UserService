namespace UserService.BackgroundTasks.Interfaces;

public interface IJob
{
    Task Run();
}