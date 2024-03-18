using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Finstro.Serverless.API.Controllers.AdminPortal
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        /// <summary>
        ///    Returns all tasks for a user.
        /// </summary>
        /// <param name="request">Object carrying the UserID</param>
        /// <returns></returns>
        [Route("TasksForUser")]
        [HttpPost]
        public ActionResult<TasksForUserResponse> TasksForUser(UserRequest request)
        {
            return Ok(new TasksForUserResponse()
            {
                Amount = 10m,
                Client = new BasicClient() { Balance = 100m, ClientID = 66, ClientName = "Ricardo", Limit = 5000m, Status = "Pending" },
                ClientStatus = "Approved",
                CreatedDate = DateTime.UtcNow,
                TaskStatus = "Done",
                Type = "Call Back"
            });
        }

        /// <summary>
        ///    Returns all tasks for a user.
        /// </summary>
        /// <param name="request">Cretes a new task by a user</param>
        /// <returns></returns>
        [Route("CreateTaskForUser")]
        [HttpPost]
        public async Task<ActionResult> CreateTaskForUser(CreateTaskForUserRequest request)
        {
            return Ok();
        }

        /// <summary>
        ///    Get task-settings for a user.
        /// </summary>
        /// <param name="userId">ID representation of user</param>
        /// <returns></returns>
        [Route("TaskSettingsForUser/{userId}")]
        [HttpGet]
        public ActionResult<TaskSettingsForUser> TaskSettingsForUser(int userId)
        {
            return Ok(new TasksForUserResponse());
        }

        /// <summary>
        ///    Updates task-settings for a user.
        /// </summary>
        /// <param name="request">Task-settings object to update</param>
        /// <returns></returns>
        [Route("TaskSettingsForUser/{userId}")]
        [HttpPut]
        public async Task<ActionResult> TaskSettingsForUser(TaskSettingsForUser request)
        {
            return Ok();
        }
    }
}