using System.Net;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Helpers;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        this._taskService = taskService;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateTaskControllerAsync([FromBody] TaskRequestBody request)
    {
        string taskName = NameNormalizer.Normalize(request.Name);
        var result = await this._taskService.CreateTaskServiceAsync(taskName);
        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? (int)HttpStatusCode.BadRequest, new ErrorResponse
            {
                Details = result.Error ?? "an error occured."
            });
        }
        return StatusCode((int)HttpStatusCode.Created, new SuccessResponse<Models.Task>
        {
            Data = result.Data
        });
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllTasksControllerAsync()
    {
        var result = await this._taskService.GetAllTaskServiceAsync();
        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? (int)HttpStatusCode.BadRequest, new ErrorResponse
            {
                Details = result.Error ?? "an error occured."
            });
        }
        return StatusCode((int)HttpStatusCode.OK, new SuccessResponse<Models.Task[]>
        {
            Data = result.Data
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleTaskControllerAsync(Guid id)
    {
        var result = await this._taskService.GetSingleTaskServiceAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? (int)HttpStatusCode.BadRequest, new ErrorResponse
            {
                Details = result.Error ?? "an error occured."
            });
        }
        return StatusCode((int)HttpStatusCode.OK, new SuccessResponse<Models.Task>
        {
            Data = result.Data
        });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> MarkTaskAsCompletedControllerAsync(Guid id)
    {
        var result = await this._taskService.MarkTaskAsCompleteServiceAsync(id);
        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? (int)HttpStatusCode.BadRequest, new ErrorResponse
            {
                Details = result.Error ?? "an error occured."
            });
        }
        return StatusCode((int)HttpStatusCode.OK, new SuccessResponse<Models.Task>
        {
            Data = result.Data
        });
    }

}