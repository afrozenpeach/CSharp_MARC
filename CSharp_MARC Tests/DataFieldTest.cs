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
 * @author    Mattie Schraeder <mattie@csharpmarc.net>
 * @copyright 2009-2022 Mattie Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CSharp_MARC_Tests
{


    /// <summary>
    ///This is a test class for DataFieldTest and is intended
    ///to contain all DataFieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class DataFieldTest
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


		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);

			{
				char expected = ' ';
				char actual = target.Indicator1;
				Assert.AreEqual(expected, actual);
				actual = target.Indicator2;
				Assert.AreEqual(expected, actual);
				expected = 'a';
				actual = target['a'].Code;
				Assert.AreEqual(expected, actual);
				expected = 'b';
				actual = target['b'].Code;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "100";
				string actual = target.Tag;
				Assert.AreEqual(expected, actual);
				expected = "It's a book!";
				actual = target['a'].Data;
				Assert.AreEqual(expected, actual);
				expected = "Anne Author";
				actual = target['b'].Data;
				Assert.AreEqual(expected, actual);
			}

			{
				int expected = 2;
				int actual = target.Subfields.Count;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest1()
		{
			string tag = "100";
			DataField target = new DataField(tag);

			{
				string expected = "100";
				string actual = target.Tag;
				Assert.AreEqual(expected, actual);
			}

			{
				char expected = ' ';
				char actual = target.Indicator1;
				Assert.AreEqual(expected, actual);
				actual = target.Indicator2;
				Assert.AreEqual(expected, actual);
			}

			{
				int expected = 0;
				int actual = target.Subfields.Count;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest2()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			char ind1 = '1';
			char ind2 = '0';
			DataField target = new DataField(tag, subfields, ind1, ind2);

			{
				char expected = '1';
				char actual = target.Indicator1;
				Assert.AreEqual(expected, actual);
				expected = '0';
				actual = target.Indicator2;
				Assert.AreEqual(expected, actual);
				expected = 'a';
				actual = target['a'].Code;
				Assert.AreEqual(expected, actual);
				expected = 'b';
				actual = target['b'].Code;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "100";
				string actual = target.Tag;
				Assert.AreEqual(expected, actual);
				expected = "It's a book!";
				actual = target['a'].Data;
				Assert.AreEqual(expected, actual);
				expected = "Anne Author";
				actual = target['b'].Data;
				Assert.AreEqual(expected, actual);
			}

			{
				int expected = 2;
				int actual = target.Subfields.Count;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for DataField Constructor
		///</summary>
		[TestMethod()]
		public void DataFieldConstructorTest3()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			char ind1 = '1';
			DataField target = new DataField(tag, subfields, ind1);

			{
				char expected = '1';
				char actual = target.Indicator1;
				Assert.AreEqual(expected, actual);
				expected = ' ';
				actual = target.Indicator2;
				Assert.AreEqual(expected, actual);
				expected = 'a';
				actual = target['a'].Code;
				Assert.AreEqual(expected, actual);
				expected = 'b';
				actual = target['b'].Code;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "100";
				string actual = target.Tag;
				Assert.AreEqual(expected, actual);
				expected = "It's a book!";
				actual = target['a'].Data;
				Assert.AreEqual(expected, actual);
				expected = "Anne Author";
				actual = target['b'].Data;
				Assert.AreEqual(expected, actual);
			}

			{
				int expected = 2;
				int actual = target.Subfields.Count;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest()
		{
			string tag = "600";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			subfield = new Subfield('c', "Some text");
			subfields.Add(subfield);
			subfield = new Subfield('d', "Some more text");
			subfields.Add(subfield);
			subfield = new Subfield('v', "Some fancy text");
			subfields.Add(subfield);
			subfield = new Subfield('z', "Some more fancy text");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			char[] excludeCodes = { 'a', 'b' };
			string expected = "Some text Some more text -- Some fancy text -- Some more fancy text";
			string actual = target.FormatField(excludeCodes);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest1()
		{
			string tag = "600";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			subfield = new Subfield('c', "Some text");
			subfields.Add(subfield);
			subfield = new Subfield('d', "Some more text");
			subfields.Add(subfield);
			subfield = new Subfield('v', "Some fancy text");
			subfields.Add(subfield);
			subfield = new Subfield('z', "Some more fancy text");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			string expected = "It's a book! Anne Author Some text Some more text -- Some fancy text -- Some more fancy text";
			string actual = target.FormatField();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			List<Subfield> expected = new List<Subfield>();
			subfield = new Subfield('a', "It's a book!");
			expected.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			expected.Add(subfield);
			List<Subfield> actual = target.GetSubfields();

			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i].Code, actual[i].Code);
				Assert.AreEqual(expected[i].Data, actual[i].Data);
			}
		}

		/// <summary>
		///A test for GetSubfields
		///</summary>
		[TestMethod()]
		public void GetSubfieldsTest1()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Otter Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			List<Subfield> expected = new List<Subfield>();
			subfield = new Subfield('b', "Anne Author");
			expected.Add(subfield);
			subfield = new Subfield('b', "Anne Otter Author");
			expected.Add(subfield);
			List<Subfield> actual = target.GetSubfields('b');

			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i].Code, actual[i].Code);
				Assert.AreEqual(expected[i].Data, actual[i].Data);
			}
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			string tag = "100";
			DataField target = new DataField(tag);
			bool expected = true;
			bool actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Otter Author");
			subfields.Add(subfield);
			target = new DataField(tag, subfields);
			expected = false;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			string expected = "  " + FileMARC.SUBFIELD_INDICATOR.ToString() + "aIt's a book!" + FileMARC.SUBFIELD_INDICATOR.ToString() + "bAnne Author" + FileMARC.END_OF_FIELD.ToString();
			string actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			string expected = "100    [a]: It's a book!" + Environment.NewLine + "       [b]: Anne Author";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ValidateIndicator
		///</summary>
		[TestMethod()]
		public void ValidateIndicatorTest()
		{
			char ind = 'x';
			bool expected = true;
			bool actual = DataField.ValidateIndicator(ind);
			Assert.AreEqual(expected, actual);
			ind = '1';
			actual = DataField.ValidateIndicator(ind);
			Assert.AreEqual(expected, actual);
			ind = '%';
			expected = false;
			actual = DataField.ValidateIndicator(ind);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Indicator1
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Invalid indicator.")]
		public void Indicator1Test()
		{
			string tag = "100";
			DataField target = new DataField(tag);
			char expected = '1';
			target.Indicator1 = expected;
			char actual = target.Indicator1;
			Assert.AreEqual(expected, actual);
			expected = '%';
			target.Indicator1 = expected; //Will throw exception
		}

		/// <summary>
		///A test for Indicator2
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(ArgumentException), "Invalid indicator.")]
		public void Indicator2Test()
		{
			string tag = "100";
			DataField target = new DataField(tag);
			char expected = '1';
			target.Indicator2 = expected;
			char actual = target.Indicator2;
			Assert.AreEqual(expected, actual);
			expected = '%';
			target.Indicator2 = expected; //Will throw exception
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		public void ItemTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			char code = 'a';
			Subfield expected = new Subfield('a', "It's a book!");
			Subfield actual = target[code];
			Assert.AreEqual(expected.Code, actual.Code);
			Assert.AreEqual(expected.Data, actual.Data);
		}

		/// <summary>
		///A test for Subfields
		///</summary>
		[TestMethod()]
		public void SubfieldsTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			List<Subfield> expected = new List<Subfield>();
			subfield = new Subfield('a', "It's another book!");
			expected.Add(subfield);
			subfield = new Subfield('b', "Anne Otter Author");
			expected.Add(subfield);
			target.Subfields = expected;
            List<Subfield> actual = target.Subfields;

			Assert.AreEqual(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i].Code, actual[i].Code);
				Assert.AreEqual(expected[i].Data, actual[i].Data);
			}
		}

		/// <summary>
		///A test for Clone
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(NullReferenceException))]
		public void CloneTest()
		{
			string tag = "100";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('a', "It's a book!");
			subfields.Add(subfield);
			subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			Field target = new DataField(tag, subfields);
			Field actual = target.Clone();
			Assert.AreNotEqual(target, actual);

			target.Tag = "200";

			string expectedString = tag;
			string actualString = actual.Tag;
			Assert.AreEqual(expectedString, actualString);

			((DataField)target).Indicator1 = '1';
			((DataField)target).Indicator2 = '2';

			expectedString = "  ";
			actualString = ((DataField)actual).Indicator1.ToString() + ((DataField)actual).Indicator2.ToString();
			Assert.AreEqual(expectedString, actualString);

			((DataField)target)['a'].Code = 'c';
			((DataField)target)['c'].Data = "It's a NEW book!";

			expectedString = "aIt's a book!";
			actualString = ((DataField)actual)['a'].Code.ToString() + ((DataField)actual)['a'].Data;
			Assert.AreEqual(expectedString, actualString);

			//This next line will fail as there's no subfield c!
			((DataField)actual)['c'].Code = 'a';
		}

		/// <summary>
		///A test for InsertSubfield
		///</summary>
		[TestMethod()]
		public void InsertSubfieldTest()
		{
			string tag = "600";
			List<Subfield> subfields = new List<Subfield>();
			Subfield subfield = new Subfield('b', "Anne Author");
			subfields.Add(subfield);
			subfield = new Subfield('d', "Some text");
			subfields.Add(subfield);
			subfield = new Subfield('2', "Some more text");
			subfields.Add(subfield);
			subfield = new Subfield('3', "Some fancy text");
			subfields.Add(subfield);
			subfield = new Subfield('5', "Some more fancy text");
			subfields.Add(subfield);
			DataField target = new DataField(tag, subfields);
			Subfield newSubfield = new Subfield('a', "Testing an insert at the beginning!");
			target.InsertSubfield(newSubfield);
			newSubfield = new Subfield('c', "Testing an insert between letters!");
			target.InsertSubfield(newSubfield);
			newSubfield = new Subfield('e', "Testing an insert at the end of letters before numbers");
			target.InsertSubfield(newSubfield);
			newSubfield = new Subfield('1', "Testing a numbered insert before numbers!");
			target.InsertSubfield(newSubfield);
			newSubfield = new Subfield('4', "Testing a numbered insert between numbers!");
			target.InsertSubfield(newSubfield);
			newSubfield = new Subfield('6', "Testing a numbered insert after numbers!");
			target.InsertSubfield(newSubfield);

			Assert.AreEqual('a', target.Subfields[0].Code);
			Assert.AreEqual('b', target.Subfields[1].Code);
			Assert.AreEqual('c', target.Subfields[2].Code);
			Assert.AreEqual('d', target.Subfields[3].Code);
			Assert.AreEqual('e', target.Subfields[4].Code);
			Assert.AreEqual('1', target.Subfields[5].Code);
			Assert.AreEqual('2', target.Subfields[6].Code);
			Assert.AreEqual('3', target.Subfields[7].Code);
			Assert.AreEqual('4', target.Subfields[8].Code);
			Assert.AreEqual('5', target.Subfields[9].Code);
			Assert.AreEqual('6', target.Subfields[10].Code);
		}

        /// <summary>
        ///A test for ToXML
        ///</summary>
        [TestMethod()]
        public void ToXMLTest()
        {
            string tag = "100";
            List<Subfield> subfields = new List<Subfield>();
            Subfield subfield = new Subfield('a', "It's a book!");
            subfields.Add(subfield);
            subfield = new Subfield('b', "Anne Author");
            subfields.Add(subfield);
            char ind1 = '1';
            char ind2 = '0';
            DataField target = new DataField(tag, subfields, ind1, ind2);

            XElement expected = new XElement(FileMARCXML.Namespace + "datafield", new XAttribute("tag", "100"), new XAttribute("ind1", "1"), new XAttribute("ind2", "0"),
                                        new XElement(FileMARCXML.Namespace + "subfield", new XAttribute("code", "a"), "It's a book!"),
                                        new XElement(FileMARCXML.Namespace + "subfield", new XAttribute("code", "b"), "Anne Author"));

            XElement actual = target.ToXML();
            Assert.IsTrue(XNode.DeepEquals(expected, actual));
        }
	}
}
