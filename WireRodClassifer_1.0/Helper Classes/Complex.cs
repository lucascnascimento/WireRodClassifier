using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0
{
    public class Complex
    {
        #region Campos

        double _real = 0;
        double _imag = 0;

        #endregion

        #region Propriedades

        public double Real
        {
            get
            {
                return _real;
            }
            set
            {
                _real = value;
            }
        }

        public double Imag
        {
            get
            {
                return _imag;
            }
            set
            {
                _imag = value;
            }
        }

        #endregion

        #region Construtores

        public Complex(double real, double imag)
        {
            Real = real;
            Imag = imag;
        }

        public Complex()
        {

        }

        #endregion

        #region Métodos

        public static Complex SubtraiComplexo(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imag - b.Imag);
        }

        // Overload operador +
        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imag + b.Imag);
        }

        // Overload operador -
        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imag - b.Imag);
        }

        // Overload operador *
        public static Complex operator *(Complex a, Complex b)
        {
            Complex tmp = new Complex();
            tmp.Real = (a.Real * b.Real) - (a.Imag * b.Imag);
            tmp.Imag = (a.Real * b.Real) + (a.Imag * b.Imag);
            return tmp;
        }

        // Overload operador /
        public static Complex operator /(Complex a, Complex b)
        {
            double div = (b.Real * b.Real) + (b.Imag * b.Imag);
            Complex tmp = new Complex();
            tmp.Real = (a.Real * b.Real) + (a.Imag * b.Imag);
            tmp.Real /= div;
            tmp.Imag = (a.Imag * b.Real) - (a.Real * b.Imag);
            tmp.Imag /= div;
            return tmp;
        }

        // Retorna o conjugado
        public Complex GetConjugate()
        {
            Complex tmp = new Complex();
            tmp.Real = this.Real;
            tmp.Imag = this.Imag * -1;
            return tmp;
        }

        // Retorna o recíproco
        public Complex GetReciprocal()
        {
            Complex t = new Complex();
            t.Real = this.Real;
            t.Imag = this.Imag * -1;
            double div;
            div = (this.Real * this.Real) + (this.Imag * this.Imag);
            t.Real /= div;
            t.Imag = div;
            return t;
        }

        // Retorna o módulo
        public double GetModulus()
        {
            double z;
            z = (this.Real * this.Real) + (this.Imag * this.Imag);
            z = Math.Sqrt(z);
            return z;
        }

        public void SetData(double r, double i)
        {
            this.Real = r;
            this.Imag = i;
        }

        // Retorna a parte Real
        public double GetReal()
        {
            return this.Real;
        }

        // Retorna a parte Imaginária
        public double GetImag()
        {
            return this.Imag;
        }

        #endregion
    }
}
