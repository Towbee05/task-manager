using System.Net;
using TaskManager.Models;
using TaskManager.Repository;

namespace TaskManager.Services;

public interface ITaskService
{
    Task<ServiceResult<Models.Task>> CreateTaskServiceAsync(string taskName);
    Task<ServiceResult<Models.Task[]>> GetAllTaskServiceAsync();
    Task<ServiceResult<Models.Task>> GetSingleTaskServiceAsync(Guid taskId);
    Task<ServiceResult<Models.Task>> MarkTaskAsCompleteServiceAsync(Guid taskId);
}

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger _logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        this._repository = taskRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Models.Task>> CreateTaskServiceAsync(string taskName)
    {
        try
        {
            Models.Task task = await this._repository.CreateTaskAsync(taskName);
            this._logger.LogInformation("create task service: task with id: {Id} successfully added", task.Id);
            return new ServiceResult<Models.Task>
            {
                Success = true,
                Data = task
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "create task service: task creation failed");
            return new ServiceResult<Models.Task>
            {
                Success = false,
                Error = "internal server error",
                ErrorCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ServiceResult<Models.Task[]>> GetAllTaskServiceAsync()
    {
        try
        {
            var allTasks = await this._repository.GetAllTasksAsync();
            Models.Task[] allTasksArray = allTasks.ToArray();

            this._logger.LogInformation("get all tasks service: successfully fetched all tasks");
            return new ServiceResult<Models.Task[]>
            {
                Success = true,
                Data = allTasksArray
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "get all tasks service: task fetch failed");
            return new ServiceResult<Models.Task[]>
            {
                Success = false,
                Error = "internal server error",
                ErrorCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ServiceResult<Models.Task>> GetSingleTaskServiceAsync(Guid taskId)
    {
        try
        {
            Models.Task? task = await this._repository.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                this._logger.LogWarning("get single task service: unable to find task with id: {Id}", taskId);
                return new ServiceResult<Models.Task>
                {
                    Success = false,
                    Error = "not found",
                    ErrorCode = (int)HttpStatusCode.NotFound
                };

            }
            this._logger.LogInformation("get single task service: successfully fetched task with id: {Id}", taskId);
            return new ServiceResult<Models.Task>
            {
                Success = true,
                Data = task
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "get single task service: task fetch failed");
            return new ServiceResult<Models.Task>
            {
                Success = false,
                Error = "internal server error",
                ErrorCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ServiceResult<Models.Task>> MarkTaskAsCompleteServiceAsync(Guid taskId)
    {
        try
        {
            DateTime currentTime = DateTime.UtcNow;
            int affectedRows = await this._repository.MarkTaskAsDoneById(taskId, currentTime);
            if (affectedRows < 1)
            {
                this._logger.LogWarning("mark task as completed service: operation affected zero rows");
                return new ServiceResult<Models.Task>
                {
                    Success = false,
                    Error = "failed to mark task as complete",
                    ErrorCode = (int)HttpStatusCode.BadRequest
                };
            }

            Models.Task? task = await this._repository.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                this._logger.LogWarning("mark task as completed service: unable to find task with id: {Id}", taskId);
                return new ServiceResult<Models.Task>
                {
                    Success = false,
                    Error = "not found",
                    ErrorCode = (int)HttpStatusCode.NotFound
                };
            }

            this._logger.LogInformation("mark task as completed service: successfully marked task with id: {Id} as completed", taskId);
            return new ServiceResult<Models.Task>
            {
                Success = true,
                Data = task
            };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "get single task service: task fetch failed");
            return new ServiceResult<Models.Task>
            {
                Success = false,
                Error = "internal server error",
                ErrorCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}