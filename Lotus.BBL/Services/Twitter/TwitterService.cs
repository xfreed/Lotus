using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lotus.BBL.Interfaces;
using Lotus.BBL.Models;
using Tweetinvi;
using Tweetinvi.Models;

namespace Lotus.BBL.Services.Twitter
{
    public class TwitterService : ITwitterService
    {
        private readonly ITwitterClient _twitterClient;

        public TwitterService()
        {
            _twitterClient = new TwitterClient(Resources.Settings.ConsumerKey, Resources.Settings.ConsumerSecret,
                Resources.Settings.AccessToken, Resources.Settings.AccessTokenSecret);
        }
        
        public void GetUserCredentials(string consumerKey, string consumerSecret, string accessToken,
            string accessTokenSecret)
        {
            try
            { 
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

        public async Task TryUserCredentialsAsync()
        {
            try
            {
                await _twitterClient.Users.GetAuthenticatedUserAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void PublishTweetAsync(string tweetText) => await _twitterClient.Tweets.PublishTweetAsync(tweetText);

        public async Task<IEnumerable<IUser>> FindUsersByTweetTextAsync(string tweetText)
        {
            var userTweets = await _twitterClient.Search.SearchTweetsAsync(tweetText);
            return userTweets.Select(x => x.CreatedBy).Distinct();
        }

        public void ChangeTweetDelayTime(string milliseconds) => Resources.ResourceWorker.ChangeResourceText("TwitterFollowDelay",milliseconds); // 240000 milliseconds for 24/7 adding friends

        public async Task UnFollowUsersNotFollowingYouAsync()
        {
            var accountSettings = await _twitterClient.AccountSettings.GetAccountSettingsAsync();
            var followers = await _twitterClient.Users.GetFollowersAsync(accountSettings.ScreenName);
            var following = await _twitterClient.Users.GetFriendsAsync(accountSettings.ScreenName);
            var toUnFollow = following.Select(x => x.ScreenName).Except(followers.Select(x => x.ScreenName)).ToList();
            try
            {
                foreach (var username in toUnFollow)
                {
                    //TODO Set max application limit somewhere
                    await _twitterClient.Users.UnfollowUserAsync(username);
                    Task.Delay(int.Parse(Resources.Settings.TwitterFollowDelay))
                        .Wait(); // Wait X second after next unfollow

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task FollowUsersByTextAsync(string text)
        {
            var usersEnumerable = await FindUsersByTweetTextAsync(text);
            foreach (var username in usersEnumerable)
            {
                try
                {
                    //TODO Set max application limit somewhere
                    await FollowUserAsync(username);
                    Task.Delay(int.Parse(Resources.Settings.TwitterFollowDelay)).Wait(); // Wait X second after next follow
                    var a = await _twitterClient.RateLimits.GetRateLimitsAsync();
                    //a.ApplicationRateLimitStatusLimit.Remaining
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
        public async Task<AccountData> GetUserInformation()
        {
            IAccountSettings accountSettings = null;
            IUser[] followers=null,following=null;
            try
            {
                accountSettings = await _twitterClient.AccountSettings.GetAccountSettingsAsync();
                followers = await _twitterClient.Users.GetFollowersAsync(accountSettings.ScreenName);
                following = await _twitterClient.Users.GetFriendsAsync(accountSettings.ScreenName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new AccountData
            {
                Username = accountSettings?.ScreenName,
                Followers = followers?.Length.ToString(),
                Following = following?.Length.ToString()
            };
        }



        private async Task FollowUserAsync(IUserIdentifier username) => await _twitterClient.Users.FollowUserAsync(username);
        
    }
}