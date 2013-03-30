using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using System.Text;
using Android.Media;

namespace clap1
{
	[Activity (Label = "clap1", MainLauncher = true)]
	public class Activity1 : Activity, ISensorEventListener 
	{
		int count = 1;
		private SensorManager _sensorManager;
		private TextView _sensorTextView;
		private static readonly object _syncLock = new object();

		public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
		{
			// We don't want to do anything here.
		}

		MediaPlayer _mediaPlayer = null;

		void Clap(TimeSpan duration)
		{
			_mediaPlayer.Start();
		}

		public void OnSensorChanged(SensorEvent e)
		{
			lock (_syncLock)
			{
//				switch (e.Sensor.
				var text = new StringBuilder("x = ")
					.Append(e.Values[0])
						.Append(", y=")
						.Append(e.Values[1])
						.Append(", z=")
						.Append(e.Values[2]);
				_sensorTextView.Text = text.ToString();

				var x = e.Values[0];
				if (x < -6) Clap(TimeSpan.FromSeconds(1));
			}
		}

	    protected override void OnPause()
        {
			base.OnPause();
			_sensorManager.UnregisterListener(this);
		}

		protected override void OnResume()
		{
			base.OnResume();
			_sensorManager.RegisterListener(this, _sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Ui);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

#if true
			_mediaPlayer = MediaPlayer.Create(this, Resource.Raw.clap1);
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
				aLabel.Text = "Clap " + count++;
				Clap(TimeSpan.FromSeconds(2));
			};

			layout.AddView (aLabel);
			layout.AddView (_sensorTextView);
			layout.AddView(aButton);
			SetContentView(layout);
#else
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
#endif
		}
	}
}


