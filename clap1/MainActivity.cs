using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Locations;
using System.Text;
using System.Collections.Generic;
using Android.Media;

// http://api.espn.com/v1/sports/baseball/mlb/events/320328111?apikey=ynjqg72rvk8sbm2k8qtrdkqs

namespace clap1
{
	[Activity (Label = "clap1", MainLauncher = true)]
	public class Activity1 : Activity, ISensorEventListener, ILocationListener 
	{
		int count = 1;
		private SensorManager _sensorManager;
		private TextView _sensorTextView;
		private static readonly object _syncLock = new object();
		private string stadiumLocations = " Los Angeles Dodgers,33.800327,-117.882596, Angel Stadium of Anaheim| Los Angeles Angels,39.051494,-94.480196, Kauffman Stadium| Texas Rangers,32.751113,-97.082175, Rangers Ballpark in Arlington| San Francisco Giants,37.778443,-122.389108, AT&T Park| Kansas City Royals,29.757716,-95.355452, Minute Maid Park| Milwaukee Brewers,43.028315,-87.97114, Miller Park| Colorado Rockies,39.756265,-104.994129, Coors Field| Houston Astros,25.957781,-80.238884, Sun Life Stadium| Toronto Blue Jays,43.64159,-79.388722, Rogers Centre| St. Louis Cardinals,38.622709,-90.192669, Busch Stadium| New York Mets,40.757121,-73.845743, Citi Field| Chicago White Sox,41.83002,-87.63401, U.S. Cellular Field| San Diego Padres,32.707509,-117.157045, PETCO Park| Detroit Tigers,42.338928,-83.048697, Comerica Park| Pittsburgh Pirates,40.446804,-80.005469, PNC Park| Cincinnati Reds,39.097048,-84.5063, Great American Ball Park| Minnesota Twins,44.982172,-93.277475, Target Field| Chicago Cubs,41.948152,-87.655678, Wrigley Field| Seattle Mariners,47.591504,-122.33212, Safeco Field| Arizona Diamondbacks,33.44529,-112.066665, Chase Field| Atlanta Braves,33.735503,-84.389351, Turner Field| Baltimore Orioles,39.284175,-76.621432, Oriole Park at Camden Yards| Cleveland Indians,41.496076,-81.685437, Progressive Field| Miami Marlins,34.07252,-118.24077, Dodger Stadium| Oakland Athletics,37.751676,-122.200641, Oakland-Alameda County Coliseum| Tampa Bay Rays,27.767925,-82.653529, Tropicana Field| Washington Nationals,38.872901,-77.007404, Nationals Park| Philadelphia Phillies,39.906062,-75.166475, Citizens Bank Park| Boston Red Sox,42.346596,-71.097052, Fenway Park| New York Yankees,40.82933,-73.92636, Yankee Stadium";
		
		LocationManager _locMgr;
		private List<MLBStadium> stadiums;
		Location currentLocation;

		string DEFAULT_STADIUM = "[Fenway Park]";


#if true // GPS
		private string GetClosest()
		{
			if (currentLocation != null)
			{
				MLBStadium closestStadium = null;
				double evaluatedDistance;
				foreach (MLBStadium curr in stadiums)
				{
					evaluatedDistance = curr.DistanceToMe (currentLocation.Latitude, currentLocation.Longitude);
					if (closestStadium == null || evaluatedDistance < closestStadium.Distance)
					{
						closestStadium = curr;
					}
				}
				
				return closestStadium.Stadium;
			}
			return DEFAULT_STADIUM;
		}

		public void OnLocationChanged (Location location)
		{
			currentLocation = location;
		}
		
		public void OnProviderDisabled (string provider)
		{
//			throw new NotImplementedException ();
		}
		
		public void OnProviderEnabled (string provider)
		{
//			throw new NotImplementedException ();
		}
		
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
//			throw new NotImplementedException ();
		}
#endif

#if true // audio
		MediaPlayer _mediaPlayer = null;
		
		void Clap(int clapFileNumber)
		{
			switch (clapFileNumber)
			{
			case 1:
				_mediaPlayer = MediaPlayer.Create(this, Resource.Raw.clap1);
				break;
			case 2:
			default:
				_mediaPlayer = MediaPlayer.Create(this, Resource.Raw.clap2);
				break;
			case 3:
				_mediaPlayer = MediaPlayer.Create(this, Resource.Raw.clap3);
				break;

			}

//			int plays = (int)(duration.TotalSeconds + 0.5);
//			plays = Math.Min (10, plays);
//			plays = Math.Max (1, plays);
//			for (int i=0; i<plays; i++)
			{
				_mediaPlayer.Start();
//				System.Threading.Thread.Sleep(TimeSpan.FromSeconds (1));
			}

			if (String.IsNullOrEmpty(_stadium) || _stadium == DEFAULT_STADIUM) 
			{
				_stadium = GetClosest();
			}
		}
#endif

#if true // accelerometer
		public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
		{
			// We don't want to do anything here.
		}

		public void OnSensorChanged(SensorEvent e)
		{
			lock (_syncLock)
			{
				try
				{
					var text = new StringBuilder("x = ")
						.Append(e.Values[0])
							.Append(", y=")
							.Append(e.Values[1])
							.Append(", z=")
							.Append(e.Values[2]);
					_sensorTextView.Text = text.ToString();
					
					var x = e.Values[0];
					if (x < -6) Clap(1);
					if (x > 6) Clap(2);
				}
				catch (Exception ex)
				{
					_sensorTextView.Text = ex.Message;
				}
			}
		}
#endif

		protected override void OnPause()
		{
			base.OnPause();
			_sensorManager.UnregisterListener(this);
			_locMgr.RemoveUpdates (this);
		}

		// This needs ACCESS_FINE_LOCATION Android permissions set somehow
		protected override void OnResume()
		{
			base.OnResume();
			_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
			_locMgr.RequestLocationUpdates (
				LocationManager.GpsProvider, 2000, 1, this);
			_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
		}

		string _stadium = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

#if true // GPS
			_locMgr = GetSystemService (Context.LocationService) as LocationManager;
			
			stadiums = new List<MLBStadium>();
			string[] stadiumArray = stadiumLocations.Split ('|');
			foreach (string currStadium in stadiumArray)
			{
				stadiums.Add(new MLBStadium(currStadium));
			}
			_stadium = GetClosest();
			//button.Click += delegate {
			//	button.Text = GetClosest();
			//};
#endif

#if true
//			_mediaPlayer = MediaPlayer.Create(this, Resource.Raw.clap1);
#endif
			
#if true // Accelerometer
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);
			_sensorManager = (SensorManager) GetSystemService(Context.SensorService);
			//	_sensorTextView = FindViewById<TextView>(Resource.Id.accelerometer_text);
#endif
			
#if true
			var layout = new LinearLayout(this);
			layout.Orientation = Android.Widget.Orientation.Vertical;
			
			_sensorTextView = new TextView(this);
			var aLabel = new TextView(this);
			aLabel.Text = "Clapping. With one hand. With a beer in the other.";
			
			var aButton = new Button(this);
			aButton.Text = "Do it";
			aButton.Click += (sender, e) => { 
				aLabel.Text = _stadium + count++;
				Clap(3);
			};
			
			layout.AddView (aLabel);
			layout.AddView (_sensorTextView);
			layout.AddView(aButton);
			SetContentView(layout);
#endif
		}
	}
}


