﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class UploadManager<TUpload> where TUpload : UploadEntity
    {
        private UploadStore<TUpload> _store;

        public UploadManager(UploadStore<TUpload> store)
        {
            _store = store;
        }

        public void Create(TUpload upload)
        {
            AsyncHelper.RunSync(() => _store.CreateAsync(upload));
        }        

        public async Task<bool> CreateAsync(TUpload upload)
        {
            await _store.CreateAsync(upload);
            return true;
            //try
            //{
            //    await _store.CreateAsync(upload);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    //TODO: Log the exception
            //    return false;
            //}
        }

        public async Task<bool> DeleteAsync(TUpload upload)
        {
            try
            {
                await _store.DeleteAsync(upload);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
        }

        public bool Delete(TUpload upload)
        {
            
            try
            {
                AsyncHelper.RunSync(() => _store.DeleteAsync(upload));
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return false;
            }
        }

        public List<TUpload> FindPendingUploadsByBranchIdAsync(string branchId)
        {
            return AsyncHelper.RunSync<List<TUpload>>(() => _store.FindPendingUploadsByBranchIdAsync(branchId));
        }

        public int UpdateUploadStatus(string uploadId, int status)
        {
            return AsyncHelper.RunSync<int>(() => _store.UpdateUploadStatus(uploadId, status));
        }

        public int UpdateUploadOperator(string uploadId, string operatorId)
        {
            return AsyncHelper.RunSync<int>(() => _store.UpdateUploadOperator(uploadId, operatorId));
        }
    }
}
