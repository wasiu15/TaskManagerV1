using Domain;
using Domain.Models;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Domain.Dtos;
using TaskManager.Infrastructure.Utilities;

public class BackgroundWorkerService : BackgroundService
{
    //readonly ILogger<BackgroundWorkerService> _logger;
    private readonly IRepositoryManager _repository;
    private readonly IServiceProvider _serviceProvider;
    readonly IConfiguration _configuration;
    private readonly IHttpClientWrapper _httpClient;

    public BackgroundWorkerService(IServiceProvider serviceProvider, IConfiguration configuration, IHttpClientWrapper httpClient, IRepositoryManager repository)
    {
        //_logger = logger;
        _repository = repository;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                //var scopedService = scope.ServiceProvider.GetRequiredService<IHttpClientWrapper>();
                var getAnyUnCompletedTaskToDueInTwoDays = await _repository.TaskRepository.GetAnyUnCompletedTaskToDueInTwoDays(false);
                if (getAnyUnCompletedTaskToDueInTwoDays != null)
                {
                    foreach (var task in getAnyUnCompletedTaskToDueInTwoDays)
                    {
                        var checkIfUserAlreadyRecievedNotification = await _repository.NotificationRepository.GetByNotificationIdAndUserId(task.TaskId, task.UserId, true);
                        if (checkIfUserAlreadyRecievedNotification == null)
                        {
                            //  NOW WE CAN SEND MAIL AND UPDATE DB
                            //  GET CURRENT USER EMAIL AND NAME FROM THE HTTPCONTEXT CLASS
                            var getUserFromDb = await _repository.UserRepository.GetByUserId(task.UserId, false);
                            if (getUserFromDb != null)
                            {
                                var sendRequest = new EmailSenderRequestDto
                                {
                                    email = getUserFromDb.Email,
                                    subject = "Due Date Reminder",
                                    message = "Hello " + getUserFromDb.Name + ", \n Your task with the ID: " + task.TaskId + " now has less than 48 hours left to be complete as at Time: " + DateTime.UtcNow.ToString()
                                };

                                //  GET THE MAILER URL... WHERE WE WOULD BE SENDING OUR POST REQUEST TO
                                var mailerUrl = $"{_configuration.GetSection("ExternalAPIs")["MailerUrl"]}";

                                //  THIS LINE SENDS THE REQUEST TO THE EMAIL SERVER
                                var sendEmailResponse = _httpClient.SendPostEmailAsync<string>(mailerUrl, sendRequest);

                                //  IT WILL ONLY UPDATE THIS RECORD IS THE EMAIL WAS SENT SUCCESSFULLY ELSE THE TASK WILL BE RETURNED BACK FOR THE NEXT BATCH PROCESSING
                                if (sendEmailResponse == "1")
                                {
                                    var createNotificationRequest = new Notification
                                    {
                                        NotificationId = Guid.NewGuid().ToString(),
                                        TaskId = task.TaskId.ToString(),
                                        RecievedUserId = task.UserId,
                                        Type = NotificationType.Due_date.ToString(),
                                        Message = sendRequest.message,
                                        ReadStatus = NotificationStatus.Unread.ToString(),
                                        Time = DateTime.UtcNow,
                                    };
                                    _repository.NotificationRepository.CreateNotification(createNotificationRequest);
                                    await _repository.SaveAsync();
                                }
                            }
                        }
                    }
                }

                await Task.Delay(6000, stoppingToken);
            }
        }
    }
}