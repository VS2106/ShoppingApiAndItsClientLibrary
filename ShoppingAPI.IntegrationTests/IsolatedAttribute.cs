using System;
using System.Transactions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ShoppingAPI.IntegrationTests
{
    //Attribute that roll back db after each test
    public class IsolatedAttribute : Attribute, ITestAction
    {
        private TransactionScope _transactionScope;
        public void BeforeTest(ITest test)
        {
            _transactionScope = new TransactionScope();
        }

        public void AfterTest(ITest test)
        {
            _transactionScope.Dispose();
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}
