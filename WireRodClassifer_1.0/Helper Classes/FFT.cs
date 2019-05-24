using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0
{
    public static class FFT
    {
        
        #region Métodos

        //public static int calculaFFT(int N, Complex[] Input, Complex[] Output)
        public static Complex[] calculaFFT(Complex[] input)
        {
            #region New FFT 3 Mistura dos dois códigos

            int N = input.Length;
            Complex[] Output = new Complex[N];
            int NSobre2 = N >> 1;

            Complex wn = new Complex();
            wn.Real = Math.Cos(2.0 * Math.PI / N);
            wn.Imag = -(Math.Sin(2.0 * Math.PI / N));


            Complex[] in_impar, out_impar, in_par, out_par;

            if (N == 1)
            {
                Output[0] = input[0];
                return Output;
            }

            in_par = new Complex[NSobre2];
            in_impar = new Complex[NSobre2];
            out_par = new Complex[NSobre2];
            out_impar = new Complex[NSobre2];
            

            for (int aux = 0, aux2 = 0; aux < N; aux += 2, aux2++)
            {
                in_par[aux2] = input[aux];
            }

            for (int aux = 1, aux2 = 0; aux < N; aux += 2, aux2++)
            {
                in_impar[aux2] = input[aux];
            }

            out_impar = calculaFFT(in_impar);
            out_par = calculaFFT(in_par);

            Complex w = new Complex(1, 0);

            for (int aux = 0; aux <= NSobre2 - 1; aux++)
            {
                Output[aux] = out_par[aux] + w * out_impar[aux];
                Output[aux + NSobre2] = out_par[aux] - w * out_impar[aux];
                w = w * wn;
            }

            return Output;
            #endregion
        }
        #endregion
    }
}
