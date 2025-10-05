using NUnit.Framework;
using System.Numerics;
using Company.Utilities.Runtime;

namespace Company.Utilities.Tests
{
    [TestFixture]
    public class NumericsExtensionsTests
    {
        struct TestData
        {
            public Vector2 Vector;
            public Vector2 Other;
            public float ExpectedAngle;
        }

        [Test]
        public void Angle_ReturnsCorrectAngle()
        {   
            var testData = new TestData[]
            {
                new TestData { Vector = new Vector2(1, 0), Other = new Vector2(0, 1), ExpectedAngle = 90 },
                new TestData { Vector = new Vector2(0, 1), Other = new Vector2(1, 0), ExpectedAngle = 90 },
                new TestData { Vector = new Vector2(1, 1), Other = new Vector2(0, 1), ExpectedAngle = 45 },
                new TestData { Vector = new Vector2(0, 1), Other = new Vector2(1, 1), ExpectedAngle = 45 },
            };

            foreach (var data in testData)
            {
                var angle = NumericsExtensions.Angle(data.Vector, data.Other);
                Assert.AreEqual(data.ExpectedAngle, angle);
            }
        }
    }
}