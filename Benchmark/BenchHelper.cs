
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Victor;

namespace VictorBenchmarks;

class VictorRenderTest
{
	private readonly VictorSDK _sdk;
	private readonly List<double> _insertTimes = new();
	private readonly Stopwatch _sw = new();

	internal VictorRenderTest(VictorSDK sdk) => _sdk = sdk;
	

	internal void InsertMany(int count, int dims)
	{
		for (int i = 0; i < count; i++)
		{
			var vec = RandomVector(dims);
			_sw.Restart();
			_sdk.Insert((ulong)(i + 1), vec, (ushort)dims);
			_sw.Stop();
			_insertTimes.Add(_sw.Elapsed.TotalMilliseconds);
		}
	}

	internal BenchmarkResult GetStats()
	{
		return new BenchmarkResult
		{
			Count = _insertTimes.Count,
			Total = _insertTimes.Sum(),
			Min = _insertTimes.Min(),
			Max = _insertTimes.Max(),
			Avg = _insertTimes.Average()
		};
	}

	internal record BenchmarkResult
	{
		public int Count { get; init; }
		public double Total { get; init; }
		public double Min { get; init; }
		public double Max { get; init; }
		public double Avg { get; init; }

		public override string ToString() =>
			$"Total: {Total:F2}ms | Avg: {Avg:F2}ms | Min: {Min:F2}ms | Max: {Max:F2}ms";
	}

	private static float[] RandomVector(int dims)
	{
		var rand = new Random();
		return Enumerable.Range(0, dims).Select(_ => (float)rand.NextDouble()).ToArray();
	}
}
