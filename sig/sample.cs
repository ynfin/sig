using System;

namespace sig
{
	public class sample
	{
		public string rawTimeStamp;

        public DateTime time;
        public double value;

        public TimeSpan delta;
        public TimeSpan cycleDelta;


		public sample (string rawtime, string rawdata)
		{
			rawTimeStamp = rawtime;
            value = Convert.ToDouble(rawdata);

            time = DateTime.ParseExact(rawtime, "dd:MM:yyyy HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
		}

        public string getSigAnTime()
        {
            return time.ToString("dd:MM:yyyy HH:mm:ss:fff");
        }

	}
}

