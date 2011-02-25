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
    ///This is a test class for DataFieldTest and is intended
    ///to contain all DataFieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class DataFieldTest
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
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest2()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			char ind1 = '\0'; // TODO: Initialize to an appropriate value
			char ind2 = '\0'; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields, ind1, ind2);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest3()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			char ind1 = '\0'; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields, ind1);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char[] excludeCodes = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.FormatField(excludeCodes);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.FormatField();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			actual = target.GetSubfields();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			Nullable<char> code = new Nullable<char>(); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			actual = target.GetSubfields(code);
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
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
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
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
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
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ValidateIndicator
		///</summary>
		[TestMethod()]
		public void ValidateIndicatorTest()
		{
			char ind = '\0'; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = DataField.ValidateIndicator(ind);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Indicator1
		///</summary>
		[TestMethod()]
		public void Indicator1Test()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char expected = '\0'; // TODO: Initialize to an appropriate value
			char actual;
			target.Indicator1 = expected;
			actual = target.Indicator1;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Indicator2
		///</summary>
		[TestMethod()]
		public void Indicator2Test()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char expected = '\0'; // TODO: Initialize to an appropriate value
			char actual;
			target.Indicator2 = expected;
			actual = target.Indicator2;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char code = '\0'; // TODO: Initialize to an appropriate value
			Subfield actual;
			actual = target[code];
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Subfields
		///</summary>
		[TestMethod()]
		public void SubfieldsTest()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			target.Subfields = expected;
			actual = target.Subfields;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest4()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest5()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest6()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			char ind1 = '\0'; // TODO: Initialize to an appropriate value
			char ind2 = '\0'; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields, ind1, ind2);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest7()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			List<Subfield> subfields = null; // TODO: Initialize to an appropriate value
			char ind1 = '\0'; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag, subfields, ind1);
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest2()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char[] excludeCodes = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.FormatField(excludeCodes);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest3()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.FormatField();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest2()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			actual = target.GetSubfields();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest3()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			Nullable<char> code = new Nullable<char>(); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			actual = target.GetSubfields(code);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
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
		public void ToRawTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
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
		public void ToStringTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for ValidateIndicator
		///</summary>
		[TestMethod()]
		public void ValidateIndicatorTest1()
		{
			char ind = '\0'; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = DataField.ValidateIndicator(ind);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Indicator1
		///</summary>
		[TestMethod()]
		public void Indicator1Test1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char expected = '\0'; // TODO: Initialize to an appropriate value
			char actual;
			target.Indicator1 = expected;
			actual = target.Indicator1;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Indicator2
		///</summary>
		[TestMethod()]
		public void Indicator2Test1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char expected = '\0'; // TODO: Initialize to an appropriate value
			char actual;
			target.Indicator2 = expected;
			actual = target.Indicator2;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			char code = '\0'; // TODO: Initialize to an appropriate value
			Subfield actual;
			actual = target[code];
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Subfields
		///</summary>
		[TestMethod()]
		public void SubfieldsTest1()
		{
			string tag = string.Empty; // TODO: Initialize to an appropriate value
			DataField target = new DataField(tag); // TODO: Initialize to an appropriate value
			List<Subfield> expected = null; // TODO: Initialize to an appropriate value
			List<Subfield> actual;
			target.Subfields = expected;
			actual = target.Subfields;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
