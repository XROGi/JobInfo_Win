using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ji.Models;
using Ji.Services;

namespace Ji.ViewModels
{
    internal class DataStore_ObjMsg : IDataStore<ObjMsg>
    {
        List<ObjMsg> items;

        public async Task<bool> AddItemAsync(ObjMsg item)
        {
            items.Add(item);

            return await Task.FromResult(true);
            //     throw new System.NotImplementedException();
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((ObjMsg arg) => arg.ObjId == Convert.ToInt32(id)).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<ObjMsg> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.ObjId == Convert.ToInt32(id)));
        }

        public async Task<IEnumerable<ObjMsg>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async  Task<bool> UpdateItemAsync(ObjMsg item)
        {
            var oldItem = items.Where((ObjMsg arg) => arg.ObjId == item.ObjId).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
            //throw new System.NotImplementedException();
        }
    }
}