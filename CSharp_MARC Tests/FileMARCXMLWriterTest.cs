using MARC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSharp_MARC_Tests
{
    
    
    /// <summary>
    ///This is a test class for FileMARCXMLWriterTest and is intended
    ///to contain all FileMARCXMLWriterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileMARCXMLWriterTest
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
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Test Records\\onerecord.xml")]
        public void WriteTest()
        {
            string filename = "onerecord.xml";
            string testFilename = "onerecord_test.xml";
            string source = File.ReadAllText(filename);
            FileMARCXML targetXML = new FileMARCXML(source);
            Record record = targetXML[0];

            using (FileMARCXMLWriter target = new FileMARCXMLWriter(testFilename))
            {
                target.Write(record);
            }

            string expected = source;
            string actual = File.ReadAllText(testFilename);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Write
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Test Records\\music.xml")]
        public void WriteTest1()
        {
            string filename = "music.xml";
            string testFilename = "music_test.xml";
            string source = File.ReadAllText(filename);
            FileMARCXML targetXML = new FileMARCXML(source);
            List<Record> records = new List<Record>();

            foreach (Record record in targetXML)
            {
                records.Add(record);
            }

            using (FileMARCXMLWriter target = new FileMARCXMLWriter(testFilename))
            {
                target.Write(records);
            }

            string expected = source;
            string actual = File.ReadAllText(testFilename);
            Assert.AreEqual(expected, actual);
        }
    }
}
