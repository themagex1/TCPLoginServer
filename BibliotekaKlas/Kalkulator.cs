using System;
using System.Collections.Generic;
using System.Text;

namespace BibliotekaSerwera
{
    class Kalkulator
    {
        public static int printResult(int n)
        {
            return n * 2;
        }

        public static int[] calc(int one, int two)
        {
            int[] tb = new int[2];
            tb[0] = one + two;
            tb[1] = one * two;
            return tb;
        }
    }
}
