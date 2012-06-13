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
 * @copyright 2009-2012 Matt Schraeder and Bound to Stay Bound Books <http://www.btsb.com>
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
			int expected = 0;
			int actual;
			actual = target.RawSource.Count;
			Assert.AreEqual(expected, actual);

		}

		/// <summary>
		///A test for FileMARC Constructor
		///</summary>
		[TestMethod()]
		public void FileMARCConstructorTest1()
		{
			//This string is taken as a copy and paste from record.mrc in the Test Records directory.
			string source = "01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
			FileMARC target = new FileMARC(source);

			{
				int expected = 1;
				int actual;
				actual = target.RawSource.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = source;
				string actual;
				actual = target.RawSource[0];
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for Add
		///</summary>
		[TestMethod()]
		public void AddTest()
		{
			FileMARC target = new FileMARC();
			//This string is taken as a copy and paste from record.mrc in the Test Records directory.
			string source = "01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
			target.Add(source);

			{
				int expected = 1;
				int actual;
				actual = target.RawSource.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				string expected = source;
				string actual;
				actual = target.RawSource[0];
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for CleanSource
		///</summary>
		[TestMethod()]
		[DeploymentItem("CSharp_MARC.dll")]
		public void CleanSourceTest()
		{
			FileMARC_Accessor target = new FileMARC_Accessor();
			string source = "\x0a";
			string expected = string.Empty;
			string actual;
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
			source = "\x0d";
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
			source = "\x00";
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
			source = "\x0a\x0d\x0a\x0d\x00\x00\x0a\x0d\x0a\x0d\x00\x00\x0a\x0d\x00";
			actual = target.CleanSource(source);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetEnumerator
		///</summary>
		[TestMethod()]
		public void GetEnumeratorTest()
		{
			FileMARC target = new FileMARC();
			IEnumerator actual;
			actual = target.GetEnumerator();
			Assert.IsInstanceOfType(actual, typeof(IEnumerator));
		}

		/// <summary>
		///A test for ImportMARC
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		public void ImportMARCTest()
		{
			FileMARC target = new FileMARC();
			string file = "record.mrc";
			target.ImportMARC(file);

			{
				int expected = 1;
				int actual;
				actual = target.RawSource.Count;
				Assert.AreEqual(expected, actual);
			}

			{
				//This string is taken as a copy and paste from record.mrc in the Test Records directory.
				string source = "01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
				string expected = source;
				string actual;
				actual = target.RawSource[0];
				Assert.AreEqual(expected, actual);
			}
		}

		/// <summary>
		///A test for MoveNext
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		[DeploymentItem("Test Records\\record2.mrc")]
		public void MoveNextTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			target.ImportMARC("record2.mrc");
			bool expected = true;
			bool actual;
			actual = target.MoveNext();
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
		[DeploymentItem("Test Records\\record.mrc")]
		[DeploymentItem("Test Records\\record2.mrc")]
		public void ResetTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			target.ImportMARC("record2.mrc");
			target.MoveNext();
			target.MoveNext();
			target.MoveNext();
			target.Reset();
			target.MoveNext();
			bool expected = true;
			bool actual;
			actual = target.MoveNext();
			Assert.AreEqual(expected,  actual);
		}

		/// <summary>
		///A test for Count
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		[DeploymentItem("Test Records\\record2.mrc")]
		public void CountTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			target.ImportMARC("record2.mrc");
			int expected = 2;
			int actual;
			actual = target.Count;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for Current
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CurrentTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			//This string is taken as a copy and paste from record.mrc in the Test Records directory.
			string expected = "01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
			string actual;
			target.MoveNext();
			object current = target.Current;
			actual = ((Record)current).ToRaw();
			Assert.AreEqual(expected, actual);
			target.Reset();
			current = target.Current; //This will throw an exception
		}

		/// <summary>
		///A test for Item
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		public void ItemTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			int index = 0;
			Record recordAtIndex;
			recordAtIndex = target[index];
			//This string is taken as a copy and paste from record.mrc in the Test Records directory.
			string expected = "01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
			string actual;
			actual = recordAtIndex.ToRaw();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for RawSource
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\record.mrc")]
		[DeploymentItem("Test Records\\record2.mrc")]
		public void RawSourceTest()
		{
			FileMARC target = new FileMARC();
			target.ImportMARC("record.mrc");
			target.ImportMARC("record2.mrc");
			List<string> expected = new List<string>();
			//This string is taken as a copy and paste from record.mrc in the Test Records directory.
			expected.Add("01754cam a2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.");
			//This string is taken as a copy and paste from record2.mrc in the Test Records directory.
			expected.Add("02265cam a2200493 a 45000010009000000050017000090080041000269060045000679250044001129550266001560100017004220200027004390200030004660200025004960200022005210350024005430350021005670370060005880400074006480420009007220500023007310820014007541000018007682450061007862460018008472600037008653000029009024900030009315860037009615200247009986500038012456500031012836500032013146500031013466500029013776500022014066500026014286500022014546500022014766550030014988000046015288560091015748560106016651499856020100927211213.0070907s2008    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb24 2007-09-07clb24 2007-09-07dlb07 2007-09-13 ret. to CIP for complete textdlb07 2007-10-11elb07 2007-10-11 to CIPaps09 2008-07-29 2 copies rec'd., to CIP ver.fld11 2008-07-30 Z-CipVergld11 2008-07-30 copy 1 & 2 to BCCDexc09 2010-09-27 modified call #  a  2007032296  a0810994739 (hardcover)  a9780810994737 (hardcover)  a9780810995529 (pbk.)  a0810995522 (pbk.)  a(OCoLC)ocn166872907  a(OCoLC)166872907  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBTCTAdBAKERdUPZdYDXCPdDAJdTEFdXY4dEHHdGK8dJEDdDLC  alcac00aPZ7.K6232bDk 200800a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bRodrick rules /cby Jeff Kinney.30aRodrick rules  aNew York :bAmulet Books,c2008.  a216 p. :bill. ;c21 cm.1 aDiary of a wimpy kid ;v28 aA Junior Library Guild selection  aGreg Heffley tells about his summer vacation and his attempts to steer clear of trouble when he returns to middle school and tries to keep his older brother Rodrick from telling everyone about Greg's most humiliating experience of the summer. 0aMiddle schoolsvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aFamiliesvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aSchoolsvFiction. 1aFamily lifevFiction. 1aDiariesvFiction. 1aHumorous stories. 7aChildren's stories.2lcsh1 aKinney, Jeff.tDiary of a wimpy kid ;v2.423Publisher descriptionuhttp://www.loc.gov/catdir/enhancements/fy0801/2007032296-d.html423Contributor biographical informationuhttp://www.loc.gov/catdir/enhancements/fy0807/2007032296-b.html");
			List<string> actual;
			actual = target.RawSource;
			Assert.AreEqual(expected.Count, actual.Count);
			Assert.AreEqual(expected[0], actual[0]);
			Assert.AreEqual(expected[1], actual[1]);
		}
	}
}
