using System.Reflection;
using NUnit.Framework;

namespace TIZSoft.Log.Tests
{
	[TestFixture]
	public class LogManagerTests
	{
		LogManager logManager;

		[SetUp]
		public void Setup()
		{
            logManager = LogManager.Default;
		}

		[TearDown]
		public void TearDown()
		{
			var defaultInstance = typeof(LogManager)
				.GetField("defaultInstance", BindingFlags.Static | BindingFlags.NonPublic);
			if (defaultInstance != null)
			{
				defaultInstance.SetValue(null, null);
			}
		}

		[Test]
		public void Test_FindOrCreateCurrentTypeLogger_Success()
		{
			var logger = logManager.FindOrCreateCurrentTypeLogger();
			Assert.AreEqual(typeof(LogManagerTests).FullName, logger.Name);
		}
    }
}
