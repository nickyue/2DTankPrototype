using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		private class Line
		{
			public int y;
			public int min;
			public int max;
			public bool used;
			public List<Line> ups = new List<Line>();
			public List<Line> dns = new List<Line>();
		}
		
		public static List<Island> Islands = new List<Island>();

		public static List<Island> BorderIslands = new List<Island>();

		private static List<Line> lines = new List<Line>();

		private static List<Line> scan = new List<Line>();

		private static int lineCount;
		
		public static Island GetIsland()
		{
			var island = D2dPool<Island>.Spawn() ?? new Island();

			Islands.Add(island);

			return island;
		}

		public static void Clear()
		{
			for (var i = Islands.Count - 1; i >= 0; i--)
			{
				var island = Islands[i];

				island.Clear();

				D2dPool<Island>.Despawn(island);
			}

			for (var i = BorderIslands.Count - 1; i >= 0; i--)
			{
				var island = BorderIslands[i];

				island.Clear();

				D2dPool<Island>.Despawn(island);
			}

			Islands.Clear();
			BorderIslands.Clear();
		}

		public static void FastFind(byte[] alphaData, int alphaWidth, int alphaHeight)
		{
			FastFind(alphaData, alphaWidth, alphaHeight, new D2dRect(0, alphaWidth, 0, alphaHeight));
		}

		public static void FastFindLocal(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect rect)
		{
			FastFind(alphaData, alphaWidth, alphaHeight, rect);

			for (var i = Islands.Count - 1; i >= 0; i--)
			{
				var island = Islands[i];

				if (island.MinX == rect.MinX || island.MaxX == rect.MaxX || island.MinY == rect.MinY || island.MaxY == rect.MaxY)
				{
					Islands.RemoveAt(i);

					BorderIslands.Add(island);
				}
			}
		}

		private static void FastFind(byte[] alphaData, int alphaWidth, int alphaHeight, D2dRect rect)
		{
			Clear();

			lineCount = 0;

			var oldCount = 0;

			for (var y = rect.MinY; y < rect.MaxY; y++)
			{
				var newCount = FastFindLines(alphaData, alphaWidth, y, rect.MinX, rect.MaxX);

				FastLinkLines(lineCount - newCount - oldCount, lineCount - newCount, lineCount);

				oldCount = newCount;
			}

			for (var i = 0; i < lineCount; i++)
			{
				var line = lines[i];

				if (line.used == false)
				{
					var island = GetIsland();

					island.MinX = line.min;
					island.MaxX = line.max;
					island.MinY = line.y;
					island.MaxY = line.y + 1;

					// Scan though all connected lines and add to list
					scan.Clear(); scan.Add(line); line.used = true;

					for (var j = 0; j < scan.Count; j++)
					{
						var scanLine = scan[j];

						island.MinX = Mathf.Min(island.MinX, line.min);
						island.MaxX = Mathf.Max(island.MaxX, line.max);
						island.MinY = Mathf.Min(island.MinY, line.y    );
						island.MaxY = Mathf.Max(island.MaxY, line.y + 1);

						AddToScan(scanLine.ups);
						AddToScan(scanLine.dns);

						for (var x = scanLine.min; x < scanLine.max; x++)
						{
							island.AddPixel(x, scanLine.y);
						}
					}
				}
			}
		}

		private static void AddToScan(List<Line> lines)
		{
			for (var i = lines.Count - 1; i >= 0; i--)
			{
				var line = lines[i];

				if (line.used == false)
				{
					scan.Add(line); line.used = true;
				}
			}
		}

		private static void FastLinkLines(int min, int mid, int max)
		{
			for (var i = min; i < mid; i++)
			{
				var oldLine = lines[i];

				for (var j = mid; j < max; j++)
				{
					var newLine = lines[j];

					if (newLine.min < oldLine.max && newLine.max > oldLine.min)
					{
						oldLine.ups.Add(newLine);
						newLine.dns.Add(oldLine);
					}
				}

				oldLine.min -= 1;
				oldLine.max += 1;
			}
		}

		private static int FastFindLines(byte[] alphaData, int alphaWidth, int y, int minX, int maxX)
		{
			var line   = default(Line);
			var count  = 0;
			var offset = alphaWidth * y;

			for (var x = minX; x < maxX; x++)
			{
				if (alphaData[offset + x] > 127)
				{
					// Start new line?
					if (line == null)
					{
						line = GetLine(); count += 1;
						line.min = line.max = x;
						line.y = y;
					}

					// Expand line
					line.max += 1;
				}
				// Terminate line?
				else if (line != null)
				{
					line = null;
				}
			}

			return count;
		}

		private static Line GetLine()
		{
			var line = default(Line);

			if (lineCount >= lines.Count)
			{
				line = new Line();

				lines.Add(line);
			}
			else
			{
				line = lines[lineCount];

				line.used = false;
				line.ups.Clear();
				line.dns.Clear();
			}

			lineCount += 1;

			return line;
		}
	}
}