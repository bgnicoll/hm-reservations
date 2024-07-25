using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{ 
    [TestClass]
    public class DateTimeRoundingSegmentTests
    {
        [TestMethod]
        public void RoundUpToNearestSegment_ExactSegment_ReturnsSameDateTime()
        {
            var dateTime = new DateTime(2024, 7, 25, 14, 0, 0);
            var expected = new DateTime(2024, 7, 25, 14, 0, 0);

            var result = DateTimeSegmentHelper.RoundUpToNearestSegment(dateTime);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RoundUpToNearestSegment_NotOnSegment_ReturnsRoundedUpDateTime()
        {
            var dateTime = new DateTime(2024, 7, 25, 14, 7, 30);
            var expected = new DateTime(2024, 7, 25, 14, 15, 0);

            var result = DateTimeSegmentHelper.RoundUpToNearestSegment(dateTime);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RoundDownToNearestSegment_ExactSegment_ReturnsSameDateTime()
        {
            var dateTime = new DateTime(2024, 7, 25, 14, 15, 0);
            var expected = new DateTime(2024, 7, 25, 14, 15, 0);

            var result = DateTimeSegmentHelper.RoundDownToNearestSegment(dateTime);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RoundDownToNearestSegment_NotOnSegment_ReturnsRoundedDownDateTime()
        {
            var dateTime = new DateTime(2024, 7, 25, 14, 22, 45);
            var expected = new DateTime(2024, 7, 25, 14, 15, 0);

            var result = DateTimeSegmentHelper.RoundDownToNearestSegment(dateTime);

            Assert.AreEqual(expected, result);
        }
    }
}
