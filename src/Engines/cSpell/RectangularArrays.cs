namespace SpellChecker.Engines.cSpell.Extensions {
	internal static class RectangularArrays {
		public static int[][] ReturnRectangularIntArray(int x, int y) {
			int[][] ret = new int[x][];
			for (int i = 0; i < x; i++) {
				ret[i] = new int[y];
			}

			return ret;
		}
	}
}