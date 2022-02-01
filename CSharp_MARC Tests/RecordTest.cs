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
using System.Linq;
using System.IO;
using System.Text;

namespace CSharp_MARC_Tests
{


    /// <summary>
    ///This is a test class for RecordTest and is intended
    ///to contain all RecordTest Unit Tests
    ///</summary>
	[TestClass()]
	public class RecordTest
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
		///A test for Record Constructor
		///</summary>
		[TestMethod()]
		public void RecordConstructorTest()
		{
			Record target = new Record();

			{
				int expected = 0;
				int actual = target.Fields.Count;
				Assert.AreEqual(expected, actual);
				actual = target.Warnings.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "                        "; //24 spaces! Yes I counted! :awesome:
				string actual = target.Leader;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for GetFields
		///</summary>
		[TestMethod()]
		public void GetFieldsTest()
		{
			Record target = new Record();
			string tag = "245";
			target.Fields.Add(new ControlField("001", "I am data! (But not Data from Star Trek TNG)"));
			target.Fields.Add(new DataField(tag));
			target.Fields.Add(new DataField("521", new List<Subfield>(), ' ', '1'));
		    List<Field> expected = new List<Field> {new DataField(tag)};
		    List<Field> actual = target.GetFields(tag);
			Assert.AreEqual(expected.Count, actual.Count);
			Assert.AreEqual(expected[0].ToRaw(), actual[0].ToRaw());
		}

		/// <summary>
		///A test for InsertField
		///</summary>
		[TestMethod()]
		public void InsertFieldTest()
		{
			Record target = new Record();
			string tag = "245";
			target.Fields.Add(new ControlField("001", "I am data! (But not Data from Star Trek TNG)"));
			target.Fields.Add(new DataField(tag));
			target.Fields.Add(new DataField("521", new List<Subfield>(), ' ', '1'));
			Field newField = new DataField("300", new List<Subfield>(), '0', '1');
			target.InsertField(newField);
			Assert.AreSame(newField, target.Fields[2]);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTest()
		{
			Record target = new Record();
			target.Fields.Add(new ControlField("001", "I am data! (But not Data from Star Trek TNG)"));
			//This will be empty, and thus not written
			target.Fields.Add(new DataField("245"));
			//I broke each bit of the record out into their own strings and concatenated because it's easier to follow.
			//Leader -> Tag -> Length (+1 for EoF) -> Starts At -> EoF -> Data -> EoF -> EoR
			string expected = "00083     2200037   4500" + "001" + "0045" + "00000" + FileMARC.END_OF_FIELD + "I am data! (But not Data from Star Trek TNG)" + FileMARC.END_OF_FIELD + FileMARC.END_OF_RECORD;
			string actual = target.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToRaw
		///</summary>
		[TestMethod()]
		public void ToRawTestEncoding()
		{
			Record target = new Record();
			Encoding encoding = Encoding.UTF8;
			target.Fields.Add(new ControlField("001", "I am data! © (But not Data from Star Trek TNG)"));
			//This will be empty, and thus not written
			target.Fields.Add(new DataField("245"));
			//I broke each bit of the record out into their own strings and concatenated because it's easier to follow.
			//Leader -> Tag -> Length (+1 for EoF) -> Starts At -> EoF -> Data -> EoF -> EoR
			string expected = "00086     2200037   4500" + "001" + "0048" + "00000" + FileMARC.END_OF_FIELD + "I am data! © (But not Data from Star Trek TNG)" + FileMARC.END_OF_FIELD + FileMARC.END_OF_RECORD;
			string actual = target.ToRaw(encoding);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			Record target = new Record();
			target.Fields.Add(new ControlField("001", "I am data! (But not Data from Star Trek TNG)"));
			//This will be empty, and thus not written
			target.Fields.Add(new DataField("245"));
			string expected = "LDR 00083     2200037   4500" + Environment.NewLine +
							  "001" + "     " + "I am data! (But not Data from Star Trek TNG)" + Environment.NewLine;
			string actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Fields
		///</summary>
		[TestMethod()]
		public void FieldsTest()
		{
			Record target = new Record();
			ControlField controlField = new ControlField("001", "I am data! (But not Data from Star Trek TNG)");
			DataField dataField = new DataField("245");
			DataField dataField2 = new DataField("521", new List<Subfield>(), ' ', '1');
			target.Fields.Add(controlField);
			target.Fields.Add(dataField);
			target.Fields.Add(dataField2);

			{
				int expected = 3;
				int actual = target.Fields.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				Field expected = controlField;
				Field actual = target.Fields[0];
				Assert.AreSame(expected, actual);
				expected = dataField;
				actual = target.Fields[1];
				Assert.AreSame(expected, actual);
				expected = dataField2;
				actual = target.Fields[2];
				Assert.AreSame(expected, actual);
			}

			{
			    List<Field> expected = new List<Field> {controlField, dataField, dataField2};
			    target.Fields = expected;
				List<Field> actual = target.Fields;
				Assert.AreSame(expected, actual);
			}
		}

		/// <summary>
		///A test for [] overload
		///</summary>
		[TestMethod()]
		public void ItemTest()
		{
			Record target = new Record();
			string tag = "245";
			ControlField controlField = new ControlField("001", "I am data! (But not Data from Star Trek TNG)");
			DataField dataField = new DataField(tag);
			DataField dataField2 = new DataField(tag, new List<Subfield>(), ' ', '1');
			target.Fields.Add(controlField);
			target.Fields.Add(dataField);
			target.Fields.Add(dataField2);
			Field expected = dataField;
			Field actual = target[tag];
		    Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Leader
		///</summary>
		[TestMethod()]
		public void LeaderTest()
		{
			Record target = new Record();
			string expected = "Take me to your leader!";
			target.Leader = expected;
			string actual = target.Leader;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Warnings
		///</summary>
		[TestMethod()]
		public void WarningsTest()
		{
			Record target = new Record();
			string expected = "IT'S ALL BROKEN! SOUND THE ALARM!";
			target.AddWarnings("IT'S ALL BROKEN! SOUND THE ALARM!");
			List<string> actual = target.Warnings;
			Assert.AreEqual(expected, actual[0]);
		}

		/// <summary>
		///A test for Clone
		///</summary>
		[TestMethod()]
		public void CloneTest()
		{
			Record target = new Record();
			Record expected = target;
			Record actual = target.Clone();
			Assert.AreNotEqual(expected, actual);

			target.Leader = "Take me to your leader!";

			string expectedString = string.Empty.PadRight(FileMARC.LEADER_LEN);
			string actualString = actual.Leader;
			Assert.AreEqual(expectedString, actualString);

			target.Warnings.Add("Testing!");

			int expectedCount = 0;
			int actualCount = actual.Warnings.Count;
			Assert.AreEqual(expectedCount, actualCount);

			target.Fields.Add(new ControlField("001", "Test Data"));

			expectedCount = 0;
			actualCount = actual.Fields.Count;
			Assert.AreEqual(expectedCount, actualCount);
		}

        /// <summary>
        ///A test for ToXML
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Test Records\\sandburg.xml")]
        public void ToXMLTest()
        {
            string source = File.ReadAllText("sandburg.xml");
            FileMARCXML targetXML = new FileMARCXML(source);
            Record target = targetXML[0];

            XDocument xdoc = XDocument.Parse(source);
            XElement expected = xdoc.Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record");
            XElement actual = target.ToXML();
            Assert.IsTrue(XNode.DeepEquals(expected, actual));
        }

        /// <summary>
        ///A test for ToXMLDocument
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Test Records\\onerecord.xml")]
        public void ToXMLDocumentTest()
        {
            string source = File.ReadAllText("onerecord.xml");
            FileMARCXML targetXML = new FileMARCXML(source);
            Record target = targetXML[0];

            string expected = source;
            XDocument xdoc = target.ToXMLDocument();
            using (StringWriter writer = new Utf8StringWriter())
            {
                xdoc.Save(writer);
                string actual = writer.ToString();
                Assert.AreEqual(expected, actual);
            }
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
	}
}
