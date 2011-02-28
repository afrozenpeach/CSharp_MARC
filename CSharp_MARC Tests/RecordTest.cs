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
using System.Collections.Generic;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for RecordTest and is intended
    ///to contain all RecordTest Unit Tests
    ///</summary>
	[TestClass()]
	public class RecordTest
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
		///A test for Record Constructor
		///</summary>
		[TestMethod()]
		public void RecordConstructorTest()
		{
			Record target = new Record();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for GetFields
		///</summary>
		[TestMethod()]
		public void GetFieldsTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Field> expected = null; // TODO: Initialize to an appropriate value
			List<Field> actual;
			actual = target.GetFields(tag);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for InsertField
		///</summary>
		[TestMethod()]
		public void InsertFieldTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			Field newField = null; // TODO: Initialize to an appropriate value
			target.InsertField(newField);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
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
			Record target = new Record(); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Fields
		///</summary>
		[TestMethod()]
		public void FieldsTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			List<Field> expected = null; // TODO: Initialize to an appropriate value
			List<Field> actual;
			target.Fields = expected;
			actual = target.Fields;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			Field actual;
			actual = target[tag];
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Leader
		///</summary>
		[TestMethod()]
		public void LeaderTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			target.Leader = expected;
			actual = target.Leader;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Warnings
		///</summary>
		[TestMethod()]
		public void WarningsTest()
		{
			Record target = new Record(); // TODO: Initialize to an appropriate value
			List<string> actual;
			actual = target.Warnings;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
