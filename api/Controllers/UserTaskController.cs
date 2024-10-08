﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.DTOs.User;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserTaskController(IUserTaskService userTaskService, UserManager<User> userManager, ILogger<UserController> logger)
        {
            _userTaskService = userTaskService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasksAsync([FromQuery] QueryObject query)
        {
            var user = await GetUser();

            if (user is null)
                return StatusCode(500, "No such user");

            var taskDtoList = await _userTaskService.GetAllTasksAsync(user.Id, query);

            if (taskDtoList is null)
                return NotFound();

            return Ok(taskDtoList);
        }

        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetTaskAsync([FromRoute] Guid taskId)
        {
            var user = await GetUser();

            if (user is null)
                return StatusCode(500, "No such user");

            var taskDto = await _userTaskService.GetTaskByIdAsync(taskId, user.Id);            

            if (taskDto is null)
                return NotFound();

            return Ok(taskDto);                
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] CreateUserTaskRequestDto createTaskDto)
        {
            if (!TryValidateModel(createTaskDto))
                return BadRequest(ModelState);

            var taskDto = await _userTaskService.AddTaskAsync(createTaskDto);

            _logger.LogInformation($"Task with id {taskDto.Id} has been created");

            return CreatedAtAction(nameof(GetTaskAsync), new { taskId = taskDto.Id }, taskDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTaskAsync([FromRoute] Guid id, [FromBody] UpdateUserTaskRequestDto updateTaskDto)
        {
            if (!TryValidateModel(updateTaskDto))
                return BadRequest(ModelState);

            var user = await GetUser();

            if (user is null)
                return StatusCode(500, "No such user");

            var taskDto = await _userTaskService.EditTaskAsync(id, updateTaskDto, user.Id);

            if (taskDto is null)
                return NotFound();

            _logger.LogWarning($"Task with id {taskDto.Id} has been updated");

            return Ok(taskDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid id)
        {
            var user = await GetUser();

            if (user is null)
                return StatusCode(500, "No such user");

            var task = await _userTaskService.DeleteTaskAsync(id, user.Id);

            if (task is null)
                return NotFound();

            _logger.LogWarning($"Task with id {task.Id} has been deleted");

            return NoContent();
        }

        protected virtual async Task<User> GetUser()
        {
            var userName = User.GetUserName();
            return await _userManager.FindByNameAsync(userName);
        }
    }
}
