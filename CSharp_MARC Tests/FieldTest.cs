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
 * @author    Matt Schraeder-Urbanowicz <matt@btsb.com> <matt@csharpmarc.net>
 * @copyright 2009-2016 Matt Schraeder-Urbanowicz and Bound to Stay Bound Books <http://www.btsb.com>
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
        /// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

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
		    List<Subfield> subfields = new List<Subfield>
		    {
		        new Subfield('a', "It's a book!"),
		        new Subfield('b', "Anne Author")
		    };
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
			string actual = target.FormatField();
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
			bool actual = target.IsControlField();
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
			bool actual = target.IsDataField();
			Assert.AreEqual(expected, actual);
			expected = false;
			target = CreateControlField();
			actual = target.IsDataField();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			Field target = CreateControlField();
			bool expected = false;
			bool actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			expected = true;
			((ControlField)target).Data = string.Empty;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			expected = false;
			target = CreateDataField();
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			expected = true;
			((DataField)target).Subfields.Clear();
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			Field target = CreateControlField();
			string expected = "  2007032296" + FileMARC.END_OF_FIELD.ToString();
			string actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
			target = CreateDataField();
			expected = "1 " + FileMARC.SUBFIELD_INDICATOR.ToString() + "aIt's a book!" + FileMARC.SUBFIELD_INDICATOR.ToString() + "bAnne Author" + FileMARC.END_OF_FIELD.ToString();
			actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			Field target = CreateControlField();
			string expected = "001       2007032296";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
			target = CreateDataField();
			expected = "100 1  [a]: It's a book!" + Environment.NewLine + "       [b]: Anne Author";
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Tag
		///</summary>
		[TestMethod()]
		public void TagTest()
		{
			Field target = CreateControlField();
			string expected = "001";
			string actual = target.Tag;
            target.Tag = expected;
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
			bool actual = Field.ValidateTag(tag);
			Assert.AreEqual(expected, actual);
			tag = "###";
			expected = false;
			actual = Field.ValidateTag(tag);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Clone
		///</summary>
		[TestMethod()]
		public void CloneTest()
		{
			Field target = CreateControlField();
			Field expected = target;
			Field actual = target.Clone();
			Assert.AreNotEqual(expected, actual);

			target = CreateDataField();
			expected = target;
			actual = target.Clone();
			Assert.AreNotEqual(expected, actual);
		}
	}
}
