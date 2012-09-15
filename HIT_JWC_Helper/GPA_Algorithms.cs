using System;
using System.Collections.Generic;
using System.Text;
/*
 * Author:  Xu Zhipei
 * Email:   xuzhipei@gmail.com
 * Licence: MIT
 * */
namespace HIT_JWC_Helper
{
    class GPA_Algorithms
    {
        /*90-100  4.0   
        85-89   3.7   
        82-84   3.3   
        78-81   3.0   
        75-77   2.7   
        72-74   2.3   
        68-71   2.0   
        64-67   1.5   
        60-63   1.0   
        <60     0*/
        static public double Peking(double aveg)
        {
            int s = (int)aveg;
            if (s >= 90)
                return 4.0;
            else if (s >= 85)
                return 3.7;
            else if (s >= 82)
                return 3.2;
            else if (s >= 78)
                return 3.0;
            else if (s >= 75)
                return 2.7;
            else if (s >= 72)
                return 2.3;
            else if (s >= 68)
                return 2.0;
            else if (s >= 64)
                return 1.5;
            else if (s >= 60)
                return 1.0;
            else
                return 0.0;
        }
        /*90-100  A  4.0  
          80-89   B  3.0   
          70-79   C  2.0  
          60-69   D  1.0*/
        static public double Standard(double aveg)
        {
            int s = (int)aveg;
            if (s >= 90)
                return 4.0;
            else if (s >= 80)
                return 3.0;
            else if (s >= 70)
                return 2.0;
            else if (s >= 60)
                return 1.0;
            else
                return 0.0;
        }
        /*85-100     4.0  
          70-85      3.0  
          60-70      2.0  
          0-60       0*/
        static public double Standard_Improved(double aveg)
        {
            int s = (int)aveg;
            if (s >= 85)
                return 4.0;
            else if (s >= 70)
                return 3.0;
            else if (s >= 60)
                return 1.0;
            else
                return 0.0;
        }
        /*90-100  4.3  
             85-90   4.0  
             80-85   3.7  
             75-80   3.3  
             70-75   3  
             65-70   2.7  
             60-65   2.3*/
        static public double Standard_4_3(double aveg)
        {
            int s = (int)aveg;
            if (s >= 90)
                return 4.3;
            else if (s >= 85)
                return 4.0;
            else if (s >= 80)
                return 3.7;
            else if (s >= 75)
                return 3.3;
            else if (s >= 70)
                return 3.0;
            else if (s >= 65)
                return 2.7;
            else if (s >= 60)
                return 2.3;
            else
                return 0.0;
        }
        static public double Zhejiang(double aveg)
        {
            int s = (int)aveg;
            if (s > 85) 
                return 4;
            else 
                return 4 - (85 - s) / 10;
        }
    }
}
