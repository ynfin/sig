using System;
using System.Collections.Generic;

namespace sig
{
    public class analyzer
    {
        public analyzer()
        {
        }

        // trim signal length to given time
        public signal trimEndTime(signal sig, TimeSpan cutAtTime)
        {
            foreach (var cyc in sig.cycleList)
            {
                int cutSample = -1;
                for (int i = 0; i < cyc.samples.Count; i++)
                {
                    if (cyc.samples[i].cycleDelta > cutAtTime)
                    {
                        cutSample = i;
                        break;
                    }
                }

                if (cutSample > 0)
                {
                    int trimCount = cyc.samples.Count - cutSample;
                    cyc.samples.RemoveRange(cutSample, trimCount );
                    Console.WriteLine( "trimstart:" + cutSample + " -> " + (cutSample + trimCount) + "[" + trimCount + "]");
                }
            }

            return sig;
        }



        // find main break points to segment into cycles
        public List<DateTime> FindDebugBreakPoints(List<sample> samples, double first, double second, int before)
        {
            int lasttrigger = -10;

            List<DateTime> breakTimes = new List<DateTime>();

            for (int i = 0; i < samples.Count - 1; i++)
            {
                var nowValue = samples[i].value;
                var nextValue = samples[i+1].value;

                if ((nowValue == first) && (nextValue == second) && (i > lasttrigger + 10))
                {
                    lasttrigger = i;
                    //breakTimes.Add(samples[i].time);

                    if (before != 0 && i > before)
                    {
                        breakTimes.Add(samples[i-before].time);
                    }
                    else
                    {
                        breakTimes.Add(samples[i].time);
                    }

                }
                    
            }
                
            return breakTimes;
        }




    }
}

