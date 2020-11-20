using System.Collections.Generic;
using System.Threading.Tasks;
using Lotus.BBL.Models;
using Tweetinvi.Models;

namespace Lotus.BBL.Interfaces
{
   public interface ITwitterService
    {
        public  void GetUserCredentials(string consumerKey, string consumerSecret, string accessToken,
            string accessTokenSecret);

        public Task TryUserCredentialsAsync();
        public void PublishTweetAsync(string tweetText);
        public Task<IEnumerable<IUser>> FindUsersByTweetTextAsync(string tweetText);
        public void ChangeTweetDelayTime(string milliseconds);
        public Task FollowUsersByTextAsync(string text);
        public Task UnFollowUsersNotFollowingYouAsync();
        public Task<AccountData> GetUserInformation();
    }
}
