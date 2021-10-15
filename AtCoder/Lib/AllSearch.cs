using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCoder.Lib
{
	class AllSearch
	{
		public static IEnumerable<(int x1, int x2, int y1, int y2)> PairAllSearch(int h, int w)
		{
			for (int y1 = 0; y1 < h; y1++)
				for (int y2 = y1; y2 < h; y2++)
					for (int x1 = 0; x1 < w; x1++)
						for (int x2 = x1; x2 < w; x2++)
							yield return (x1, x2, y1, y2);
		}
	}
}
