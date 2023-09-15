namespace TaskManager.Application.Repository.Interfaces
{
    public interface IRepositoryManager
    {
        ITaskRepository TaskRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IUserRepository UserRepository { get; }
        INotificationRepository NotificationRepository { get; }
        Task SaveAsync();
    }
}
