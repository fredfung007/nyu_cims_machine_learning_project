using System;
using System . Collections . ObjectModel;
using System . IO;

namespace DeepBoost
{
    class Program
    {
        static void Main ( string [ ] args )
        {
            DeepBoost deepboost = new DeepBoost ( 50 , new Double [ 100000 ] );
            Collection<Double [ ]> dataset = new Collection<Double [ ]> ( );
            Int32 [ ] label = new Int32 [ 351 ];
            StreamReader sr = new StreamReader ( "D:\\ionosphere.data.txt" );


            for ( int j = 0 ; j < 351 ; j++ )
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

            deepboost . SetTraining ( dataset , label );

            deepboost . boost ( );

            deepboost . validate ( 50 );
        }
    }
}
