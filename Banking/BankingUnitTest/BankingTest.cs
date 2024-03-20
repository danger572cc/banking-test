using Banking.Exceptions;
using Banking.Interfaces;
using Banking.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace BankingUnitTest
{
    [TestClass]
    public class BankingTest
    {
        private const string CUSTOMER_CSV = "customers.csv";

        private readonly string _csvFilePath;

        public BankingTest()
        {
            _csvFilePath = GetCsvDirectory();
        }

        [TestMethod]
        public void BankingDoubleInitialization()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);
            Assert.ThrowsException<AlreadyInitialiseException>(() => bankingService.Initialise(_csvFilePath));
        }

        [TestMethod]
        public void SearchCustomerByName()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            var query = bankingService.SearchCustomers(name: "meaghan");

            Assert.IsTrue(query != null);
            Assert.IsTrue(query.Count() > 0);
        }

        [TestMethod]
        public void FeeBothCustomerInternalAmount50()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            var fee = bankingService.CalculateFees(34, 22, 50d);

            Assert.IsTrue(fee == 0d);
        }

        [TestMethod]
        public void FeeBothCustomerInternalAmount100()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            var fee = bankingService.CalculateFees(34, 22, 100d);

            Assert.IsTrue(fee == 5d);
        }

        [TestMethod]
        public void FeeInternalGiverExternalBeneficiaryAmount75()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            var fee = bankingService.CalculateFees(34, 76, 75);

            Assert.IsTrue(fee == 10d);
        }

        [TestMethod]
        public void TransferExternalGiver()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            Assert.ThrowsException<ExternallGiverException>(() => bankingService.Transfer(74, 162, 10d));
        }

        [TestMethod]
        public void TransferNegativeAmount()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            Assert.ThrowsException<InvalidAmountException>(() => bankingService.Transfer(161, 162, -50d));
        }

        [TestMethod]
        public void TransferExceedBalance()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            Assert.ThrowsException<NotMoneyAvailableException>(() => bankingService.Transfer(41, 75, 40d));
        }

        [TestMethod]
        public void TransferInternalGiverExternalBeneficiary()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            bankingService.Transfer(41, 75, 10d);

            var emiterInfo = bankingService.SearchCustomers(41).FirstOrDefault();
            var receiverInfo = bankingService.SearchCustomers(75).FirstOrDefault();

            Assert.IsTrue(emiterInfo.Balance == 20);
            Assert.IsTrue(receiverInfo.Balance == 84);
        }

        [TestMethod]
        public void TransferBothCustomerInternal()
        {
            IBanking bankingService = new BankingService();
            bankingService.Initialise(_csvFilePath);

            bankingService.Transfer(275, 475, 20d);

            var emiterInfo = bankingService.SearchCustomers(275).FirstOrDefault();
            var receiverInfo = bankingService.SearchCustomers(475).FirstOrDefault();

            Assert.IsTrue(emiterInfo.Balance == 254);
            Assert.IsTrue(receiverInfo.Balance == 494);
        }

        #region private methods
        private string GetCsvDirectory() 
        {
            string csvDirectory = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, CUSTOMER_CSV);
            return csvDirectory;
        }
        #endregion
    }
}
