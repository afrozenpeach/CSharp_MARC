/**
 * Parser for MARC records
 *
 * This project is based on the File_MARC package 
 * (http://pear.php.net/package/File_MARC) by Dan Scott , which was based on PHP
 * MARC package, originally called "php-marc", that is part of the Emilda 
 * Project (http://www.emilda.org). Both projects were released under the LGPL
 * which allowed me to port the project to C# for use with the .NET Framework.
 * 
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * @author    Matt Schraeder <mschraeder@btsb.com> <mschraeder@csharpmarc.net>
 * @copyright 2009-2011 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for FileMARCTest and is intended
    ///to contain all FileMARCTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileMARCTest
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
		///A test for FileMARC Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCConstructorTest()
		{
			FileMARC target = new FileMARC();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for FileMARC Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCConstructorTest1()
		{
			string source = string.Empty; // TODO: Initialize to an appropriate value
			FileMARC target = new FileMARC(source);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod()]
		public void AddTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			string source = string.Empty; // TODO: Initialize to an appropriate value
			target.Add(source);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for CleanSource
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void CleanSourceTest()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			string source = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetEnumerator
		///</summary>
		[TestMethod()]
		public void GetEnumeratorTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			IEnumerator expected = null; // TODO: Initialize to an appropriate value
			IEnumerator actual;
			actual = target.GetEnumerator();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ImportMARC
		///</summary>
		[TestMethod()]
		public void ImportMARCTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			string file = string.Empty; // TODO: Initialize to an appropriate value
			target.ImportMARC(file);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for MoveNext
		///</summary>
		[TestMethod()]
		public void MoveNextTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Reset
		///</summary>
		[TestMethod()]
		public void ResetTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			target.Reset();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ResetWarnings
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void ResetWarningsTest()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			target.ResetWarnings();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for decode
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void decodeTest()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			int index = 0; // TODO: Initialize to an appropriate value
			Record expected = null; // TODO: Initialize to an appropriate value
			Record actual;
			actual = target.decode(index);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Count
		///</summary>
		[TestMethod()]
		public void CountTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			int actual;
			actual = target.Count;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Current
		///</summary>
		[TestMethod()]
		public void CurrentTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			object actual;
			actual = target.Current;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			int index = 0; // TODO: Initialize to an appropriate value
			Record actual;
			actual = target[index];
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for RawSource
		///</summary>
		[TestMethod()]
		public void RawSourceTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			List<string> actual;
			actual = target.RawSource;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Warnings
		///</summary>
		[TestMethod()]
		public void WarningsTest()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			List<string> actual;
			actual = target.Warnings;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for FileMARC Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCConstructorTest2()
		{
			FileMARC target = new FileMARC();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for FileMARC Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCConstructorTest3()
		{
			string source = string.Empty; // TODO: Initialize to an appropriate value
			FileMARC target = new FileMARC(source);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod()]
		public void AddTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			string source = string.Empty; // TODO: Initialize to an appropriate value
			target.Add(source);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for CleanSource
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void CleanSourceTest1()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			string source = string.Empty; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetEnumerator
		///</summary>
		[TestMethod()]
		public void GetEnumeratorTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			IEnumerator expected = null; // TODO: Initialize to an appropriate value
			IEnumerator actual;
			actual = target.GetEnumerator();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ImportMARC
		///</summary>
		[TestMethod()]
		public void ImportMARCTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			string file = string.Empty; // TODO: Initialize to an appropriate value
			target.ImportMARC(file);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for MoveNext
		///</summary>
		[TestMethod()]
		public void MoveNextTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Reset
		///</summary>
		[TestMethod()]
		public void ResetTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			target.Reset();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ResetWarnings
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void ResetWarningsTest1()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			target.ResetWarnings();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for decode
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void decodeTest1()
		{
			FileMARC_Accessor target = new FileMARC_Accessor(); // TODO: Initialize to an appropriate value
			int index = 0; // TODO: Initialize to an appropriate value
			Record expected = null; // TODO: Initialize to an appropriate value
			Record actual;
			actual = target.decode(index);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Count
		///</summary>
		[TestMethod()]
		public void CountTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			int actual;
			actual = target.Count;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Current
		///</summary>
		[TestMethod()]
		public void CurrentTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			object actual;
			actual = target.Current;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			int index = 0; // TODO: Initialize to an appropriate value
			Record actual;
			actual = target[index];
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for RawSource
		///</summary>
		[TestMethod()]
		public void RawSourceTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			List<string> actual;
			actual = target.RawSource;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Warnings
		///</summary>
		[TestMethod()]
		public void WarningsTest1()
		{
			FileMARC target = new FileMARC(); // TODO: Initialize to an appropriate value
			List<string> actual;
			actual = target.Warnings;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
