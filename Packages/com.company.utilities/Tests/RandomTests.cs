using NUnit.Framework;
using UnityEngine;
using System.Linq;
using Random = Company.Utilities.Runtime.Random;

namespace Company.Utilities.Tests
{
    [TestFixture]
    public class RandomTests
    {
        private const int TestIterations = 1000;

        [Test]
        public void Range_Float_ReturnsValueWithinRange()
        {
            float min = 0.0f;
            float max = 10.0f;

            var results = new float[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().Range(min, max);
            }

            Assert.IsTrue(results.All(r => r >= min && r < max), "All float range results should be within specified bounds");
        }

        [Test]
        public void Range_Int_ReturnsValueWithinRange()
        {
            int min = 0;
            int max = 10;

            var results = new int[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().Range(min, max);
            }

            Assert.IsTrue(results.All(r => r >= min && r < max), "All int range results should be within specified bounds");
        }

        [Test]
        public void NextFloat_ReturnsValueBetweenZeroAndOne()
        {
            var results = new float[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().NextFloat();
            }

            Assert.IsTrue(results.All(r => r >= 0.0f && r < 1.0f), "All NextFloat results should be between 0 and 1");
        }

        [Test]
        public void NextDirection_ReturnsNormalizedVector()
        {
            var results = new Vector2[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().NextDirection();
            }

            Assert.IsTrue(results.All(r => Mathf.Abs(r.magnitude - 1.0f) < 0.001f), "All NextDirection results should be normalized");
        }

        [Test]
        public void InsideUnitCircle_ReturnsVectorWithinUnitCircle()
        {
            var results = new Vector2[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().InsideUnitCircle();
            }

            Assert.IsTrue(results.All(r => r.magnitude <= 1.0f), "All InsideUnitCircle results should be within unit circle");
        }

        [Test]
        public void GetRandomDirectionFromCone_WithZeroAngle_ReturnsOriginalDirection()
        {
            Vector2 linearVelocity = Vector2.right;
            float angle = 0f;

            var results = new Vector2[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().GetRandomDirectionFromCone(linearVelocity, angle);
            }

            Assert.IsTrue(results.All(r => Vector2.Distance(r, linearVelocity) < 0.001f), "All results should match original direction when angle is 0");
        }

        [Test]
        public void GetRandomDirectionFromCone_ReturnsDirectionWithinCone()
        {
            Vector2 linearVelocity = Vector2.right;
            float angle = 30f;

            var results = new Vector2[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().GetRandomDirectionFromCone(linearVelocity, angle);
            }

            Assert.IsTrue(results.All(r => {
                float angleBetween = Vector2.Angle(linearVelocity, r);
                return angleBetween <= angle / 2f;
            }), "All results should be within the specified cone angle");
            
            Assert.IsTrue(results.All(r => Mathf.Abs(r.magnitude - 1.0f) < 0.001f), "All results should be normalized");
        }

        [Test]
        public void GetRandomDirectionFromCone_ResultIsNormalized()
        {
            Vector2 linearVelocity = Vector2.right;
            float angle = 60f;

            var results = new Vector2[TestIterations];
            for (int i = 0; i < TestIterations; i++)
            {
                results[i] = new Random().GetRandomDirectionFromCone(linearVelocity, angle);
            }

            Assert.IsTrue(results.All(r => Mathf.Abs(r.magnitude - 1.0f) < 0.001f), "All results should be normalized");
        }
    }
}
