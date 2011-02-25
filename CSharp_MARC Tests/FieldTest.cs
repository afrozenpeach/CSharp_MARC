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
    ///This is a test class for FieldTest and is intended
    ///to contain all FieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FieldTest
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


		internal virtual Field CreateControlField()
		{
			Field target = new ControlField("001", "  2007032296");
			return target;
		}

		internal virtual Field CreateDataField()
		{
			List<Subfield> subfields = new List<Subfield>();
			subfields.Add(new Subfield('a', "It's a book!"));
			subfields.Add(new Subfield('b', "Anne Author"));
			Field target = new DataField("100", subfields, '1', ' ');
			return target;
		}

		/// <summary>
		///A test for FormatField
		///Relying on derived members to test actual output. This test is verify I can the function is accessible as the base class.
		///</summary>
		[TestMethod()]
		public void FormatFieldTest()
		{
			Field target = CreateControlField();
			string expected = string.Empty;
			string actual;
			actual = target.FormatField();
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		///A test for IsControlField
		///</summary>
		[TestMethod()]
		public void IsControlFieldTest()
		{
			Field target = CreateControlField();
			bool expected = true; 
			bool actual;
			actual = target.IsControlField();
			Assert.AreEqual(expected, actual);
			target = CreateDataField();
			expected = false;
			actual = target.IsControlField();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsDataField
		///</summary>
		[TestMethod()]
		public void IsDataFieldTest()
		{
			Field target = CreateDataField(); 
			bool expected = true;
			bool actual;
			actual = target.IsDataField();
			Assert.AreEqual(expected, actual);
			expected = false;
			target = CreateControlField();
			actual = target.IsDataField();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsEmpty
		///Relying on derived members to test actual output. This test is verify I can the function is accessible as the base class.
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			Field target = CreateControlField();
			bool expected = false;
			bool actual;
			actual = target.IsEmpty();
			Assert.AreNotEqual(expected, actual);
			target = CreateDataField();
			actual = target.IsEmpty();
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///Relying on derived members to test actual output. This test is verify I can the function is accessible as the base class.
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			Field target = CreateControlField();
			string expected = string.Empty;
			string actual;
			actual = target.ToRaw();
			Assert.AreNotEqual(expected, actual);
			target = CreateDataField();
			actual = target.ToRaw();
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///Relying on derived members to test actual output. This test is verify I can the function is accessible as the base class.
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			Field target = CreateControlField(); 
			string expected = string.Empty;
			string actual;
			actual = target.ToString();
			Assert.AreNotEqual(expected, actual);
			target = CreateDataField();
			actual = target.ToString();
			Assert.AreNotEqual(expected, actual);
		}

		/// <summary>
		///A test for Tag
		///</summary>
		[TestMethod()]
		public void TagTest()
		{
			Field target = CreateControlField();
			string expected = "001";
			string actual;
			target.Tag = expected;
			actual = target.Tag;
			Assert.AreEqual(expected, actual);
			target = CreateDataField();
			expected = "100";
			actual = target.Tag;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ValidateTag
		///</summary>
		[TestMethod()]
		public void ValidateTagTest()
		{
			string tag = "001";
			bool expected = true;
			bool actual;
			actual = Field.ValidateTag(tag);
			Assert.AreEqual(expected, actual);
			tag = "###";
			expected = false;
			actual = Field.ValidateTag(tag);
			Assert.AreEqual(expected, actual);
		}
	}
}
