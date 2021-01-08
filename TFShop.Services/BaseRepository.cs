using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TFShop.Services
{
    public abstract class BaseRepository
    {
        protected abstract string TABLE_NAME { get; set; }

        protected readonly CloudStorageAccount _account;
        protected readonly CloudTable _table;

        public BaseRepository()
        {
            try
            {
                _account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureTableStorage"));
                CloudTableClient _tableClient = _account.CreateCloudTableClient(new TableClientConfiguration());
                _table = _tableClient.GetTableReference(TABLE_NAME);
                _table.CreateIfNotExistsAsync();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
