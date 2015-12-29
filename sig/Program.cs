using System;
using System.Collections.Generic;

namespace sig
{
	class MainClass
	{
        static List<DateTime> breakPoints;

		public static void Main (string[] args)
		{

            // --------- SETTINGS ----------

            string csvPath = @"/Users/yngve/Projects/Python/15bar_400cc.csv";
            //string csvPath = @"C:\Users\Yngve Finnestad\Documents\highpri_cornertest\15bar_400cc.csv";
            string triggerSignal = "DebugDCU1";
            bool ignoreStart = true;




			// read file
			inout io = new inout();
			io.readCSV(csvPath);
			List<signal> signals = io.getSignals();

            // get breakpoints
            foreach (var signal in signals)
            {
                if (signal.name.Contains("DebugDCU1"))
                    breakPoints = signal.findBreakPoints(6,7);

            }

            // use breakpoints to set cycle
            foreach (var signal in signals)
            {
                signal.makeCycles(breakPoints,ignoreStart);
            }

            // info dump
            Console.WriteLine("/////////////////////////////////////////////////////////////////////");
            Console.WriteLine("Csv Data file:  " + csvPath);
            Console.WriteLine("Signals count : " + signals.Count);
            Console.WriteLine("Trigger Signal: " + triggerSignal);
            Console.WriteLine("/////////////////////////////////////////////////////////////////////");

            foreach (var signal in signals)
                signal.printInfo();
            
            // write signal CSV file for each signal with overlapping cycles
            foreach (var signal in signals)
            {
                io.writeCSV(signal,csvPath,csvType.Excel);
            }
            
            
            
            
            
            
                


		}
	}
}
