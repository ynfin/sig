using System;
using System.Collections.Generic;
using System.IO;

namespace sig
{
	class MainClass
	{
        static List<DateTime> breakPoints;

        // --------- SETTINGS ----------

        //public static string csvPath = @"C:\ProgramData\ABB Industrial IT\Robotics IT\RobView5\ABB.Robotics.Paint.RobView.Plugin.SignalAnalyzer\heatgrind_5x_max_170C.csv";
        //public static string csvPath = @"/Users/yngve/Projects/Python/15bar_400cc.csv";
        //public string csvPath = @"C:\Users\Yngve Finnestad\Documents\highpri_cornertest\15bar_400cc.csv";
        //public static string csvPathFolder = @"/Volumes/DOS/!TRANSFER/FeedTubeGrind copy/heavygrind/for_graph";
        //public static string csvPathFolder = @"/Volumes/DOS/!TRANSFER/FeedTubeGrind copy/finegrind/3sec initial";
        public static string csvPathFolder = @"C:\ProgramData\ABB Industrial IT\Robotics IT\RobView5\ABB.Robotics.Paint.RobView.Plugin.SignalAnalyzer\heat_slope_cycles";


        public static string triggerSignal = "A1Atom.SetPoint";
        public static double first_trigger = 20;
        public static double second_triger = 0;
        public static int samples_before = 0;
        public static bool ignoreStart = true;
        static int outputnumber = 0;

        public static void Main (string[] args)
		{

            string[] filePaths = Directory.GetFiles(csvPathFolder, "*.csv");

            //processFile(csvPath);
            

            foreach (var path in filePaths)
            {
                processFile(path);
            }

           
		}

        public static void processFile(string filePath)
        {
            // read file
            inout io = new inout();
            io.readCSV(filePath);
            List<signal> signals = io.getSignals();

            // get breakpoints
            foreach (var signal in signals)
            {
                if (signal.name.Contains(triggerSignal))
                {
                    breakPoints = signal.findBreakPoints(first_trigger, second_triger, samples_before);
                }
            }

            // use breakpoints to set cycle
            foreach (var signal in signals)
            {
                signal.makeCycles(breakPoints,ignoreStart);
            }

            // info dump
            Console.WriteLine("/////////////////////////////////////////////////////////////////////");
            Console.WriteLine("Csv Data file:  " + filePath);
            Console.WriteLine("Signals count : " + signals.Count);
            Console.WriteLine("Trigger Signal: " + triggerSignal);
            Console.WriteLine("/////////////////////////////////////////////////////////////////////");

            foreach (var signal in signals)
                signal.printInfo();


            // trim the signal
            List<signal> trimmedSignals = new List<signal>();

            foreach (var signal in signals)
            {
                analyzer a = new analyzer();
                trimmedSignals.Add(a.trimEndTime(signal, new TimeSpan(0, 0, 10)));
            }

            foreach (var signal in signals)
            {

            }


            foreach (var signal in signals)
            {
                if (signal.name.Contains("A1Atom.Actual"))
                {
                    List<analyzerSample> analyzedSamples = new List<analyzerSample>();

                    analyzer a = new analyzer();
                    analyzedSamples = a.getVarianceArray(signal);
                    io.writeAnalyzedSignalsToCSV(analyzedSamples,filePath,false);
                }

                
            }

            // write signal CSV file for each signal with overlapping cycles
            //foreach (var signal in signals)
            foreach (var signal in trimmedSignals)
            {
                //io.writeCSV(signal,filePath,csvType.Excel);
            }
        }

	}
}
