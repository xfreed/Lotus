using System;
using System.Threading.Tasks;
using Hangfire;
using Lotus.BBL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lotus.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITwitterService _twitterService;
        public IndexModel(ILogger<IndexModel> logger, ITwitterService twitterService)
        {
            _logger = logger;
            _twitterService = twitterService;
        }

        public void OnGet()
        {
            RecurringJob.AddOrUpdate<ITwitterService>(x =>
                x.FollowUsersByTextAsync("#lofi"), "5 4 * * *");
            RecurringJob.AddOrUpdate<ITwitterService>(x =>
                x.UnFollowUsersNotFollowingYouAsync(), Cron.Daily);
        }

        public IActionResult OnGetTwitterAutoFollowing(string text)
        {
            try
            {
                //TODO add signalr
                _twitterService.FollowUsersByTextAsync(text);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return new JsonResult("Start");
        }
        public IActionResult OnGetCheckCredentials()
        {
            string state = "fail";
            try
            {
                _twitterService.TryUserCredentialsAsync();
                state = "good";
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return new JsonResult(state);
        }

        public async Task<IActionResult> OnGetAccountData()
        {
           var data = await _twitterService.GetUserInformation();
            return new JsonResult(data);
        }
    }
}
