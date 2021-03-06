using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Merp.Web.Site.Areas.OnTime.WorkerServices;
using Merp.Web.Site.Areas.OnTime.Model.Task;
using Merp.TimeTracking.TaskManagement.Web.Areas.OnTime.Model.Task;

namespace Merp.Web.Site.Areas.OnTime.Controllers
{
    [Area("OnTime")]
    [Authorize(Roles ="TaskManagement")]
    public class TaskController : Controller
    {
        public TaskControllerWorkerServices WorkerServices { get; private set; }

        public TaskController(TaskControllerWorkerServices workerServices)
        {
            WorkerServices = workerServices ?? throw new ArgumentNullException(nameof(workerServices));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IEnumerable<CurrentTaskModel> Backlog()
        {
            return WorkerServices.GetBacklogModel();
        }

        [HttpGet]
        public IEnumerable<CurrentTaskModel> NextSevenDays()
        {
            return WorkerServices.GetNextSevenDaysModel();
        }

        [HttpGet]
        public IEnumerable<CurrentTaskModel> Today()
        {
            return WorkerServices.GetTodayModel();
        }

        [HttpGet]
        public object PriorityOptions()
        {
            return Enum.GetValues(typeof(global::Merp.TimeTracking.TaskManagement.QueryStack.Model.TaskPriority));
        }

        [HttpPost]
        public IActionResult Add([FromForm] AddModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var taskId = WorkerServices.Add(model.Name);
                return Ok(taskId);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Update(Guid id, UpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                WorkerServices.Update(id, model.Name, model.Priority, model.JobOrderId);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Cancel(Guid? id)
        {
            if (!id.HasValue)
                return BadRequest();

            try
            {
                WorkerServices.Cancel(id.Value);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult MarkAsComplete(Guid id)
        {
            try
            {
                WorkerServices.MarkAsComplete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}