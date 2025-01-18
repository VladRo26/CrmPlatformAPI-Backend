using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CrmPlatformAPI.Helpers
{
    public class ActivityLog : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var resultContext = await next();

            if(context.HttpContext.User.Identity.IsAuthenticated != true)
            {
                return;
            }

            var username = resultContext.HttpContext.User.GetUsername();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IRepositoryUser>();

            var user = await repo.GetByUserNameAsync(username);

            if (user == null)
            {
                return;
            }
            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync();



        }
    }
}
