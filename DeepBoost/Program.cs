using System;
using System . Collections . ObjectModel;
using System . IO;

namespace DeepBoost
{
    class Program
    {
        static void Main ( string [ ] args )
        {
            for ( int i = 3 ; i <= 7 ; i++ )
            {
                for ( int j = 3 ; j <= 7 ; j++ )
                {
                    Double [ ] LAMBDA = new Double [ 100000 ];
                    Double lambda = Math . Pow ( 10 , -i );
                    Double beta = Math . Pow ( 10 , -j );
                    Console . WriteLine ( "lambda=" + lambda . ToString ( ) );
                    Console . WriteLine ( "beta=" + beta . ToString ( ) );
                    
                    for ( int loop = 0 ; loop < 100000 ; loop++ )
                    {
                        LAMBDA [ loop ] = lambda * Math . Sqrt ( ( 2 * Math . Log ( 2 * 351 * 34 ) ) / ( 351 ) ) + beta;
                    }
                    DeepBoost deepboost = new DeepBoost ( 50 , LAMBDA );

                    boosting ( deepboost );
                }
            }

            Console . WriteLine ( "FINISHED" );
            Console . ReadLine ( );
        }
        static void boosting( DeepBoost deepboost )
        {            
            Collection<Double [ ]> dataset = new Collection<Double [ ]> ( );
            Int32 [ ] label = new Int32 [ 295 ];
            StreamReader sr = new StreamReader ( "D:\\train-ionosphere-n5.data" );


            for ( int j = 0 ; j < 295 ; j++ )
            {
                String temp = sr . ReadLine ( );
                Double [ ] data = new Double [ 34 ];
                for ( int i = 0 ; i < 34 ; i++ )
                {
                    data [ i ] = Double . Parse ( temp . Split ( ',' ) [ i ] );
                }
                if ( temp . Split ( ',' ) [ 34 ] == "b" )
                    label [ j ] = 1;
                else
                    label [ j ] = -1;
                dataset . Add ( data );
            }

            Collection<Double [ ]> testset = new Collection<Double [ ]> ( );
            Int32 [ ] testlabel = new Int32 [ 70 ];
            sr = new StreamReader ( "D:\\test-ionosphere.data" );


            for ( int j = 0 ; j < 70 ; j++ )
            {
                String temp = sr . ReadLine ( );
                Double [ ] data = new Double [ 34 ];
                for ( int i = 0 ; i < 34 ; i++ )
                {
                    data [ i ] = Double . Parse ( temp . Split ( ',' ) [ i ] );
                }
                if ( temp . Split ( ',' ) [ 34 ] == "b" )
                    testlabel [ j ] = 1;
                else
                    testlabel [ j ] = -1;
                testset . Add ( data );
            }


            deepboost . SetTraining ( dataset , label );
            deepboost . SetTest ( testset , testlabel );

            deepboost . boost ( );

            Console . WriteLine ( );
        }
    }
}
