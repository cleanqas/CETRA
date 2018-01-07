using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadDataManager<TUploadData> where TUploadData : UploadDataEntity
    {
        private UploadDataStore<TUploadData> _store;

        public UploadDataManager(UploadDataStore<TUploadData> store)
        {
            _store = store;
        }

        public void Create(TUploadData upload)
        {
            AsyncHelper.RunSync(() => _store.CreateAsync(upload));
        }

        public async Task<bool> CreateAsync(TUploadData upload)
        {
            try
            {
                await _store.CreateAsync(upload);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string uploadId)
        {
            try
            {
                await _store.DeleteAsync(uploadId);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
        }
    }
}
