using System.Numerics;
using NUnit.Framework;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Tests
{
    [TestFixture]
    public class BoundsTests
    {
        struct TestData
        {
            public Bounds Bounds;
            public Vector2 Point;
            public bool ExpectedResult;
        }

        [Test]
        public void Contains_ReturnsCorrectResult()
        {
            var testData = new TestData[]
            {
                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(0, 0), ExpectedResult = true },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(10, 10), ExpectedResult = true },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(0, 0), ExpectedResult = true },

                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(6, 6), ExpectedResult = false },
                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(-6, 0), ExpectedResult = false },
                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(0, -6), ExpectedResult = false },
                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(6, 0), ExpectedResult = false },
                new TestData { Bounds = new Bounds(Vector2.Zero, new Vector2(10, 10)), Point = new Vector2(0, 6), ExpectedResult = false },

                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(11, 11), ExpectedResult = false },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(-1, 0), ExpectedResult = false },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(0, -1), ExpectedResult = false },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(12, 0), ExpectedResult = false },
                new TestData { Bounds = new Bounds(new Vector2(5, 5), new Vector2(10, 10)), Point = new Vector2(0, 12), ExpectedResult = false },
            };

            foreach (var data in testData)
            {
                Assert.AreEqual(data.ExpectedResult, data.Bounds.Contains(data.Point), $"Expected {data.ExpectedResult} for point {data.Point} in bounds {data.Bounds}");
            }
        }
    }
}