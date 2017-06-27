using Microsoft.VisualStudio.TestTools.UnitTesting;
using MARC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.Tests
{
    [TestClass()]
    public class FileMARCXMLReaderTests
    {

        [TestMethod()]
        [DeploymentItem("Test Records\\manyrecords.xml")]
        public void FileMARCXMLReaderTest()
        {
            string filename = "manyrecords.xml";
            FileMARCXMLReader target = new FileMARCXMLReader(filename);
            Assert.IsInstanceOfType(target, typeof(FileMARCXMLReader));
        }

        [TestMethod()]
        [DeploymentItem("Test Records\\manyrecords.xml")]
        public void GetEnumeratorTest()
        {
            string filename = "manyrecords.xml";
            FileMARCXMLReader reader = new FileMARCXMLReader(filename);
            int target = 1000;
            int actual = 0;
            foreach (Record marc in reader)
            {
                actual++;
            }

            Assert.AreEqual(target, actual);
        }
    }
}