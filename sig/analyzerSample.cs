using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sig
{
    public class analyzerSample
    {

        public List<sample> sampleList = new List<sample>();
        public List<double> numberList = new List<double>();
        public TimeSpan timePoint = new TimeSpan();
        public string signalName = "";

        public double average;
        public double sumOfSquaresOfDifferences;
        public double standardDeviation;

        public void processStats()
        {
            average = numberList.Average();
            sumOfSquaresOfDifferences = numberList.Select(val => (val - average) * (val - average)).Sum();
            standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / numberList.Count);
        }

        public void setSignalName(string name)
        {
            signalName = name;
        }

        public void setTimePoint(TimeSpan ts)
        {
            timePoint = ts;
        }

        public void addToList(sample sample)
        {
            sampleList.Add(sample);
            numberList.Add(sample.value);
        }

        public analyzerSample(TimeSpan currentTs)
        {
            timePoint = currentTs;
        }

    }
}
