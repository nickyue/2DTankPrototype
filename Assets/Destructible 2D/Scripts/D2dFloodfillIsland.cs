using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public static partial class D2dFloodfill
	{
		public class Island
		{
			public List<D2dFloodfillPixel> Pixels = new List<D2dFloodfillPixel>();

			public int MinX;

			public int MinY;

			public int MaxX;

			public int MaxY;
		
			public void AddPixel(int x, int y)
			{
				var pixel = default(D2dFloodfillPixel);
			
				pixel.X = x;
				pixel.Y = y;
			
				Pixels.Add(pixel);
			}
		
			public void Clear()
			{
				Pixels.Clear();
			}

			public D2dSplitGroup CreateSplitGroup(bool feather, bool allowExpand)
			{
				if (feather == true)
				{
					//D2dFloodfill.Feather(this);
				}

				var group = D2dSplitGroup.GetSplitGroup();

				for (var j = Pixels.Count - 1; j >= 0; j--)
				{
					var pixel = Pixels[j];

					group.AddPixel(pixel.X, pixel.Y);
				}

				// Feather will automatically add a transparent border
				if (allowExpand == true && feather == false)
				{
					group.Rect.MinX -= 1;
					group.Rect.MaxX += 1;
					group.Rect.MinY -= 1;
					group.Rect.MaxY += 1;
				}

				return group;
			}
		}
	}
}