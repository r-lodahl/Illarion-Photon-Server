using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Illarion.Server.Navigation
{
    public sealed class NavigationTest
    {
        private readonly ITestOutputHelper _output;

        public NavigationTest(ITestOutputHelper output) => _output = output;

        [Trait("Category", "Navigation")]
        [Theory]
        [MemberData(nameof(LocationControlTestData))]
        public void LocationControlTest(Vector3 oldPosition, Vector3 newPosition, float deltaTime, Vector3 expectedResult)
        {
            var investigator = new LocationInvestigator();

            //BenchmarkTest(investigator.InvestigateUpdatedLocation, oldPosition, newPosition, deltaTime);


            Vector3 pathResult = investigator.InvestigateUpdatedLocation(oldPosition, newPosition, deltaTime);

            _output.WriteLine(pathResult.ToString());


            Assert.Equal(expectedResult, pathResult);
        }

        public static IEnumerable<object[]> LocationControlTestData()
        {
            yield return new object[] {new Vector3(0, 0, 0), new Vector3(1, 0, 0), 1f, new Vector3(1, 0, 0)};
            //yield return new object[] { new Vector3(0, 0, 0), new Vector3(2, 0, 0), 1f, new Vector3(1, 0, 0) };
        }

        // One Iter takes about 0.024 ms || 0.015 on PC spec
        /*private void BenchmarkTest(Func<Vector3, Vector3, float, Vector3> fun, Vector3 a, Vector3 b, float c)
        {
            int it = 10000;
            GC.Collect();
            fun.Invoke(a, b, c);
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < it; i++)
            {
                fun.Invoke(a, b, c);
            }
            stopwatch.Stop();
            _output.WriteLine((stopwatch.ElapsedMilliseconds/ (float)it).ToString() + "|||" + stopwatch.Elapsed.TotalSeconds);
        }*/
    }
}