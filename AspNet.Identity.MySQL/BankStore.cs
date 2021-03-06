﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Identity.MySQL
{
    public class BankStore<TBank> where TBank : IdentityBank
    {
        private BankTable bankTable;
        
        public MySQLDatabase Database { get; private set; }        

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public BankStore()
        {
            new BankStore<TBank>(new MySQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public BankStore(MySQLDatabase database)
        {
            Database = database;
            bankTable = new BankTable(database);
        }

        public Task CreateAsync(TBank bank)
        {
            if (bank == null)
            {
                throw new ArgumentNullException("bank");
            }

            bankTable.Insert(bank);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TBank bank)
        {
            if (bank == null)
            {
                throw new ArgumentNullException("bank");
            }
            bankTable.DeleteBankGlAccounts(bank.Id);
            bankTable.Delete(bank.Id);

            return Task.FromResult<Object>(null);
        }

        public Task UpdateAsync(TBank bank)
        {
            if (bank == null)
            {
                throw new ArgumentNullException("bank");
            }
            bankTable.Update(bank);

            return Task.FromResult<Object>(null);
        }

        public Task<TBank> FindByIdAsync(string bankId)
        {
            TBank result = bankTable.GetBankById(bankId) as TBank;

            return Task.FromResult<TBank>(result);
        }

        public Task<List<TBank>> GetAllBanksAsync()
        {
            List<TBank> result = bankTable.GetAllBanks() as List<TBank>;

            return Task.FromResult<List<TBank>>(result);
        }
    }
}
