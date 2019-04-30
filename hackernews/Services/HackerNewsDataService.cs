using hackernews.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace hackernews.Services
{
    public interface IHackerNewsDataService
    {
        Task<List<HackerNewsModel>> GetNewsListAsync();
        Task<HackerNewsModel> GetNewsItemFromCacheAsync(int newsId);
        Task<HackerNewsModel> GetNewsByIdAsync(int newsId);
    }

    public class HackerNewsDataService : IHackerNewsDataService
    {
        private IMemoryCache cache;
        public HackerNewsDataService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<HackerNewsModel>> GetNewsListAsync()
        {
            List<HackerNewsModel> newsList = new List<HackerNewsModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(Constants.TOP_STORIES_URL);
                if (response.IsSuccessStatusCode)
                {
                    var newsResponse = response.Content.ReadAsStringAsync().Result;
                    var newsIds = JsonConvert.DeserializeObject<List<int>>(newsResponse);

                    var tasks = newsIds.Select(GetNewsItemFromCacheAsync);
                    newsList = (await Task.WhenAll(tasks)).ToList();
                }
                return newsList;
            }
        }

        public async Task<HackerNewsModel> GetNewsItemFromCacheAsync(int newsId)
        {
            return await cache.GetOrCreateAsync(newsId, async cacheEntry =>
            {
                return await GetNewsByIdAsync(newsId);
            });
        }

        public async Task<HackerNewsModel> GetNewsByIdAsync(int newsId)
        {
            using (HttpClient client = new HttpClient())
            {
                HackerNewsModel newsModel = new HackerNewsModel();

                var response = await client.GetAsync(string.Format(Constants.ITEM_URL, newsId));
                if (response.IsSuccessStatusCode)
                {
                    var storyResponse = response.Content.ReadAsStringAsync().Result;
                    newsModel = JsonConvert.DeserializeObject<HackerNewsModel>(storyResponse);
                }

                return newsModel;
            }
        }
    }
}
