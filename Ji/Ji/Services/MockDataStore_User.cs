using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ji.Models;

namespace Ji.Services
{
    public class MockDataStore_User : IDataStore<UserChat>
    {
        List<UserChat> items;

        public MockDataStore_User()
        {
            items = new List<UserChat>();
            try
            {
                var mockItems = new List<UserChat>()
            {
                   
                new UserChat { FIO = "Чат первый", UserId=1  },
                new UserChat { FIO = "Чат второй", UserId=2  },
            };
                /*   {
                      new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                      new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                      new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                      new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                      new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                      new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
                  };
                  */
                foreach (var item in mockItems)
                {
                    items.Add(item);
                }
            }catch (Exception err)
            {

            }
        }

        public async Task<bool> AddItemAsync(UserChat item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(UserChat item)
        {
            var oldItem = items.Where((UserChat arg) => arg.UserId == item.UserId).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((UserChat arg) => arg.UserId == Convert.ToInt32(id)).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<UserChat> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.UserId == Convert.ToInt32(id)));
        }

        public async Task<IEnumerable<UserChat>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
 
}