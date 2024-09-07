using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;

        public UserTaskController(IUserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] QueryObject query)
        {
            var taskDtoList = await _userTaskService.GetAllTasksAsync(query);

            if (taskDtoList is null)
                return NotFound();

            return Ok(taskDtoList);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTask([FromRoute] Guid id)
        {
            var taskDto = await _userTaskService.GetTaskByIdAsync(id);

            if (taskDto is null)
                return NotFound();

            return Ok(taskDto);                
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateUserTaskRequestDto createTaskDto)
        {
            var taskDto = await _userTaskService.AddTaskAsync(createTaskDto);

            return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateUserTaskRequestDto updateTaskDto)
        {
            var taskDto = await _userTaskService.EditTaskAsync(id, updateTaskDto);

            if (taskDto is null)
                return NotFound();

            return Ok(taskDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            var task = await _userTaskService.DeleteTaskAsync(id);

            if (task is null)
                return NotFound();

            return NoContent();
        }
    }
}
