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
 * @copyright 2009-2018 Mattie Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace CSharp_MARC_Tests
{


    /// <summary>
    ///This is a test class for ControlFieldTest and is intended
    ///to contain all ControlFieldTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ControlFieldTest
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
		///A test for ControlField Constructor
		///</summary>
		[TestMethod()]
		public void ControlFieldConstructorTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);
			string expected = "001";
			string actual = target.Tag;
			Assert.AreEqual(expected, actual);
			expected = "  2007032296";
			actual = target.Data;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for FormatField
		///</summary>
		[TestMethod()]
		public void FormatFieldTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);
			string expected = "  2007032296";
			string actual = target.FormatField();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsEmpty
		///</summary>
		[TestMethod()]
		public void IsEmptyTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);
			bool expected = false;
			bool actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
			target = new ControlField(tag, string.Empty);
			expected = true;
			actual = target.IsEmpty();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);
			string expected = "  2007032296" + FileMARC.END_OF_FIELD.ToString();
			string actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);
			string expected = "001       2007032296";
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Data
		///</summary>
		[TestMethod()]
		public void DataTest()
        {
            string tag = "001";
            string data = "  2007032296";
            ControlField target = new ControlField(tag, data);
            string expected = "  2011022800";
            target.Data = expected;
            string actual = target.Data;
            Assert.AreEqual(expected, actual);
        }

		/// <summary>
		///A test for Clone
		///</summary>
		[TestMethod()]
		public void CloneTest()
		{
			string tag = "001";
			string data = "  2007032296";
			ControlField target = new ControlField(tag, data);

			Field expected = target;
			Field actual = target.Clone();
			Assert.AreNotEqual(expected, actual);

			target.Data = "  2011063096";

			string expectedString = data;
			string actualString = ((ControlField)actual).Data;
			Assert.AreEqual(expectedString, actualString);

			target.Tag = "002";

			expectedString = tag;
			actualString = ((ControlField)actual).Tag;
			Assert.AreEqual(expectedString, actualString);
		}

        /// <summary>
        ///A test for ToXML
        ///</summary>
        [TestMethod()]
        public void ToXMLTest()
        {
            string tag = "001";
            string data = "  2007032296";
            ControlField target = new ControlField(tag, data);

            XElement expected = new XElement(FileMARCXML.Namespace + "controlfield", new XAttribute("tag", "001"), "  2007032296");
            XElement actual = target.ToXML();
            Assert.IsTrue(XNode.DeepEquals(expected, actual));
        }
	}
}
