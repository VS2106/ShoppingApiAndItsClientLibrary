using NUnit.Framework;
using ShoppingAPI.App_Start;

namespace ShoppingAPI.Tests
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            AutoMapperConfig.Initialize();
        }
    }
}
