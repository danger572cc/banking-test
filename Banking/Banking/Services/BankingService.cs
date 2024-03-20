using Banking.Entities;
using Banking.Exceptions;
using Banking.Interfaces;
using System;
using System.Linq;

namespace Banking.Services
{
    public class BankingService : IBanking
    {
        private readonly CustomerDatabase _db;

        private bool _allReadyCalled = false;

        public BankingService()
        {
            _db = new CustomerDatabase();
        }

        public double CalculateFees(long giverId, long beneficiaryId, double amount)
        {
            double fee = 0d;
            var emiterCustomer = SearchCustomers(giverId).FirstOrDefault();
            if (emiterCustomer.Type == "external") 
            {
                throw new ExternallGiverException("El emisor es externo no es posible realizar la transferencia.");
            }
            var receiverCustomer = SearchCustomers(beneficiaryId).FirstOrDefault();
            if (emiterCustomer.Type == "internal" && receiverCustomer.Type == "internal" && amount >= 100d) 
            {
                fee = 5d;
            }
            if (emiterCustomer.Type == "internal" && receiverCustomer.Type == "external")
            {
                fee = 10d;
            }
            return fee;
        }

        public long CountCustomers()
        {
            return _db.Customers.Count;
        }

        public void Initialise(string path)
        {
            var init = CallOnlyOnce(_db.DbContext, path);
            init();
        }

        public Customer[] SearchCustomers(long? id = null, string name = null)
        {
            var queryResult = Array.Empty<Customer>();
            if (!string.IsNullOrEmpty(name) && id == null)
            {
                queryResult = _db.Customers.Where(f => f.Name.ToLower() == name.ToLower()).ToArray();
            }
            else if (id.HasValue)
            {
                queryResult = _db.Customers.Where(f => f.Id == id.GetValueOrDefault()).ToArray();
            }
            return queryResult;
        }

        public void Transfer(long giverId, long beneficiaryId, double amount)
        {
            if (amount < 0) 
            {
                throw new InvalidAmountException("El monto de la operación no es válido.");
            }
            var transferFee = CalculateFees(giverId, beneficiaryId, amount);
            var totalTransfer = amount + transferFee;
            var emiterCustomer = SearchCustomers(giverId).FirstOrDefault();
            if (emiterCustomer.Balance < totalTransfer) 
            {
                throw new NotMoneyAvailableException("Saldo insufiente para realizar la operación.");
            }            
            var receiverCustomer = SearchCustomers(beneficiaryId).FirstOrDefault();
            //Debitar
            emiterCustomer.Balance -= totalTransfer;
            //Acreditar
            receiverCustomer.Balance += amount;
        }

        #region private methods 
        private Action CallOnlyOnce(Action<string> action, string path)
        {
            Action ret = () =>
            {
                if (!_allReadyCalled)
                {
                    action(path);
                    _allReadyCalled = true;
                }
                else
                {
                    throw new AlreadyInitialiseException($"El método {action.Method.Name} sólo puede invocarse una sola vez");
                }
            };
            return ret;
        }
        #endregion
    }
}
