using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Lotus.BBL.Interfaces
{
   public interface ITwitter
    {
        public  void GetUserCredentials(string consumerKey, string consumerSecret, string accessToken,
            string accessTokenSecret);

        public void TryUserCredentials();
        public void PublishTweetAsync(string tweetText);
        public Task<ITweet[]> FindUsersByTweetText(string tweetText);
        public void ChangeTweetDelayTime(string milliseconds);
    }
}
