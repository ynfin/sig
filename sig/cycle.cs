using System;
using System.Collections.Generic; 

namespace sig
{
	public class cycle
	{
        public List<sample> samples = new List<sample>();

		public cycle ()
		{
		}

        public void addSample(sample sample)
        {
            this.samples.Add(sample);
        }

        public TimeSpan makeCycleSpan()
        {
            return samples[samples.Count - 1].time - samples[0].time;
        }

        public void generateDeltas()
        {
            foreach (var item in samples)
            {
                item.cycleDelta = item.time - samples[0].time;
            }
        }

	}
}

