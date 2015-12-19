using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace DeepBoost
{
    class HypothesisSet
    {
        Collection<Hypothesis> hypothesisList = new Collection<Hypothesis> ( );
        Int32 dimension;
        int roundLimit;
        int sampleSize;

        public HypothesisSet ( Int32 RoundLimit , Int32 Dimension , Int32 SampleSize , Collection<double [ ]> trainingData )
        {
            this . dimension = Dimension;
            this . roundLimit = RoundLimit;
            this . sampleSize = SampleSize;
            this . hypothesisList = new Collection<Hypothesis> ( );
            int [ ] signs = { 1 , -1 };


            foreach ( int thresholdSign in signs )
                for ( int loop = 0 ; loop < dimension ; loop++ )
                    for ( int m = 0 ; m < sampleSize ; m++ )
                    {
                        double threshold = trainingData [ m ] [ loop ];
                        Hypothesis h = new Hypothesis ( threshold , thresholdSign , loop , roundLimit );
                        this . hypothesisList . Add ( h );
                    }
        }
        public Collection<Hypothesis> getHList ( )
        {
            return this . hypothesisList;
        }

        public Double getPrediction(Int32 time, Double [ ] x , Int32 y )
        {
            Double result = 0.0;
            Double temp = 0.0;
            foreach ( Hypothesis h in this . hypothesisList )
            {
                temp += h . alphaPrediction ( time , x );
            }
            result = y * temp;
            return result;
        }

    }
}