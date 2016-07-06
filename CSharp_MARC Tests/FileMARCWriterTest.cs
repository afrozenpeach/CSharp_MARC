using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for FileMARCWriterTest and is intended
    ///to contain all FileMARCWriterTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FileMARCWriterTest
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
		///A test for Write
		///Reads in two records. One is a normal MARC21, the other is a MARC21 RDA which includes a © symbol in tag 260.
		///Read the records, write them back out to a new file, then read them back in. The first read and second read should be the same.
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\testwriter.mrc")]
		[DeploymentItem("Test Records\\tworecords_onerda.mrc")]
		public void WriteTest()
		{
			string filename = "tworecords_onerda.mrc";
			string writeFilename = "testwriter.mrc";
			string target1 = null;
			string target2 = null;
			string expected1 = null;
			string expected2 = null;
			bool first = true;
			List<Record> records = new List<Record>();

			using (FileMARCReader reader = new FileMARCReader(filename))
			{
				foreach (Record record in reader)
				{
					if (first)
					{
						first = false;
						expected1 = record.ToRaw();
					}
					else
					{
						expected2 = record.ToRaw();
					}

					records.Add(record);
				}
			}

			using (FileMARCWriter target = new FileMARCWriter(writeFilename))
			{	
				target.Write(records);
				target.WriteEnd();
			}

			Assert.IsTrue(File.Exists(filename));

			first = true;
			using (FileMARCReader reader = new FileMARCReader(writeFilename))
			{
				foreach (Record record in reader)
				{
					if (first)
					{
						first = false;
						target1 = record.ToRaw();
					}
					else
					{
						target2 = record.ToRaw();
					}
				}
			}

			Assert.AreEqual(expected1, target1);
			Assert.AreEqual(expected2, target2);
			Assert.IsTrue(expected1.Contains("©"));
		}

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\testwriter.mrc")]
		public void WriteTest1()
		{
			string filename = "testwriter.mrc";
			string expected = "01754cam  2200397 a 45000010009000000050017000090080041000269060045000679250044001129550187001560100017003430200034003600200037003940350023004310350020004540370060004740400049005340420009005830500024005920820014006161000018006302450070006482600038007183000029007565860037007855200280008226500038011026500034011406500031011746500031012056500029012366500025012656500022012906500022013126500022013341460273320110216094643.0061020s2007    nyua   c      000 f eng    a7bcbccorignewd1eecipf20gy-gencatlg0 aacquireb2 shelf copiesxpolicy default  alb18 2006-10-20ilb18 2006-10-20elb18 2006-10-20 to CIPaps04 2007-05-17 1 copy rec'd., to CIP ver.fld11 2007-07-02 Z-CipVergld11 2007-07-02 to BCCDalf27 2007-07-11 cp. 2 to BCCD  a  2006031847  a0810993139 (paper over board)  a9780810993136 (paper over board)  a(OCoLC)ocm74029165  a(OCoLC)74029165  bJunior Library Guildnhttp://www.juniorlibraryguild.com  aDLCcDLCdBAKERdBTCTAdTEFdYDXCPdEHHdDLC  alcac00aPZ7.K6232bDia 200700a[Fic]2221 aKinney, Jeff.10aDiary of a wimpy kid :bGreg Heffley's journal /cby Jeff Kinney.  aNew York :bAmulet Books,cc2007.  a217 p. :bill. ;c22 cm.8 aA Junior Library Guild selection  aGreg records his sixth grade experiences in a middle school where he and his best friend, Rowley, undersized weaklings amid boys who need to shave twice daily, hope just to survive, but when Rowley grows more popular, Greg must take drastic measures to save their friendship. 0aMiddle schoolsvJuvenile fiction. 0aFriendshipvJuvenile fiction. 0aSchoolsvJuvenile fiction. 0aDiariesvJuvenile fiction. 1aMiddle schoolsvFiction. 1aFriendshipvFiction. 1aSchoolsvFiction. 1aDiariesvFiction. 1aHumorous stories.";
			string target1 = string.Empty;

			FileMARC marc = new FileMARC(expected);
			Record record = marc[0];

			using (FileMARCWriter target = new FileMARCWriter(filename))
			{
				target.Write(record);
				target.WriteEnd();
			}

			Assert.IsTrue(File.Exists(filename));

			using (FileMARCReader reader = new FileMARCReader(filename))
			{
				foreach (Record rec in reader)
				{
					target1 = rec.ToRaw();
				}
			}

			Assert.AreEqual(expected, target1);
		}

		/// <summary>
		///A test for WriteEnd
		///</summary>
		[TestMethod()]
		[DeploymentItem("Test Records\\testwriter.mrc")]
		public void WriteEndTest()
		{
			string filename = "testwriter.mrc";
			string expected = FileMARCWriter.END_OF_FILE.ToString();
			using (FileMARCWriter target = new FileMARCWriter(filename))
			{
				target.WriteEnd();
			}

			string target1 = File.ReadAllText(filename);

			Assert.AreEqual(expected, target1);
		}
	}
}
