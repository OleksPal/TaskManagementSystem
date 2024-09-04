using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> GetAllTasks()
        {
            var taskList = await _userTaskService.GetAllTasks();

            return Ok(taskList);
        }
    }
}
