using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ji.Models;

namespace Ji.Services
{
    public class MockDataStore : IDataStore<GroupChat>
    {
        List<GroupChat> items;

        public MockDataStore()
        {
            items = new List<GroupChat>();
            try
            {
                var mockItems = new List<GroupChat>()
            {
                   
                new GroupChat { Text = "Чат первый", ObjId=1, TypeId = MsgObjType.PrivateChatd },
                new GroupChat { Text = "Чат второй", ObjId=2, TypeId = MsgObjType.PrivateChatd },
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

        public async Task<bool> AddItemAsync(GroupChat item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(GroupChat item)
        {
            var oldItem = items.Where((GroupChat arg) => arg.ObjId == item.ObjId).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((GroupChat arg) => arg.ObjId ==Convert.ToInt32(id)).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<GroupChat> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.ObjId == Convert.ToInt32(id)));
        }

        public async Task<IEnumerable<GroupChat>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
    /*
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }*/
}