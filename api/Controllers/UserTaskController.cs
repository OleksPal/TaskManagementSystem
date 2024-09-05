using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Mappers;
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
        public async Task<IActionResult> GetAllTasks()
        {
            var taskList = await _userTaskService.GetAllTasks();

            if (taskList is null)
                return NotFound();

            var taskDtoList = taskList.Select(task => task.ToUserTaskDTO());
            return Ok(taskDtoList);                
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask([FromRoute] Guid id)
        {
            var task = await _userTaskService.GetTaskById(id);

            if (task is null)
                return NotFound();

            var taskDto = task.ToUserTaskDTO();
            return Ok(task);                
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateUserTaskRequestDto taskDto)
        {
            var userTask = taskDto.ToUserTask();

            await _userTaskService.AddTask(userTask);

            return CreatedAtAction(nameof(GetTask), new { id = userTask.Id }, userTask.ToUserTaskDTO());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateUserTaskRequestDto taskDto)
        {
            var userTask = taskDto.ToUserTask(id);
            await _userTaskService.EditTask(userTask);

            return Ok(userTask.ToUserTaskDTO());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            await _userTaskService.DeleteTask(id);

            return NoContent();
        }
    }
}
