using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataExtractor.Services;
using System.Linq;

namespace DataExtractor.Tests
{
    [TestClass]
    public class DataProcessorTests
    {
        private readonly IDataProcessorService _dataProcessorService;

        public DataProcessorTests()
        {
            _dataProcessorService = new DataProcessorService();
        }

        [TestMethod]
        public void ProcessData_WithStandardNameNHSPairs_ShouldExtractCorrectly()
        {
            // Arrange
            string firstFormat = "Name:John Smith NHS Number:123456";
            string secondFormat = @"[]";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John Smith", result[0].Name);
            Assert.AreEqual("123456", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessFirstFormat_WithStandardNameNHSPairs_ShouldExtractCorrectly()
        {
            // Arrange
            string firstFormat = "Name:John Smith NHS Number:123456";
            string secondFormat = @"[]";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John Smith", result[0].Name);
            Assert.AreEqual("123456", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessFirstFormat_WithNHSNumberFollowedByName_ShouldExtractCorrectly()
        {
            // Arrange
            string firstFormat = "NHS Number:999 James McDonald";
            string secondFormat = @"[]";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("James McDonald", result[0].Name);
            Assert.AreEqual("999", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessFirstFormat_WithLoneNHSNumber_ShouldMarkAsUnknown()
        {
            // Arrange
            string firstFormat = "NHS Number:444";
            string secondFormat = @"[]";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Unknown", result[0].Name);
            Assert.AreEqual("444", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessFirstFormat_WithMultipleFormats_ShouldExtractAll()
        {
            // Arrange
            string firstFormat = "Name:John Smith NHS Number:123456[[new-line]]NHS Number:999 James McDonald[[new-line]]NHS Number:444";
            string secondFormat = @"[]";
            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(p => p.Name == "John Smith" && p.NHSNumber == "123456"));
            Assert.IsTrue(result.Any(p => p.Name == "James McDonald" && p.NHSNumber == "999"));
            Assert.IsTrue(result.Any(p => p.Name == "Unknown" && p.NHSNumber == "444"));
        }

        [TestMethod]
        public void ProcessFirstFormat_WithNonNumericNHSNumber_ShouldCleanNumber()
        {
            // Arrange
            string firstFormat = "Name:David Bridge NHS Number:a44t55";
            string secondFormat = @"[]";
    
            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("David Bridge", result[0].Name);
            Assert.AreEqual("4455", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessSecondFormat_WithValidJSON_ShouldExtractCorrectly()
        {
            // Arrange
            string firstFormat = "";
            string secondFormat = @"[{""Name"":""James Jamerson"",""NHSNumber"":12345},{""Name"":""Bob Sinclair"",""NHSNumber"":5555}]";
            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(p => p.Name == "James Jamerson" && p.NHSNumber == "12345"));
            Assert.IsTrue(result.Any(p => p.Name == "Bob Sinclair" && p.NHSNumber == "5555"));
        }

        [TestMethod]
        public void ProcessSecondFormat_WithMalformedJSON_ShouldReturnEmptyList()
        {
            // Arrange
            string firstFormat = "";
            string secondFormat = "invalid json";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ProcessBothFormats_WithDuplicateNHSNumbers_ShouldRemoveDuplicates()
        {
            // Arrange
            string firstFormat = "Name:John Smith NHS Number:12345";
            string secondFormat = @"[{""Name"":""Different Name"",""NHSNumber"":12345}]";

            // Act
        // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("12345", result[0].NHSNumber);
        }

        [TestMethod]
        public void ProcessFirstFormat_WithVariousNewLineFormats_ShouldHandleCorrectly()
        {
            // Arrange
            string firstFormat = "Name:John Smith NHS Number:123[[new-line]]Name:Jane Doe NHS Number:456[[New-Line]]Name:Bob Jones NHS Number:789[[NEW-LINE]]";
            string secondFormat = @"[]";
            // Act
           var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(p => p.Name == "John Smith" && p.NHSNumber == "123"));
            Assert.IsTrue(result.Any(p => p.Name == "Jane Doe" && p.NHSNumber == "456"));
            Assert.IsTrue(result.Any(p => p.Name == "Bob Jones" && p.NHSNumber == "789"));
        }

        [TestMethod]
        public void ProcessFirstFormat_WithExtraWhitespace_ShouldTrimCorrectly()
        {
            // Arrange
            string firstFormat = "Name:   John Smith    NHS Number:   123456   ";
            string secondFormat = @"[]";

            // Act
            var result = _dataProcessorService.ProcessData(firstFormat, secondFormat).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("John Smith", result[0].Name);
            Assert.AreEqual("123456", result[0].NHSNumber);
        }
    }
}