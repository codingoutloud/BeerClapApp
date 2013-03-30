using System;

namespace clap1
{
	public class MLBStadium
	{
			public string Team { get; private set; }
			private double Latitude { get; set; }
			private double Longitude { get; set; }
			public string Stadium { get; private set; }
			public double Distance { get; private set; }
			
			public MLBStadium(string input)
			{
				string[] parsed;
				parsed = input.Split(',');
				Team = parsed[0];
				Latitude = Double.Parse(parsed[1]);
				Longitude = Double.Parse(parsed[2]);
				Stadium = parsed[3];
			}
			
			// http://www.vcskicks.com/code-snippet/distance-formula.php
			public double DistanceToMe(double myLat, double myLong)
			{
				//pythagorean theorem c^2 = a^2 + b^2
				//thus c = square root(a^2 + b^2)
				double a = (Latitude - myLat);
				double b = (Longitude - myLong);
				Distance = Math.Sqrt(a * a + b * b);
				return Distance;
			}
		}
}

