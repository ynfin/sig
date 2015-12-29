using System;
using System.Collections.Generic;

namespace sig
{
	public class signal
	{
		public cycle cycle;

        public string rawinfo;
		public string name;
		public string IP;
		public string robot;
		public string unit;
		public string type;
        public bool used = false;

        public TimeSpan span;

		public List<sample> samples;
        public List<cycle> cycleList = new List<cycle>();

		public signal()
		{
			samples = new List<sample>();
		}
            
        public void addIP(string ipString)
        {
            int pFrom = ipString.IndexOf("@IPS_") + "@IPS_".Length;
            int pTo = ipString.Length;

            IP = ipString.Substring(pFrom, pTo - pFrom); 
        }

        public void addName(string namestring){
            
            int pFrom = namestring.IndexOf("DisplayName: ObjectModel.") + "DisplayName: ObjectModel.".Length;
            int pTo = namestring.LastIndexOf("@IPS_");

            name = namestring.Substring(pFrom, pTo - pFrom);
            name = name.Replace("/", "-");
            name = name.Replace(":", "-");
        }
 
        public void addSource(string sourceString)
        {
            int pFrom = sourceString.IndexOf("Source: ") + "Source: ".Length;
            int pTo = sourceString.Length;

            robot = sourceString.Substring(pFrom, pTo - pFrom); 
        }

        public void addUnit(string unitString)
        {
            int pFrom = unitString.IndexOf("Unit: ") + "Unit: ".Length;
            int pTo = unitString.Length;

            unit = unitString.Substring(pFrom, pTo - pFrom); 
        }

        public void addType(string typeString)
        {
            int pFrom = typeString.IndexOf("SignalType: ") + "SignalType: ".Length;
            int pTo = typeString.Length;

            type = typeString.Substring(pFrom, pTo - pFrom); 
        }
            
        public string generateNumeredRawInfo(string insertString)
        {
            int startidx = rawinfo.IndexOf("DisplayName: ") + "DisplayName: ".Length;
            string modifiedraw = rawinfo.Insert(startidx, insertString);
            return modifiedraw;   
        }


        public List<DateTime> findBreakPoints(double first,double second)
        {
            analyzer a = new analyzer();
            var breaks = a.FindDebugBreakPoints(samples, first, second);
            return breaks;
        }

        public void processStoredData()
        {
            // find signal time data
            DateTime endtime = samples[samples.Count - 1].time;
            span = endtime - samples[0].time;

            foreach (var sample in samples)
            {
                // set each signal delta time
                sample.delta = sample.time - samples[0].time;
            }
        
        }

        public void makeCycles(List<DateTime> timeArray,bool clearLeading)
        {
            int idxBreaks = 0;

            cycle cycle = new cycle();

            for (int i = 0; i < samples.Count; i++)
            {
                if (samples[i].time.CompareTo(timeArray[idxBreaks]) < 0)
                {
                    cycle.addSample(samples[i]);
                }
                else
                {
                    idxBreaks++;
                    cycleList.Add(cycle);
                    cycle = new cycle();

                    // gone past last breaktime, store rest manually
                    if (idxBreaks > timeArray.Count -1)
                    {
                        while (i < samples.Count)
                        {
                            cycle.addSample(samples[i]);
                            i++;
                        }
                        cycleList.Add(cycle);
                    }
                }
            }

            if (clearLeading)
            {
                cycleList.RemoveAt(0);
            }
                
            // build cycle deltas after completed segmentation
            foreach (var completeCycle in cycleList)
            {
                completeCycle.generateDeltas();
            }


        }

		public void addSample(sample sample)
		{
			samples.Add(sample);
		}
            
        public void printInfo()
        {
            Console.WriteLine("\n---------------" + name + "---------------");
            Console.WriteLine("samples:\t " + samples.Count);
            Console.WriteLine("timespan:\t " + span.Hours + ":" + span.Minutes + ":" + span.Seconds +"  ("+span.TotalSeconds+")");
            Console.WriteLine("cycles:\t " + cycleList.Count + "\n");

            foreach (var item in cycleList)
            {
                Console.WriteLine("[" + item.samples[0].time.ToString("HH':'mm':'ss.fff") + "\t -->  " +
                    item.samples[item.samples.Count -1].time.ToString("HH':'mm':'ss.fff") + "]\t (" +
                    item.makeCycleSpan() + ")");
            }
                
        }



	}
}

