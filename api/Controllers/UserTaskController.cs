using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Mappers;
using TaskManagementSystem.Models.DTOs;
using TaskManagementSystem.Services.Interfaces;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
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

            if (taskList is not null)
                return Ok(taskList);
            else
                return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask([FromRoute] Guid id)
        {
            var task = await _userTaskService.GetTaskById(id);

            if (task is not null) 
                return Ok(task);
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] UserTaskDTO taskDTO)
        {
            var userTask = taskDTO.ToUserTask();

            await _userTaskService.AddTask(userTask);

            return CreatedAtAction(nameof(GetTask), new { id = userTask.Id }, userTask.ToUserTaskDTO());
        }
    }
}
