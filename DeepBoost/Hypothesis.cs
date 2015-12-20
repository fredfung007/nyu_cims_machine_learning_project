using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace DeepBoost
{
    class Hypothesis
    {
        Double threshold;
        Int32 sign;
        Int32 dimension;
        Double[] alpha;
        public Hypothesis ( Double threshold , Int32 sign , Int32 dimension , Int32 time )
        {
            this . threshold = threshold;
            this . sign = sign;
            this . dimension = dimension;
            this . alpha = new Double[ time+1 ];
        }

        public Double[] getAlpha()
        {
            return this.alpha;
        }

        public Int32 label ( Double [ ] x )
        {
            if ( x [ dimension ] < threshold )
                return sign;
            else
                return -sign;
        }

        public Double alphaPrediction ( Int32 time , Double [ ] x )
        {
            return alpha [ time ] * label ( x );
        }

        public Boolean isPredictionRight ( Double [ ] x , Int32 y )
        {
            if ( label ( x ) == y )
                return true;
            return false;
        }
        public void updateAlpha(Double[] newAlpha)
        {
            this . alpha = newAlpha;
        }
    }
}
