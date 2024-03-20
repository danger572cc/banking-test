using Banking.Entities;
using Banking.Interfaces;
using Core.Singleton;
using System.Collections.ObjectModel;
using System.Text;

namespace Banking.Services
{
    //public class CustomerDatabase : Singleton<CustomerDatabase>
    public class CustomerDatabase
    {
        public readonly ICsvFileService _csvService;

        public ReadOnlyCollection<Customer> Customers { get; private set; }

        public CustomerDatabase()
        {
            _csvService = new CsvFileService(Encoding.UTF8, ";", true);
        }

        public void DbContext(string path) 
        {
            Customers = _csvService.Read<Customer>(path);
        }
    }
}
