using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for ControlFieldTest and is intended
    ///to contain all ControlFieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ControlFieldTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for ControlField Constructor
		///</summary>
		[TestMethod()]
		public void ControlFieldConstructorTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.FormatField();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data); // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Data
		///</summary>
		[TestMethod()]
		public void DataTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			string data = string.Empty; // TODO: Initialize to an appropriate value
			ControlField target = new ControlField(tag, data); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			target.Data = expected;
			actual = target.Data;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
