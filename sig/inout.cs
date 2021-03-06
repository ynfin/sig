﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace sig
{
    public class inout
    {
        List<signal> signalList = new List<signal>();
        signal tempsig = new signal();

        int signalCount = 0;
        int sampleCount = 0;


        public inout()
        {
        }

        public List<signal> getSignals()
        {
            return signalList;
        }


        public void writeAnalyzedSignalsToCSV(List<analyzerSample> linspaceSamples, string dataFileOutputPath, bool writeSampleData)
        {
            StringBuilder fullSignalString = new StringBuilder();
            string filename = Path.GetFileNameWithoutExtension(dataFileOutputPath);
            
            // <header>
            fullSignalString.Append("sec,mean,var,sd,sigma1_upper,sigma1_lower,sigma2_upper,sigma2_lower,sigma3_upper,sigma3_lower");

            if (writeSampleData)
            {
                int cyclenumber = 0;
                foreach (var number in linspaceSamples[0].numberList)
                {
                    cyclenumber++;
                    fullSignalString.Append("," + filename + "cycle-" + cyclenumber);
                }
            }

            fullSignalString.AppendLine("");
            //</header>

            foreach (var sample in linspaceSamples)
            {
                // first column
                string num = sample.timePoint.TotalSeconds.ToString();
                fullSignalString.Append(num.ToString().Replace(',', '.'));

                // stats columns
                fullSignalString.Append(
                    "," + sample.average.ToString().Replace(',', '.') +
                    "," + sample.sumOfSquaresOfDifferences.ToString().Replace(',', '.') +
                    "," + sample.standardDeviation.ToString().Replace(',', '.') +
                    "," + (sample.average + sample.standardDeviation * 1).ToString().Replace(',', '.') +
                    "," + (sample.average - sample.standardDeviation * 1).ToString().Replace(',', '.') +
                    "," + (sample.average + sample.standardDeviation * 2).ToString().Replace(',', '.') +
                    "," + (sample.average - sample.standardDeviation * 2).ToString().Replace(',', '.') +
                    "," + (sample.average + sample.standardDeviation * 3).ToString().Replace(',', '.') +
                    "," + (sample.average - sample.standardDeviation * 3).ToString().Replace(',', '.')
                    );

                if (writeSampleData)
                {
                    // data columns
                    foreach (var number in sample.numberList)
                    {
                        fullSignalString.Append("," + number.ToString().Replace(',', '.'));
                    }
                }
              
                // end
                fullSignalString.AppendLine("");
            }

            string signame = linspaceSamples[0].signalName;
            var dir = Directory.CreateDirectory(dataFileOutputPath.Remove(dataFileOutputPath.Length - 4));
            string fullSignalPath = dir.FullName + "/" + filename + "[stats_" + signame + "].csv";
            // Write the string to a file.
            System.IO.StreamWriter singlefile = new System.IO.StreamWriter(fullSignalPath);
            singlefile.WriteLine(fullSignalString);
            singlefile.Close();
        }


        public void readCSV(string filepath)
        {
            var reader = new StreamReader(File.OpenRead(filepath));

            while (!reader.EndOfStream)
            {
                // read line
                var line = reader.ReadLine();

                // new signal started, create temp signal to add to later
                if (line.Contains("DisplayName: "))
                {
                    // last line of log, break all
                    if (line.Contains("SignalType: EventLog"))
                        break;

                    if (tempsig.used == true)
                    {
                        tempsig.processStoredData();
                        signalList.Add(tempsig);
                        Console.WriteLine("Signal Addded: " + tempsig.name + " with " + tempsig.samples.Count + " samples!" + tempsig.IP);
                    }

                    tempsig = new signal();

                    var info = line.Split(',');
                    tempsig.rawinfo = line;
                    tempsig.addName(info[0]);
                    tempsig.addIP(info[1]);
                    tempsig.addSource(info[2]);
                    tempsig.addUnit(info[3]);
                    tempsig.addType(info[4]);
                    tempsig.used = true;
                    signalCount++;
                }
                else
                {
                    if (line != "")
                    {
                        var values = line.Split(',');
                        var sample = new sample(values[0], values[1]);
                        tempsig.addSample(sample);
                        sampleCount++;
                    }
                }
            }
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("read " + sampleCount + " samples from " + signalCount + " signals...\n");
        }

        public void writeCSV(signal signal,string path,csvType type)
        {
            
            // build string from the given signal, use folder in input path as storage dir
           
            string filename = Path.GetFileNameWithoutExtension(path);
            var dir = Directory.CreateDirectory(path.Remove(path.Length - 4));
            string fullSignalPath = dir.FullName + "/" + filename + "[single_"+ type.ToString() + "_" + signal.name + "].csv";
            string cycleSignalPath = dir.FullName + "/" + filename + "[cycles_"+ type.ToString() + "_" + signal.name + "].csv";
   
            StringBuilder fullSignalString = new StringBuilder();
            StringBuilder cycleSignalString = new StringBuilder();

            // write header
            switch (type)
            {
                case csvType.SigAn: 
                    fullSignalString.AppendLine(signal.rawinfo);
                    break;

                case csvType.SigAn_delta:
                    fullSignalString.AppendLine(signal.rawinfo);
                    break;

                case csvType.Excel: 
                    fullSignalString.AppendLine("time,"+signal.name);
                    break;

                default:
                    break;
            }
                    
            // write single line data
            for (int i = 0; i < signal.samples.Count; i++)
            {
                switch (type)
                {
                    
                    case csvType.SigAn: 
                        // standard signal analyzer format, only available for full signal plot
                        fullSignalString.AppendLine(signal.samples[i].getSigAnTime() + "," + signal.samples[i].value);
                        break;

                    case csvType.SigAn_delta:
                        fullSignalString.AppendLine(signal.samples[i].delta.ToString("dd:MM:yyyy HH:mm:ss:fff") + "," + signal.samples[i].value);
                        // signal analyzer format, but starts at time 0 (used for cycles)
                        break;
                    
                    case csvType.Excel:
                        string deltasec = signal.samples[i].delta.TotalSeconds.ToString().Replace(',','.');
                        string value = signal.samples[i].value.ToString().Replace(',', '.');
                        fullSignalString.AppendLine(deltasec + "," + value);
                        //fullSignalString.AppendLine(signal.samples[i].delta.TotalSeconds + "," + signal.samples[i].value);
                        break;

                    default:
                        break;
                }
            }
                
            // write signal cycle data
            int cycleNumber = 0;
            foreach (var cycle in signal.cycleList)
            {
                cycleNumber++;
                // write header
                switch (type)
                {
                    case csvType.SigAn: 
                        cycleSignalString.AppendLine(signal.generateNumeredRawInfo(cycleNumber.ToString()));
                        break;

                    case csvType.SigAn_delta:
                        cycleSignalString.AppendLine(signal.generateNumeredRawInfo(cycleNumber.ToString()));
                        break;

                    case csvType.Excel: 
                        cycleSignalString.AppendLine("\ntime," + signal.name+"_"+cycleNumber.ToString());
                        break;

                    default:
                        break;
                }


                for (int i = 0; i < cycle.samples.Count; i++)
                {
                    switch (type)
                    {

                        case csvType.SigAn:
                            // standard signal analyzer format, only available for full signal plot
                            cycleSignalString.AppendLine(cycle.samples[i].getSigAnTime() + "," + cycle.samples[i].value);
                            break;

                        case csvType.SigAn_delta:
                            cycleSignalString.AppendLine(cycle.samples[i].cycleDelta.ToString("dd:MM:yyyy HH:mm:ss:fff") + "," + cycle.samples[i].value);
                            // signal analyzer format, but starts at time 0 (used for cycles)
                            break;

                        case csvType.Excel:
                            string deltasec = cycle.samples[i].cycleDelta.TotalSeconds.ToString().Replace(',', '.');
                            string value = cycle.samples[i].value.ToString().Replace(',', '.');
                            cycleSignalString.AppendLine(deltasec + "," + value);

                            //cycleSignalString.AppendLine(cycle.samples[i].cycleDelta.TotalSeconds + "," + cycle.samples[i].value);
                            break;

                        default:
                            break;
                    }

                }
            }


            // Write the string to a file.
            System.IO.StreamWriter singlefile = new System.IO.StreamWriter(fullSignalPath);
            singlefile.WriteLine(fullSignalString);
            singlefile.Close();

            System.IO.StreamWriter cyclefile = new System.IO.StreamWriter(cycleSignalPath);
            cyclefile.WriteLine(cycleSignalString);
            cyclefile.Close();
        }
            
    }

    public enum csvType
    {
        SigAn,
        SigAn_delta,
        Excel
    }
}

