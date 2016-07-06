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
 * @copyright 2009-2016 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 */

using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using System.IO;

namespace CSharp_MARC_Tests
{


    /// <summary>
    ///This is a test class for FileMARCXMLTest and is intended
    ///to contain all FileMARCXMLTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileMARCXMLTest
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
		///A test for FileMARCXML Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCXMLConstructorTest()
		{
			FileMARCXML target = new FileMARCXML();
			int expected = 0;
			int actual = target.RawSource.Count;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for FileMARCXML Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void FileMARCXMLConstructorTest1()
		{
			string source = File.ReadAllText("sandburg.xml");
			FileMARCXML target = new FileMARCXML(source);

			{
				int expected = 1;
				int actual = target.RawSource.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = XDocument.Parse(source).Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
				string actual = target.RawSource[0].ToString();
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for FileMARCXML Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void FileMARCXMLConstructorTest2()
		{
			XDocument source = XDocument.Load("sandburg.xml");
			FileMARCXML target = new FileMARCXML(source);

			{
				int expected = 1;
				int actual = target.RawSource.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = source.Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
				string actual = target.RawSource[0].ToString();
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void AddTest()
		{
			FileMARCXML target = new FileMARCXML();
			XDocument source = XDocument.Load("sandburg.xml");

			{
				int expected = 1;
				int actual = target.Add(source);
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = source.Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
				string actual = target.RawSource[0].ToString();
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void AddTest1()
		{
			FileMARCXML target = new FileMARCXML();
			string source = File.ReadAllText("sandburg.xml");

			{
				int expected = 1;
				int actual = target.Add(source);
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = XDocument.Parse(source).Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
				string actual = target.RawSource[0].ToString();
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for GetEnumerator
		///</summary>
		[TestMethod()]
		public void GetEnumeratorTest()
		{
			FileMARCXML target = new FileMARCXML();
			IEnumerator actual = target.GetEnumerator();
			Assert.IsInstanceOfType(actual, typeof(IEnumerator));
		}

		/// <summary>
		///A test for ImportMARCXML
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void ImportMARCXMLTest()
		{
			FileMARCXML target = new FileMARCXML();
			string file = "sandburg.xml";
			target.ImportMARCXML(file);

			{
				int expected = 1;
				int actual = target.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = XDocument.Load("sandburg.xml").Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
				string actual = target.RawSource[0].ToString();
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for MoveNext
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		[DeploymentItem("Test Records\\bigarchive.xml")]
		public void MoveNextTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			target.ImportMARCXML("bigarchive.xml");
			bool expected = true;
			bool actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
			actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
			expected = false;
			actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Reset
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		[DeploymentItem("Test Records\\bigarchive.xml")]
		public void ResetTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			target.ImportMARCXML("bigarchive.xml");
			target.MoveNext();
			target.MoveNext();
			target.MoveNext();
			target.Reset();
			target.MoveNext();
			bool expected = true;
			bool actual = target.MoveNext();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Validate
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		[DeploymentItem("Test Records\\bad_example.xml")]
		public void ValidateTest()
		{
			{
				XDocument source = XDocument.Load("sandburg.xml");
				List<string> errors = FileMARCXML.Validate(source);
				int expected = 0;
				int actual = errors.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				XDocument source = XDocument.Load("bad_example.xml");
				List<string> errors = FileMARCXML.Validate(source);
				int expected = 4;
				int actual = errors.Count;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for decode
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void decodeTest()
		{
			FileMARCXML_Accessor target = new FileMARCXML_Accessor();
			target.ImportMARCXML("sandburg.xml");
			int index = 0;
			Record decoded = target.decode(index);

			{
				string expected = "01142cam  2200301 a 4500";
				string actual = decoded.Leader;
				Assert.AreEqual(expected, actual);
			}

			{
				int expected = 23;
				int actual = decoded.Fields.Count();
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "   92005291 ";
				string actual = ((ControlField)decoded["001"]).Data;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = "Arithmetic /";
				string actual = ((DataField)decoded["245"])['a'].Data;
				Assert.AreEqual(expected, actual);
			}

			{
				char expected = '1';
				char actual = ((DataField)decoded["245"]).Indicator1;
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for Count
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		[DeploymentItem("Test Records\\bigarchive.xml")]
		public void CountTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			target.ImportMARCXML("bigarchive.xml");
			int expected = 2;
			int actual = target.Count;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Current
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			string expected = "01142cam  2200301 a 4500001001300000003000400013005001700017008004100034010001700075020002500092040001800117042000900135050002600144082001600170100003200186245008600218250001200304260005200316300004900368500004000417520022800457650003300685650003300718650002400751650002100775650002300796700002100819   92005291 DLC19930521155141.9920219s1993    caua   j      000 0 eng    a   92005291   a0152038655 :c$15.95  aDLCcDLCdDLC  alcac00aPS3537.A618bA88 199300a811/.522201 aSandburg, Carl,d1878-1967.10aArithmetic /cCarl Sandburg ; illustrated as an anamorphic adventure by Ted Rand.  a1st ed.  aSan Diego :bHarcourt Brace Jovanovich,cc1993.  a1 v. (unpaged) :bill. (some col.) ;c26 cm.  aOne Mylar sheet included in pocket.  aA poem about numbers and their characteristics. Features anamorphic, or distorted, drawings which can be restored to normal by viewing from a particular angle or by viewing the image's reflection in the provided Mylar cone. 0aArithmeticxJuvenile poetry. 0aChildren's poetry, American. 1aArithmeticxPoetry. 1aAmerican poetry. 1aVisual perception.1 aRand, Ted,eill.";
			target.MoveNext();
			object current = target.Current;
			string actual = ((Record)current).ToRaw();
			Assert.AreEqual(expected, actual);
			target.Reset();
			current = target.Current; //This will throw an exception
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void ItemTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			int index = 0;
			Record recordAtIndex = target[index];
			string expected = "01142cam  2200301 a 4500001001300000003000400013005001700017008004100034010001700075020002500092040001800117042000900135050002600144082001600170100003200186245008600218250001200304260005200316300004900368500004000417520022800457650003300685650003300718650002400751650002100775650002300796700002100819   92005291 DLC19930521155141.9920219s1993    caua   j      000 0 eng    a   92005291   a0152038655 :c$15.95  aDLCcDLCdDLC  alcac00aPS3537.A618bA88 199300a811/.522201 aSandburg, Carl,d1878-1967.10aArithmetic /cCarl Sandburg ; illustrated as an anamorphic adventure by Ted Rand.  a1st ed.  aSan Diego :bHarcourt Brace Jovanovich,cc1993.  a1 v. (unpaged) :bill. (some col.) ;c26 cm.  aOne Mylar sheet included in pocket.  aA poem about numbers and their characteristics. Features anamorphic, or distorted, drawings which can be restored to normal by viewing from a particular angle or by viewing the image's reflection in the provided Mylar cone. 0aArithmeticxJuvenile poetry. 0aChildren's poetry, American. 1aArithmeticxPoetry. 1aAmerican poetry. 1aVisual perception.1 aRand, Ted,eill.";
			string actual = recordAtIndex.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for RawSource
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\sandburg.xml")]
		public void RawSourceTest()
		{
			FileMARCXML target = new FileMARCXML();
			target.ImportMARCXML("sandburg.xml");
			string expected = XDocument.Load("sandburg.xml").Elements().First(e => e.Name.LocalName == "collection").Elements().First(e => e.Name.LocalName == "record").ToString();
			string actual = target.RawSource[0].ToString();
			Assert.AreEqual(expected, actual);
		}
	}
}
