using System;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace DeepBoost
{
    class DeepBoost
    {
        int roundLimit;
        Collection<double [ ]> trainingData;
        int [ ] labels;
        Collection<double [ ]> testData;
        int [ ] testlabels;
        int dimension = 32;
        HypothesisSet hypothesisSet;
        Double [ ] LAMBDA;
        Double [ ] S;
        Double [ ] eta;

        public DeepBoost ( int roundLimit , Double [ ] lambda )
        {
            this . roundLimit = roundLimit;
            this . LAMBDA = lambda;
            S = new Double [ roundLimit + 1 ];
            eta = new Double [ roundLimit + 1 ];
            S [ 1 ] = 1;
        }

        public void boost ( )
        {
            Collection<Double> trainingerror = new Collection<Double> ( );
            Collection<Double> testerror = new Collection<Double> ( );
            int sampleSize = trainingData . Count;
            double [ ] distribution = new double [ sampleSize ];
            for ( int i = 0 ; i < sampleSize ; i++ )
            {
                distribution [ i ] = 1.0 / sampleSize;
            }
            for ( int t = 1 ; t < roundLimit ; t++ )
            {
                // for each round
                //Console . WriteLine ( "Round " + t );
                // find a threshold function
                int j = 0;
                double maxd = double.MinValue;
                int k=0;
                foreach ( Hypothesis h in hypothesisSet . getHList ( ) )
                {
                    double epsilon = 0;
                    for ( int m = 0 ; m < sampleSize ; m++ )
                    {
                        if ( h . isPredictionRight ( trainingData [ m ] , labels [ m ] ) )
                            epsilon += distribution [ m ];
                        else
                            epsilon -= distribution [ m ];
                    }

                    epsilon = 0.5 * ( 1 - epsilon );

                    if ( h . getAlpha ( ) [ t - 1 ] != 0 )
                    {
                        if ( maxd < ( epsilon - 0.5 ) + Math . Sign ( h . getAlpha ( ) [ t - 1 ] ) * LAMBDA [ j ] / ( 2 * S [ t ] ) )
                        {
                            maxd = ( epsilon - 0.5 ) + Math . Sign ( h . getAlpha ( ) [ t - 1 ] ) * LAMBDA [ j ] / ( 2 * S [ t ] );
                            k = j;
                        }
                    }
                    else if ( Math . Abs ( epsilon - 0.5 ) <= LAMBDA [ j ] / ( 2 * S [ t ] ) )
                    {
                        if ( maxd < 0 )
                        {
                            maxd = 0;
                            k = j;
                        }
                    }
                    else
                    {
                        if ( maxd < ( epsilon - 0.5 ) - Math . Sign ( epsilon - 0.5 ) * LAMBDA [ j ] / ( 2 * S [ t ] ) )
                        {
                            maxd = ( epsilon - 0.5 ) - Math . Sign ( epsilon - 0.5 ) * LAMBDA [ j ] / ( 2 * S [ t ] );
                            k = j;
                        }
                    }
                    j++;
                }
                Hypothesis selectedh = hypothesisSet . getHList ( ) [ k ];
                double et = 0;
                for ( int m = 0 ; m < sampleSize ; m++ )
                {
                    if ( selectedh . isPredictionRight ( trainingData [ m ] , labels [ m ] ) )
                        et += distribution [ m ];
                    else
                        et -= distribution [ m ];
                }
                et = 0.5 * ( 1 - et );

                if ( Math . Abs ( ( 1 - et ) * Math . Exp ( selectedh . getAlpha ( ) [ t - 1 ] ) - et * Math . Exp ( -selectedh . getAlpha ( ) [ t - 1 ] ) ) <= ( LAMBDA [ k ] * sampleSize ) / ( S [ t ] ) )
                    eta [ t ] = -selectedh . getAlpha ( ) [ t - 1 ];
                else if ( ( 1 - et ) * Math . Exp ( selectedh . getAlpha ( ) [ t - 1 ] ) - et * Math . Exp ( -selectedh . getAlpha ( ) [ t - 1 ] ) > ( LAMBDA [ k ] * sampleSize ) / ( S [ t ] ) )
                    eta [ t ] = Math . Log ( -( LAMBDA [ k ] * sampleSize ) / ( 2 * et * S [ t ] ) + Math . Sqrt ( Math . Pow ( ( LAMBDA [ k ] * sampleSize ) / ( 2 * et * S [ t ] ) , 2 ) + ( 1 - et ) / ( et ) ) );
                else
                    eta [ t ] = Math . Log ( ( LAMBDA [ k ] * sampleSize ) / ( 2 * et * S [ t ] ) + Math . Sqrt ( Math . Pow ( ( LAMBDA [ k ] * sampleSize ) / ( 2 * et * S [ t ] ) , 2 ) + ( 1 - et ) / ( et ) ) );

                Double [ ] E = new Double [ hypothesisSet . getHList ( ) . Count ];
                E [ k ] = eta [ t ];

                for ( int i = 0 ; i < hypothesisSet . getHList ( ) . Count ; i++ )
                {
                    Double [ ] temp = hypothesisSet . getHList ( ) [ i ] . getAlpha ( );
                    temp [ t ] = temp [ t - 1 ] + E [ i ];
                    hypothesisSet . getHList ( ) [ i ] . updateAlpha ( temp );
                }

                S [ t + 1 ] = 0;
                for ( int m = 0 ; m < sampleSize ; m++ )
                {
                    S [ t + 1 ] += -Math . Exp ( 1 - hypothesisSet . getPrediction ( t , trainingData [ m ] , labels [ m ] ) );
                }
                for ( int m = 0 ; m < sampleSize ; m++ )
                {
                    distribution [ m ] = -Math . Exp ( 1 - hypothesisSet . getPrediction ( t , trainingData [ m ] , labels [ m ] ) ) / S [ t + 1 ];
                }
                trainingerror . Add ( this . validate ( t ) );
                testerror . Add ( this . testerror ( t ) );
            }

            //foreach ( Double err in trainingerror )
            //{
            //    Console . Write ( err . ToString ( ) + " " );
            //}
            //Console . WriteLine ( );
            //foreach ( Double err in testerror )
            //{
            //    Console . Write ( err . ToString ( ) + " " );
            //}
            Console . WriteLine ( trainingerror [ trainingerror . Count - 1 ] );
            Console . WriteLine ( testerror [ testerror . Count - 1 ] );
            Console . WriteLine ( );
        }

        public Double validate ( int time )
        {
            Double error=0.0;
            for ( int m = 0 ; m < trainingData . Count ; m++ )
            {
                if ( Math . Sign ( hypothesisSet . getPrediction ( time , this . trainingData [ m ] , this . labels [ m ] ) ) == -1 )
                    error += 1;
            }
            error /= trainingData . Count;
            //Console . WriteLine ( error );
            return error;

        }

        public Double testerror ( int time )
        {
            Double error = 0.0;
            for ( int m = 0 ; m < testData . Count ; m++ )
            {
                if ( Math . Sign ( hypothesisSet . getPrediction ( time , this . testData [ m ] , this . testlabels [ m ] ) ) == -1 )
                    error += 1;
            }
            error /= testData . Count;
            //Console . WriteLine ( error );
            return error;

        }

        public void SetTraining( Collection<double [ ]> t, int [ ] l )
        {
            this . trainingData = t;
            this . labels = l;
            hypothesisSet = new HypothesisSet ( roundLimit , dimension , trainingData . Count , trainingData );
        }
        public void SetTest ( Collection<double [ ]> t , int [ ] l )
        {
            this . testData = t;
            this . testlabels = l;
        }
    }
}