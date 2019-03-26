using System;
using System.Threading;

namespace Common.Randomization {
	public static class RandomProvider {
		private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
			new Random(Interlocked.Increment(ref seed))
		);
		private static int seed = Environment.TickCount;

		public static Random GetThreadRandom() {
			return randomWrapper.Value;
		}
	}
}