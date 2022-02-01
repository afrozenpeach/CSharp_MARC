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

namespace CSharp_MARC_Tests
{


    /// <summary>
    ///This is a test class for FileMARCReaderTest and is intended
    ///to contain all FileMARCReaderTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileMARCReaderTest
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
		///A test for FileMARCReader Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\manyrecords.mrc")]
		public void FileMARCReaderConstructorTest()
		{
			string filename = "manyrecords.mrc";
			FileMARCReader target = new FileMARCReader(filename);
			Assert.IsInstanceOfType(target, typeof(FileMARCReader));
		}

		/// <summary>
		///A test for GetEnumerator
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\manyrecords.mrc")]
		public void GetEnumeratorTest()
		{
			string filename = "manyrecords.mrc";
			FileMARCReader reader = new FileMARCReader(filename);
			int target = 100;
			int actual = 0;
			foreach (Record marc in reader)
			{
				actual++;
			}

			Assert.AreEqual(target, actual);
		}

		[TestMethod()]
		[DeploymentItem("Test Records\\007-tit.mrc")]
		public void UTF8Multibytetest()
		{
			string filename = "007-tit.mrc";
			FileMARCReader reader = new FileMARCReader(filename, true);
			int target = 4;
			int actual = 0;

			int targetWarnings = 1;
			int actualWarnings = 0;

			foreach (Record marc in reader)
            {
				actual++;

				actualWarnings += marc.Warnings.Count;
            }

			Assert.AreEqual(target, actual);
			Assert.AreEqual(targetWarnings, actualWarnings);
		}
	}
}
