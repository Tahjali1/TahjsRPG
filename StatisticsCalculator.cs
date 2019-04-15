using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class StatisticsCalculator
    {
        public static int AbilityScoreCalculator(int statistic)
        {

            if(statistic == 1)
            {
                return -5;
            }

            if(statistic == 2 || statistic == 3 )
            {
                return -4;

            }

            if (statistic == 4 || statistic == 5)
            {
                return -3;

            }

            if (statistic == 6 || statistic == 7)
            {
                return -2;

            }

            if (statistic == 8 || statistic == 9)
            {
                return -1;

            }

            if (statistic == 10 || statistic == 11)
            {
                return 0;
            }

            if (statistic == 12 || statistic == 13)
            {
                return 1;

            }

            if (statistic == 14 || statistic == 15)
            {
                return 2;

            }

            if (statistic == 16 || statistic == 17)
            {
                return 3;

            }

            if (statistic == 18 || statistic == 19)
            {
                return 4;

            }

            if (statistic == 20 || statistic == 21)
            {
                return 5;

            }

            else
            {
                return 10;

            }




        }
    }
}
