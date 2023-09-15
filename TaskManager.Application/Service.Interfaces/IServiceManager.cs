namespace TaskManager.Application.Service.Interfaces
{
    public interface IServiceManager
    {
        ITaskService TaskService { get; }
        IProjectService ProjectService { get; }
        IUserService UserService { get; }
        INotificationService NotificationService { get; }
    }
}
