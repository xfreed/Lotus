using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lotus.BBL.Interfaces;
using Tweetinvi;
using Tweetinvi.Models;

namespace Lotus.BBL.Services.Twitter
{
    public class Twitter : ITwitter
    {
        private TwitterClient _twitterClient;

        public void GetUserCredentials(string consumerKey, string consumerSecret, string accessToken,
            string accessTokenSecret)
        {
            try
            { 
                _twitterClient = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
                Resources.ResourceWorker.ChangeResourceText("ConsumerKey",consumerKey);
                Resources.ResourceWorker.ChangeResourceText("ConsumerSecret", consumerSecret);
                Resources.ResourceWorker.ChangeResourceText("AccessToken", accessToken);
                Resources.ResourceWorker.ChangeResourceText("AccessTokenSecret", accessTokenSecret);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void TryUserCredentials()
        {
            try
            {
                _twitterClient = new TwitterClient(Resources.Settings.ConsumerKey, Resources.Settings.ConsumerSecret,
                    Resources.Settings.AccessToken, Resources.Settings.AccessTokenSecret);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void PublishTweetAsync(string tweetText) => await _twitterClient.Tweets.PublishTweetAsync(tweetText);

        public async Task<ITweet[]> FindUsersByTweetText(string tweetText) => await _twitterClient.Search.SearchTweetsAsync(tweetText);

        public void ChangeTweetDelayTime(string milliseconds) => Resources.ResourceWorker.ChangeResourceText("TwitterFollowDelay",milliseconds);

        public async Task FollowUsers(IEnumerable<IUser> usersEnumerable)
        {
            foreach (var username in usersEnumerable)
            {
                try
                {
                    //TODO Set max application limit somewhere
                    await FollowUser(username);
                    Task.Delay(int.Parse(Resources.Settings.TwitterFollowDelay)).Wait(); // Wait X second after next follow
                    var a = await _twitterClient.RateLimits.GetRateLimitsAsync();
                    //a.ApplicationRateLimitStatusLimit.Remaining
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        private async Task FollowUser(IUserIdentifier username) => await _twitterClient.Users.FollowUserAsync(username);
    }
}