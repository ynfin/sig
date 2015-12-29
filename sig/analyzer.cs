using System;
using System.Collections.Generic;

namespace sig
{
    public class analyzer
    {
        public analyzer()
        {
        }

        // find main break points to segment into cycles
        public List<DateTime> FindDebugBreakPoints(List<sample> samples, double first, double second)
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
                    breakTimes.Add(samples[i].time);
                }
                    
            }
                
            return breakTimes;
        }




    }
}

