using System;

namespace sig
{
    public class BreakCondition
    {
        double high;
        double low;
        int window;
        triggerType trigger;

        public BreakCondition(double high, double low, int window, triggerType trigger)
        {
            this.high = high;
            this.low = low;
            this.window = window;
            this.trigger = trigger;
        }
    }

    public enum triggerType
    {
        risingEdge,
        fallingEdge,
        threshhold
    }


}

