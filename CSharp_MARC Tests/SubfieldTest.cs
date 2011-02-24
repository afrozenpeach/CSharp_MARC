using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for SubfieldTest and is intended
    ///to contain all SubfieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class SubfieldTest
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
		///A test for Subfield Constructor
		///</summary>
		[TestMethod()]
		public void SubfieldConstructorTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			Assert.IsInstanceOfType(target, typeof(Subfield));
			Assert.AreEqual(target.Code, code);
			Assert.AreEqual(target.Data, data);
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			bool expected = false;
			bool actual;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			target.Data = null;
			expected = true;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			target.Data = string.Empty;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			string expected = "\x1F" + "aTest Data";
			string actual;
			actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			string expected = "[a]: Test Data";
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Code
		///</summary>
		[TestMethod()]
		public void CodeTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			char expected = 'b';
			char actual;
			target.Code = expected;
			actual = target.Code;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Data
		///</summary>
		[TestMethod()]
		public void DataTest()
		{
			char code = 'a';
			string data = "Test Data";
			Subfield target = new Subfield(code, data);
			string expected = "Test Data Again";
			string actual;
			target.Data = expected;
			actual = target.Data;
			Assert.AreEqual(expected, actual);
		}
	}
}
