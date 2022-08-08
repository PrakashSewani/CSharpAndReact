using Persistance;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class ActivitesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult>GetActivites(){
            
            return HandleResult(await Mediator.Send(new List.Query()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetActivity(Guid id){
            return HandleResult(await Mediator.Send(new Details.Query{Id=id}));
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity){
            return HandleResult(await Mediator.Send(new Create.Command {Activity=activity}));
        }

        [Authorize(Policy ="isActivityHost")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity){
            activity.Id=id;
            return HandleResult(await Mediator.Send(new Edit.Command{Activity=activity}));
        }

        [Authorize(Policy ="IsActivityHost")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id){
            return HandleResult(await Mediator.Send(new Delete.Command{Id=id}));
        }

        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id){
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command{Id=id}));
        }
    }
}