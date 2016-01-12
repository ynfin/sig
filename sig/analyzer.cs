using System;
using System.Collections.Generic;

namespace sig
{
    public class analyzer
    {
        public analyzer()
        {
        }

        public List<analyzerSample> getVarianceArray(signal signal)
        {
            // go through each cycle and sample points with desired intervalls
            // store the samples in an array in a list at i = sample #

            List<analyzerSample> linspaceList = new List<analyzerSample>();
            TimeSpan maxtime = new TimeSpan();
            TimeSpan activeTime = new TimeSpan(0,0,0,0,0);
            int interval = 100;


            // determine the maximum time needed
            foreach (var cycle in signal.cycleList)
            {
                TimeSpan newtime = cycle.samples[cycle.samples.Count - 1].cycleDelta;
                if (maxtime.CompareTo(newtime) < 0)
                {
                    maxtime = newtime;
                }
            }

            // build empty linspace list
            while (activeTime.CompareTo(maxtime) < 0)
            {
                linspaceList.Add(new analyzerSample(activeTime));
                activeTime = activeTime.Add(new TimeSpan(0, 0, 0, 0, interval));
            }

            activeTime = new TimeSpan(0, 0, 0, 0, 0);
            foreach (var linspacePoint in linspaceList) // this point must be filled by one from each cycle
            {
                foreach (var cycle in signal.cycleList) // start with one cycle
                {
                    foreach (var sample in cycle.samples) // find the sample to add
                    {
                        if (sample.cycleDelta.CompareTo(linspacePoint.timePoint) >= 0) // if time has been reached
                        {
                            linspacePoint.addToList(sample);
                            linspacePoint.signalName = signal.name;
                            break;
                        }
                    }
                }
                activeTime = activeTime.Add(new TimeSpan(0, 0, 0, 0, interval));
            }

            foreach (var linspacePoint in linspaceList)
                linspacePoint.processStats();

                return linspaceList;
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

