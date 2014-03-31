/*
 * BSD Licence:
 * Copyright (c) 2001, 2002 Ben Houston [ ben@exocortex.org ]
 * Exocortex Technologies [ www.exocortex.org ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

//using Exocortex.Imaging;

namespace Exocortex.DSP
{
    // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
    // Version: May 4, 2002

    /// <summary>
    /// <p>The direction of the fourier transform.</p>
    /// </summary>
    public enum FourierDirection : int
    {
        /// <summary>
        /// Forward direction.  Usually in reference to moving from temporal
        /// representation to frequency representation
        /// </summary>
        Forward = 1,
        /// <summary>
        /// Backward direction. Usually in reference to moving from frequency
        /// representation to temporal representation
        /// </summary>
        Backward = -1,
    }


    // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
    // Version: May 4, 2002

    /// <summary>
    /// <p>A single-precision complex number representation.</p>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ComplexF : IComparable, ICloneable
    {

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The real component of the complex number
        /// </summary>
        public float Re;

        /// <summary>
        /// The imaginary component of the complex number
        /// </summary>
        public float Im;

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        public ComplexF(float real, float imaginary)
        {
            this.Re = (float)real;
            this.Im = (float)imaginary;
        }

        /// <summary>
        /// Create a complex number based on an existing complex number
        /// </summary>
        /// <param name="c"></param>
        public ComplexF(ComplexF c)
        {
            this.Re = c.Re;
            this.Im = c.Im;
        }

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        /// <returns></returns>
        static public ComplexF FromRealImaginary(float real, float imaginary)
        {
            ComplexF c;
            c.Re = (float)real;
            c.Im = (float)imaginary;
            return c;
        }

        /// <summary>
        /// Create a complex number from a modulus (length) and an argument (radian)
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        static public ComplexF FromModulusArgument(float modulus, float argument)
        {
            ComplexF c;
            c.Re = (float)(modulus * System.Math.Cos(argument));
            c.Im = (float)(modulus * System.Math.Sin(argument));
            return c;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        object ICloneable.Clone()
        {
            return new ComplexF(this);
        }
        /// <summary>
        /// Clone the complex number
        /// </summary>
        /// <returns></returns>
        public ComplexF Clone()
        {
            return new ComplexF(this);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The modulus (length) of the complex number
        /// </summary>
        /// <returns></returns>
        public float GetModulus()
        {
            float x = this.Re;
            float y = this.Im;
            return (float)Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// The squared modulus (length^2) of the complex number
        /// </summary>
        /// <returns></returns>
        public float GetModulusSquared()
        {
            float x = this.Re;
            float y = this.Im;
            return (float)x * x + y * y;
        }

        /// <summary>
        /// The argument (radians) of the complex number
        /// </summary>
        /// <returns></returns>
        public float GetArgument()
        {
            return (float)Math.Atan2(this.Im, this.Re);
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Get the conjugate of the complex number
        /// </summary>
        /// <returns></returns>
        public ComplexF GetConjugate()
        {
            return FromRealImaginary(this.Re, -this.Im);
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Scale the complex number to 1.
        /// </summary>
        public void Normalize()
        {
            double modulus = this.GetModulus();
            if (modulus == 0)
            {
                throw new DivideByZeroException("Can not normalize a complex number that is zero.");
            }
            this.Re = (float)(this.Re / modulus);
            this.Im = (float)(this.Im / modulus);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Convert to a from double precision complex number to a single precison complex number
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static explicit operator ComplexF(Complex c)
        {
            ComplexF cF;
            cF.Re = (float)c.Re;
            cF.Im = (float)c.Im;
            return cF;
        }

        /// <summary>
        /// Convert from a single precision real number to a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static explicit operator ComplexF(float f)
        {
            ComplexF c;
            c.Re = (float)f;
            c.Im = (float)0;
            return c;
        }

        /// <summary>
        /// Convert from a single precision complex to a real number
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static explicit operator float(ComplexF c)
        {
            return (float)c.Re;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Are these two complex numbers equivalent?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ComplexF a, ComplexF b)
        {
            return (a.Re == b.Re) && (a.Im == b.Im);
        }

        /// <summary>
        /// Are these two complex numbers different?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ComplexF a, ComplexF b)
        {
            return (a.Re != b.Re) || (a.Im != b.Im);
        }

        /// <summary>
        /// Get the hash code of the complex number
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Re.GetHashCode() ^ this.Im.GetHashCode());
        }

        /// <summary>
        /// Is this complex number equivalent to another object?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (o is ComplexF)
            {
                ComplexF c = (ComplexF)o;
                return (this == c);
            }
            return false;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Compare to other complex numbers or real numbers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(object o)
        {
            if (o == null)
            {
                return 1;  // null sorts before current
            }
            if (o is ComplexF)
            {
                return this.GetModulus().CompareTo(((ComplexF)o).GetModulus());
            }
            if (o is float)
            {
                return this.GetModulus().CompareTo((float)o);
            }
            if (o is Complex)
            {
                return this.GetModulus().CompareTo(((Complex)o).GetModulus());
            }
            if (o is double)
            {
                return this.GetModulus().CompareTo((double)o);
            }
            throw new ArgumentException();
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// This operator doesn't do much. :-)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ComplexF operator +(ComplexF a)
        {
            return a;
        }

        /// <summary>
        /// Negate the complex number
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ComplexF operator -(ComplexF a)
        {
            a.Re = -a.Re;
            a.Im = -a.Im;
            return a;
        }

        /// <summary>
        /// Add a complex number to a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexF operator +(ComplexF a, float f)
        {
            a.Re = (float)(a.Re + f);
            return a;
        }

        /// <summary>
        /// Add a real to a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ComplexF operator +(float f, ComplexF a)
        {
            a.Re = (float)(a.Re + f);
            return a;
        }

        /// <summary>
        /// Add to complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ComplexF operator +(ComplexF a, ComplexF b)
        {
            a.Re = a.Re + b.Re;
            a.Im = a.Im + b.Im;
            return a;
        }

        /// <summary>
        /// Subtract a real from a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexF operator -(ComplexF a, float f)
        {
            a.Re = (float)(a.Re - f);
            return a;
        }

        /// <summary>
        /// Subtract a complex number from a real
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ComplexF operator -(float f, ComplexF a)
        {
            a.Re = (float)(f - a.Re);
            a.Im = (float)(0 - a.Im);
            return a;
        }

        /// <summary>
        /// Subtract two complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ComplexF operator -(ComplexF a, ComplexF b)
        {
            a.Re = a.Re - b.Re;
            a.Im = a.Im - b.Im;
            return a;
        }

        /// <summary>
        /// Multiply a complex number by a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexF operator *(ComplexF a, float f)
        {
            a.Re = (float)(a.Re * f);
            a.Im = (float)(a.Im * f);
            return a;
        }

        /// <summary>
        /// Multiply a real by a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static ComplexF operator *(float f, ComplexF a)
        {
            a.Re = (float)(a.Re * f);
            a.Im = (float)(a.Im * f);
            return a;
        }

        /// <summary>
        /// Multiply two complex numbers together
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ComplexF operator *(ComplexF a, ComplexF b)
        {
            // (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;
            a.Re = (float)(x * u - y * v);
            a.Im = (float)(x * v + y * u);
            return a;
        }

        /// <summary>
        /// Divide a complex number by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexF operator /(ComplexF a, float f)
        {
            if (f == 0)
            {
                throw new DivideByZeroException();
            }
            a.Re = (float)(a.Re / f);
            a.Im = (float)(a.Im / f);
            return a;
        }

        /// <summary>
        /// Divide a complex number by a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ComplexF operator /(ComplexF a, ComplexF b)
        {
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;
            double denom = u * u + v * v;

            if (denom == 0)
            {
                throw new DivideByZeroException();
            }
            a.Re = (float)((x * u + y * v) / denom);
            a.Im = (float)((y * u - x * v) / denom);
            return a;
        }

        /// <summary>
        /// Parse a complex representation in this fashion: "( %f, %f )"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public ComplexF Parse(string s)
        {
            throw new NotImplementedException("ComplexF ComplexF.Parse( string s ) is not implemented.");
        }

        /// <summary>
        /// Get the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("( {0}, {1}i )", this.Re, this.Im);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Determine whether two complex numbers are almost (i.e. within the tolerance) equivalent.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        static public bool IsEqual(ComplexF a, ComplexF b, float tolerance)
        {
            return
                (Math.Abs(a.Re - b.Re) < tolerance) &&
                (Math.Abs(a.Im - b.Im) < tolerance);

        }

        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------

        /// <summary>
        /// Represents zero
        /// </summary>
        static public ComplexF Zero
        {
            get { return new ComplexF(0, 0); }
        }

        /// <summary>
        /// Represents the result of sqrt( -1 )
        /// </summary>
        static public ComplexF I
        {
            get { return new ComplexF(0, 1); }
        }

        /// <summary>
        /// Represents the largest possible value of ComplexF.
        /// </summary>
        static public ComplexF MaxValue
        {
            get { return new ComplexF(float.MaxValue, float.MaxValue); }
        }

        /// <summary>
        /// Represents the smallest possible value of ComplexF.
        /// </summary>
        static public ComplexF MinValue
        {
            get { return new ComplexF(float.MinValue, float.MinValue); }
        }


        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------
    }


    // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
    // Version: May 4, 2002

    /// <summary>
    /// <p>A double-precision complex number representation.</p>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex : IComparable, ICloneable
    {

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The real component of the complex number
        /// </summary>
        public double Re;

        /// <summary>
        /// The imaginary component of the complex number
        /// </summary>
        public double Im;

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        public Complex(double real, double imaginary)
        {
            this.Re = (double)real;
            this.Im = (double)imaginary;
        }

        /// <summary>
        /// Create a complex number based on an existing complex number
        /// </summary>
        /// <param name="c"></param>
        public Complex(Complex c)
        {
            this.Re = c.Re;
            this.Im = c.Im;
        }

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real"></param>
        /// <param name="imaginary"></param>
        /// <returns></returns>
        static public Complex FromRealImaginary(double real, double imaginary)
        {
            Complex c;
            c.Re = (double)real;
            c.Im = (double)imaginary;
            return c;
        }

        /// <summary>
        /// Create a complex number from a modulus (length) and an argument (radian)
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        static public Complex FromModulusArgument(double modulus, double argument)
        {
            Complex c;
            c.Re = (double)(modulus * System.Math.Cos(argument));
            c.Im = (double)(modulus * System.Math.Sin(argument));
            return c;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        object ICloneable.Clone()
        {
            return new Complex(this);
        }
        /// <summary>
        /// Clone the complex number
        /// </summary>
        /// <returns></returns>
        public Complex Clone()
        {
            return new Complex(this);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// The modulus (length) of the complex number
        /// </summary>
        /// <returns></returns>
        public double GetModulus()
        {
            double x = this.Re;
            double y = this.Im;
            return (double)Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// The squared modulus (length^2) of the complex number
        /// </summary>
        /// <returns></returns>
        public double GetModulusSquared()
        {
            double x = this.Re;
            double y = this.Im;
            return (double)x * x + y * y;
        }

        /// <summary>
        /// The argument (radians) of the complex number
        /// </summary>
        /// <returns></returns>
        public double GetArgument()
        {
            return (double)Math.Atan2(this.Im, this.Re);
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Get the conjugate of the complex number
        /// </summary>
        /// <returns></returns>
        public Complex GetConjugate()
        {
            return FromRealImaginary(this.Re, -this.Im);
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Scale the complex number to 1.
        /// </summary>
        public void Normalize()
        {
            double modulus = this.GetModulus();
            if (modulus == 0)
            {
                throw new DivideByZeroException("Can not normalize a complex number that is zero.");
            }
            this.Re = (double)(this.Re / modulus);
            this.Im = (double)(this.Im / modulus);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Convert to a from double precision complex number to a single precison complex number
        /// </summary>
        /// <param name="cF"></param>
        /// <returns></returns>
        public static explicit operator Complex(ComplexF cF)
        {
            Complex c;
            c.Re = (double)cF.Re;
            c.Im = (double)cF.Im;
            return c;
        }

        /// <summary>
        /// Convert from a single precision real number to a complex number
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static explicit operator Complex(double d)
        {
            Complex c;
            c.Re = (double)d;
            c.Im = (double)0;
            return c;
        }

        /// <summary>
        /// Convert from a single precision complex to a real number
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static explicit operator double(Complex c)
        {
            return (double)c.Re;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Are these two complex numbers equivalent?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Complex a, Complex b)
        {
            return (a.Re == b.Re) && (a.Im == b.Im);
        }

        /// <summary>
        /// Are these two complex numbers different?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Complex a, Complex b)
        {
            return (a.Re != b.Re) || (a.Im != b.Im);
        }

        /// <summary>
        /// Get the hash code of the complex number
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Re.GetHashCode() ^ this.Im.GetHashCode());
        }

        /// <summary>
        /// Is this complex number equivalent to another object?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (o is Complex)
            {
                Complex c = (Complex)o;
                return (this == c);
            }
            return false;
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Compare to other complex numbers or real numbers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(object o)
        {
            if (o == null)
            {
                return 1;  // null sorts before current
            }
            if (o is Complex)
            {
                return this.GetModulus().CompareTo(((Complex)o).GetModulus());
            }
            if (o is double)
            {
                return this.GetModulus().CompareTo((double)o);
            }
            if (o is ComplexF)
            {
                return this.GetModulus().CompareTo(((ComplexF)o).GetModulus());
            }
            if (o is float)
            {
                return this.GetModulus().CompareTo((float)o);
            }
            throw new ArgumentException();
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// This operator doesn't do much. :-)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a)
        {
            return a;
        }

        /// <summary>
        /// Negate the complex number
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a)
        {
            a.Re = -a.Re;
            a.Im = -a.Im;
            return a;
        }

        /// <summary>
        /// Add a complex number to a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, double f)
        {
            a.Re = (double)(a.Re + f);
            return a;
        }

        /// <summary>
        /// Add a real to a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator +(double f, Complex a)
        {
            a.Re = (double)(a.Re + f);
            return a;
        }

        /// <summary>
        /// Add to complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, Complex b)
        {
            a.Re = a.Re + b.Re;
            a.Im = a.Im + b.Im;
            return a;
        }

        /// <summary>
        /// Subtract a real from a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, double f)
        {
            a.Re = (double)(a.Re - f);
            return a;
        }

        /// <summary>
        /// Subtract a complex number from a real
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator -(double f, Complex a)
        {
            a.Re = (float)(f - a.Re);
            a.Im = (float)(0 - a.Im);
            return a;
        }

        /// <summary>
        /// Subtract two complex numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, Complex b)
        {
            a.Re = a.Re - b.Re;
            a.Im = a.Im - b.Im;
            return a;
        }

        /// <summary>
        /// Multiply a complex number by a real
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, double f)
        {
            a.Re = (double)(a.Re * f);
            a.Im = (double)(a.Im * f);
            return a;
        }

        /// <summary>
        /// Multiply a real by a complex number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator *(double f, Complex a)
        {
            a.Re = (double)(a.Re * f);
            a.Im = (double)(a.Im * f);

            return a;
        }

        /// <summary>
        /// Multiply two complex numbers together
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, Complex b)
        {
            // (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;

            a.Re = (double)(x * u - y * v);
            a.Im = (double)(x * v + y * u);

            return a;
        }

        /// <summary>
        /// Divide a complex number by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, double f)
        {
            if (f == 0)
            {
                throw new DivideByZeroException();
            }

            a.Re = (double)(a.Re / f);
            a.Im = (double)(a.Im / f);

            return a;
        }

        /// <summary>
        /// Divide a complex number by a complex number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, Complex b)
        {
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;
            double denom = u * u + v * v;

            if (denom == 0)
            {
                throw new DivideByZeroException();
            }

            a.Re = (double)((x * u + y * v) / denom);
            a.Im = (double)((y * u - x * v) / denom);

            return a;
        }

        /// <summary>
        /// Parse a complex representation in this fashion: "( %f, %f )"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public Complex Parse(string s)
        {
            throw new NotImplementedException("Complex Complex.Parse( string s ) is not implemented.");
        }

        /// <summary>
        /// Get the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("( {0}, {1}i )", this.Re, this.Im);
        }

        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Determine whether two complex numbers are almost (i.e. within the tolerance) equivalent.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        static public bool IsEqual(Complex a, Complex b, double tolerance)
        {
            return
                (Math.Abs(a.Re - b.Re) < tolerance) &&
                (Math.Abs(a.Im - b.Im) < tolerance);

        }

        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------

        /// <summary>
        /// Represents zero
        /// </summary>
        static public Complex Zero
        {
            get { return new Complex(0, 0); }
        }

        /// <summary>
        /// Represents the result of sqrt( -1 )
        /// </summary>
        static public Complex I
        {
            get { return new Complex(0, 1); }
        }

        /// <summary>
        /// Represents the largest possible value of Complex.
        /// </summary>
        static public Complex MaxValue
        {
            get { return new Complex(double.MaxValue, double.MaxValue); }
        }

        /// <summary>
        /// Represents the smallest possible value of Complex.
        /// </summary>
        static public Complex MinValue
        {
            get { return new Complex(double.MinValue, double.MinValue); }
        }


        //----------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------
    }

    // Comments? Questions? Bugs? Tell Ben Houston at ben@exocortex.org
    // Version: May 4, 2002

    /// <summary>
    /// <p>Static functions for doing various Fourier Operations.</p>
    /// </summary>
    public class Fourier
    {

        //======================================================================================

        private Fourier()
        {
        }

        //======================================================================================

        static private void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }
        static private void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }
        static private void Swap(ref ComplexF a, ref ComplexF b)
        {
            ComplexF temp = a;
            a = b;
            b = temp;
        }
        static private void Swap(ref Complex a, ref Complex b)
        {
            Complex temp = a;
            a = b;
            b = temp;
        }

        //-------------------------------------------------------------------------------------

        private const int cMaxLength = 4096;
        private const int cMinLength = 1;

        private const int cMaxBits = 12;
        private const int cMinBits = 0;


        static private bool IsPowerOf2(int x)
        {
            return (x & (x - 1)) == 0;
            //return	( x == Pow2( Log2( x ) ) );
        }
        static private int Pow2(int exponent)
        {
            if (exponent >= 0 && exponent < 31)
            {
                return 1 << exponent;
            }
            return 0;
        }
        static private int Log2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                {
                                    return 0;
                                }
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }
                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }
            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            //	since int is unsigned it can never be higher than 2,147,483,647
            //	if( x <= 2147483648 )
            //		return	31;	
            //	return	32;	
            return 31;
        }

        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------

        static private int ReverseBits(int index, int numberOfBits)
        {
            Debug.Assert(numberOfBits >= cMinBits);
            Debug.Assert(numberOfBits <= cMaxBits);

            int reversedIndex = 0;
            for (int i = 0; i < numberOfBits; i++)
            {
                reversedIndex = (reversedIndex << 1) | (index & 1);
                index = (index >> 1);
            }
            return reversedIndex;
        }

        //-------------------------------------------------------------------------------------

        static private int[][] _reversedBits = new int[cMaxBits][];
        static private int[] GetReversedBits(int numberOfBits)
        {
            Debug.Assert(numberOfBits >= cMinBits);
            Debug.Assert(numberOfBits <= cMaxBits);
            if (_reversedBits[numberOfBits - 1] == null)
            {
                int maxBits = Fourier.Pow2(numberOfBits);
                int[] reversedBits = new int[maxBits];
                for (int i = 0; i < maxBits; i++)
                {
                    int oldBits = i;
                    int newBits = 0;
                    for (int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits = (oldBits >> 1);
                    }
                    reversedBits[i] = newBits;
                }
                _reversedBits[numberOfBits - 1] = reversedBits;
            }
            return _reversedBits[numberOfBits - 1];
        }

        //-------------------------------------------------------------------------------------

        static private void ReorderArray(float[] data)
        {
            Debug.Assert(data != null);

            int length = data.Length / 2;

            Debug.Assert(Fourier.IsPowerOf2(length) == true);
            Debug.Assert(length >= cMinLength);
            Debug.Assert(length <= cMaxLength);

            int[] reversedBits = Fourier.GetReversedBits(Fourier.Log2(length));
            for (int i = 0; i < length; i++)
            {
                int swap = reversedBits[i];
                if (swap > i)
                {
                    Fourier.Swap(ref data[(i << 1)], ref data[(swap << 1)]);
                    Fourier.Swap(ref data[(i << 1) + 1], ref data[(swap << 1) + 1]);
                }
            }
        }

        static private void ReorderArray(double[] data)
        {
            Debug.Assert(data != null);

            int length = data.Length / 2;

            Debug.Assert(Fourier.IsPowerOf2(length) == true);
            Debug.Assert(length >= cMinLength);
            Debug.Assert(length <= cMaxLength);

            int[] reversedBits = Fourier.GetReversedBits(Fourier.Log2(length));
            for (int i = 0; i < length; i++)
            {
                int swap = reversedBits[i];
                if (swap > i)
                {
                    Fourier.Swap(ref data[i << 1], ref data[swap << 1]);
                    Fourier.Swap(ref data[i << 1 + 1], ref data[swap << 1 + 1]);
                }
            }
        }

        static private void ReorderArray(Complex[] data)
        {
            Debug.Assert(data != null);

            int length = data.Length;

            Debug.Assert(Fourier.IsPowerOf2(length) == true);
            Debug.Assert(length >= cMinLength);
            Debug.Assert(length <= cMaxLength);

            int[] reversedBits = Fourier.GetReversedBits(Fourier.Log2(length));
            for (int i = 0; i < length; i++)
            {
                int swap = reversedBits[i];
                if (swap > i)
                {
                    Complex temp = data[i];
                    data[i] = data[swap];
                    data[swap] = temp;
                }
            }
        }

        static private void ReorderArray(ComplexF[] data)
        {
            Debug.Assert(data != null);

            int length = data.Length;

            Debug.Assert(Fourier.IsPowerOf2(length) == true);
            Debug.Assert(length >= cMinLength);
            Debug.Assert(length <= cMaxLength);

            int[] reversedBits = Fourier.GetReversedBits(Fourier.Log2(length));
            for (int i = 0; i < length; i++)
            {
                int swap = reversedBits[i];
                if (swap > i)
                {
                    ComplexF temp = data[i];
                    data[i] = data[swap];
                    data[swap] = temp;
                }
            }
        }

        //======================================================================================

        private static int[][] _reverseBits = null;

        private static int _ReverseBits(int bits, int n)
        {
            int bitsReversed = 0;
            for (int i = 0; i < n; i++)
            {
                bitsReversed = (bitsReversed << 1) | (bits & 1);
                bits = (bits >> 1);
            }
            return bitsReversed;
        }

        private static void InitializeReverseBits(int levels)
        {
            _reverseBits = new int[levels + 1][];
            for (int j = 0; j < (levels + 1); j++)
            {
                int count = (int)Math.Pow(2, j);
                _reverseBits[j] = new int[count];
                for (int i = 0; i < count; i++)
                {
                    _reverseBits[j][i] = _ReverseBits(i, j);
                }
            }
        }

        private static int _lookupTabletLength = -1;
        private static double[,][] _uRLookup = null;
        private static double[,][] _uILookup = null;
        private static float[,][] _uRLookupF = null;
        private static float[,][] _uILookupF = null;

        private static void SyncLookupTableLength(int length)
        {
            Debug.Assert(length < 1024 * 10);
            Debug.Assert(length >= 0);
            if (length > _lookupTabletLength)
            {
                int level = (int)Math.Ceiling(Math.Log(length, 2));
                Fourier.InitializeReverseBits(level);
                Fourier.InitializeComplexRotations(level);
                //_cFFTData	= new Complex[ Math2.CeilingBase( length, 2 ) ];
                //_cFFTDataF	= new ComplexF[ Math2.CeilingBase( length, 2 ) ];
                _lookupTabletLength = length;
            }
        }

        private static int GetLookupTableLength()
        {
            return _lookupTabletLength;
        }

        private static void ClearLookupTables()
        {
            _uRLookup = null;
            _uILookup = null;
            _uRLookupF = null;
            _uILookupF = null;
            _lookupTabletLength = -1;
        }

        private static void InitializeComplexRotations(int levels)
        {
            int ln = levels;
            //_wRLookup = new float[ levels + 1, 2 ];
            //_wILookup = new float[ levels + 1, 2 ];

            _uRLookup = new double[levels + 1, 2][];
            _uILookup = new double[levels + 1, 2][];

            _uRLookupF = new float[levels + 1, 2][];
            _uILookupF = new float[levels + 1, 2][];

            int N = 1;
            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                //float scale = (float)( 1 / Math.Sqrt( 1 << ln ) );

                // positive sign ( i.e. [M,0] )
                {
                    double uR = 1;
                    double uI = 0;
                    double angle = (double)Math.PI / M * 1;
                    double wR = (double)Math.Cos(angle);
                    double wI = (double)Math.Sin(angle);

                    _uRLookup[level, 0] = new double[M];
                    _uILookup[level, 0] = new double[M];
                    _uRLookupF[level, 0] = new float[M];
                    _uILookupF[level, 0] = new float[M];

                    for (int j = 0; j < M; j++)
                    {
                        _uRLookupF[level, 0][j] = (float)(_uRLookup[level, 0][j] = uR);
                        _uILookupF[level, 0][j] = (float)(_uILookup[level, 0][j] = uI);
                        double uwI = uR * wI + uI * wR;
                        uR = uR * wR - uI * wI;
                        uI = uwI;
                    }
                }
                {


                    // negative sign ( i.e. [M,1] )
                    double uR = 1;
                    double uI = 0;
                    double angle = (double)Math.PI / M * -1;
                    double wR = (double)Math.Cos(angle);
                    double wI = (double)Math.Sin(angle);

                    _uRLookup[level, 1] = new double[M];
                    _uILookup[level, 1] = new double[M];
                    _uRLookupF[level, 1] = new float[M];
                    _uILookupF[level, 1] = new float[M];

                    for (int j = 0; j < M; j++)
                    {
                        _uRLookupF[level, 1][j] = (float)(_uRLookup[level, 1][j] = uR);
                        _uILookupF[level, 1][j] = (float)(_uILookup[level, 1][j] = uI);
                        double uwI = uR * wI + uI * wR;
                        uR = uR * wR - uI * wI;
                        uI = uwI;
                    }
                }

            }
        }

        //======================================================================================
        //======================================================================================

        static private bool _bufferFLocked = false;
        static private float[] _bufferF = new float[0];

        static private void LockBufferF(int length, ref float[] buffer)
        {
            Debug.Assert(_bufferFLocked == false);
            _bufferFLocked = true;
            if (length >= _bufferF.Length)
            {
                _bufferF = new float[length];
            }
            buffer = _bufferF;
        }
        static private void UnlockBufferF(ref float[] buffer)
        {
            Debug.Assert(_bufferF == buffer);
            Debug.Assert(_bufferFLocked == true);
            _bufferFLocked = false;
            buffer = null;
        }

        private static void LinearFFT(float[] data, int start, int inc, int length, FourierDirection direction)
        {
            Debug.Assert(data != null);
            Debug.Assert(start >= 0);
            Debug.Assert(inc >= 1);
            Debug.Assert(length >= 1);
            Debug.Assert((start + inc * (length - 1)) * 2 < data.Length);

            // copy to buffer
            float[] buffer = null;
            LockBufferF(length * 2, ref buffer);
            int j = start;
            for (int i = 0; i < length * 2; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferF(ref buffer);
        }

        private static void LinearFFT_Quick(float[] data, int start, int inc, int length, FourierDirection direction)
        {
            /*Debug.Assert( data != null );
            Debug.Assert( start >= 0 );
            Debug.Assert( inc >= 1 );
            Debug.Assert( length >= 1 );
            Debug.Assert( ( start + inc * ( length - 1 ) ) * 2 < data.Length );*/

            // copy to buffer
            float[] buffer = null;
            LockBufferF(length * 2, ref buffer);
            int j = start;
            for (int i = 0; i < length * 2; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT_Quick(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferF(ref buffer);
        }

        //======================================================================================
        //======================================================================================

        static private bool _bufferCFLocked = false;
        static private ComplexF[] _bufferCF = new ComplexF[0];

        static private void LockBufferCF(int length, ref ComplexF[] buffer)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(_bufferCFLocked == false);

            _bufferCFLocked = true;
            if (length != _bufferCF.Length)
            {
                _bufferCF = new ComplexF[length];
            }
            buffer = _bufferCF;
        }
        static private void UnlockBufferCF(ref ComplexF[] buffer)
        {
            Debug.Assert(_bufferCF == buffer);
            Debug.Assert(_bufferCFLocked == true);

            _bufferCFLocked = false;
            buffer = null;
        }

        private static void LinearFFT(ComplexF[] data, int start, int inc, int length, FourierDirection direction)
        {
            Debug.Assert(data != null);
            Debug.Assert(start >= 0);
            Debug.Assert(inc >= 1);
            Debug.Assert(length >= 1);
            Debug.Assert((start + inc * (length - 1)) < data.Length);

            // copy to buffer
            ComplexF[] buffer = null;
            LockBufferCF(length, ref buffer);
            int j = start;
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferCF(ref buffer);
        }

        private static void LinearFFT_Quick(ComplexF[] data, int start, int inc, int length, FourierDirection direction)
        {
            /*Debug.Assert( data != null );
            Debug.Assert( start >= 0 );
            Debug.Assert( inc >= 1 );
            Debug.Assert( length >= 1 );
            Debug.Assert( ( start + inc * ( length - 1 ) ) < data.Length );	*/

            // copy to buffer
            ComplexF[] buffer = null;
            LockBufferCF(length, ref buffer);
            int j = start;
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferCF(ref buffer);
        }

        //======================================================================================
        //======================================================================================

        static private bool _bufferCLocked = false;
        static private Complex[] _bufferC = new Complex[0];

        static private void LockBufferC(int length, ref Complex[] buffer)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(_bufferCLocked == false);

            _bufferCLocked = true;
            if (length >= _bufferC.Length)
            {
                _bufferC = new Complex[length];
            }
            buffer = _bufferC;
        }
        static private void UnlockBufferC(ref Complex[] buffer)
        {
            Debug.Assert(_bufferC == buffer);
            Debug.Assert(_bufferCLocked == true);

            _bufferCLocked = false;
            buffer = null;
        }

        private static void LinearFFT(Complex[] data, int start, int inc, int length, FourierDirection direction)
        {
            Debug.Assert(data != null);
            Debug.Assert(start >= 0);
            Debug.Assert(inc >= 1);
            Debug.Assert(length >= 1);
            Debug.Assert((start + inc * (length - 1)) < data.Length);

            // copy to buffer
            Complex[] buffer = null;
            LockBufferC(length, ref buffer);
            int j = start;
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferC(ref buffer);
        }

        private static void LinearFFT_Quick(Complex[] data, int start, int inc, int length, FourierDirection direction)
        {
            /*Debug.Assert( data != null );
            Debug.Assert( start >= 0 );
            Debug.Assert( inc >= 1 );
            Debug.Assert( length >= 1 );
            Debug.Assert( ( start + inc * ( length - 1 ) ) < data.Length );*/

            // copy to buffer
            Complex[] buffer = null;
            LockBufferC(length, ref buffer);
            int j = start;
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[j];
                j += inc;
            }

            FFT_Quick(buffer, length, direction);

            // copy from buffer
            j = start;
            for (int i = 0; i < length; i++)
            {
                data[j] = buffer[i];
                j += inc;
            }
            UnlockBufferC(ref buffer);
        }

        //======================================================================================
        //======================================================================================

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers (as pairs of float's).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT(float[] data, int length, FourierDirection direction)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length >= length * 2);
            Debug.Assert(Fourier.IsPowerOf2(length) == true);

            Fourier.SyncLookupTableLength(length);

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;
            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                float[] uRLookup = _uRLookupF[level, signIndex];
                float[] uILookup = _uILookupF[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    float uR = uRLookup[j];
                    float uI = uILookup[j];

                    for (int evenT = j; evenT < length; evenT += N)
                    {
                        int even = evenT << 1;
                        int odd = (evenT + M) << 1;

                        float r = data[odd];
                        float i = data[odd + 1];

                        float odduR = r * uR - i * uI;
                        float odduI = r * uI + i * uR;

                        r = data[even];
                        i = data[even + 1];

                        data[even] = r + odduR;
                        data[even + 1] = i + odduI;

                        data[odd] = r - odduR;
                        data[odd + 1] = i - odduI;
                    }
                }
            }
        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers (as pairs of float's).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT_Quick(float[] data, int length, FourierDirection direction)
        {
            /*Debug.Assert( data != null );
            Debug.Assert( data.Length >= length*2 );
            Debug.Assert( Fourier.IsPowerOf2( length ) == true );

            Fourier.SyncLookupTableLength( length );*/

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;
            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                float[] uRLookup = _uRLookupF[level, signIndex];
                float[] uILookup = _uILookupF[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    float uR = uRLookup[j];
                    float uI = uILookup[j];

                    for (int evenT = j; evenT < length; evenT += N)
                    {
                        int even = evenT << 1;
                        int odd = (evenT + M) << 1;

                        float r = data[odd];
                        float i = data[odd + 1];

                        float odduR = r * uR - i * uI;
                        float odduI = r * uI + i * uR;

                        r = data[even];
                        i = data[even + 1];

                        data[even] = r + odduR;
                        data[even + 1] = i + odduI;

                        data[odd] = r - odduR;
                        data[odd + 1] = i - odduI;
                    }
                }
            }
        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT(ComplexF[] data, int length, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < length)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be at least as large as 'data.Length' parameter");
            }
            if (Fourier.IsPowerOf2(length) == false)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be a power of 2");
            }

            Fourier.SyncLookupTableLength(length);

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;

            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                float[] uRLookup = _uRLookupF[level, signIndex];
                float[] uILookup = _uILookupF[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    float uR = uRLookup[j];
                    float uI = uILookup[j];

                    for (int even = j; even < length; even += N)
                    {
                        int odd = even + M;

                        float r = data[odd].Re;
                        float i = data[odd].Im;

                        float odduR = r * uR - i * uI;
                        float odduI = r * uI + i * uR;

                        r = data[even].Re;
                        i = data[even].Im;

                        data[even].Re = r + odduR;
                        data[even].Im = i + odduI;

                        data[odd].Re = r - odduR;
                        data[odd].Im = i - odduI;
                    }
                }
            }

        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT_Quick(ComplexF[] data, int length, FourierDirection direction)
        {
            /*if( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            if( data.Length < length ) {
                throw new ArgumentOutOfRangeException( "length", length, "must be at least as large as 'data.Length' parameter" );
            }
            if( Fourier.IsPowerOf2( length ) == false ) {
                throw new ArgumentOutOfRangeException( "length", length, "must be a power of 2" );
            }

            Fourier.SyncLookupTableLength( length );*/

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;

            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                float[] uRLookup = _uRLookupF[level, signIndex];
                float[] uILookup = _uILookupF[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    float uR = uRLookup[j];
                    float uI = uILookup[j];

                    for (int even = j; even < length; even += N)
                    {
                        int odd = even + M;

                        float r = data[odd].Re;
                        float i = data[odd].Im;

                        float odduR = r * uR - i * uI;
                        float odduI = r * uI + i * uR;

                        r = data[even].Re;
                        i = data[even].Im;

                        data[even].Re = r + odduR;
                        data[even].Im = i + odduI;

                        data[odd].Re = r - odduR;
                        data[odd].Im = i - odduI;
                    }
                }
            }

        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void FFT(ComplexF[] data, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Fourier.FFT(data, data.Length, direction);
        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT(Complex[] data, int length, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < length)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be at least as large as 'data.Length' parameter");
            }
            if (Fourier.IsPowerOf2(length) == false)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be a power of 2");
            }

            Fourier.SyncLookupTableLength(length);

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;

            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                double[] uRLookup = _uRLookup[level, signIndex];
                double[] uILookup = _uILookup[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    double uR = uRLookup[j];
                    double uI = uILookup[j];

                    for (int even = j; even < length; even += N)
                    {
                        int odd = even + M;

                        double r = data[odd].Re;
                        double i = data[odd].Im;

                        double odduR = r * uR - i * uI;
                        double odduI = r * uI + i * uR;

                        r = data[even].Re;
                        i = data[even].Im;

                        data[even].Re = r + odduR;
                        data[even].Im = i + odduI;

                        data[odd].Re = r - odduR;
                        data[odd].Im = i - odduI;
                    }
                }
            }

        }

        /// <summary>
        /// Compute a 1D fast Fourier transform of a dataset of complex numbers.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void FFT_Quick(Complex[] data, int length, FourierDirection direction)
        {
            /*if( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            if( data.Length < length ) {
                throw new ArgumentOutOfRangeException( "length", length, "must be at least as large as 'data.Length' parameter" );
            }
            if( Fourier.IsPowerOf2( length ) == false ) {
                throw new ArgumentOutOfRangeException( "length", length, "must be a power of 2" );
            }

            Fourier.SyncLookupTableLength( length );   */

            int ln = Fourier.Log2(length);

            // reorder array
            Fourier.ReorderArray(data);

            // successive doubling
            int N = 1;
            int signIndex = (direction == FourierDirection.Forward) ? 0 : 1;

            for (int level = 1; level <= ln; level++)
            {
                int M = N;
                N <<= 1;

                double[] uRLookup = _uRLookup[level, signIndex];
                double[] uILookup = _uILookup[level, signIndex];

                for (int j = 0; j < M; j++)
                {
                    double uR = uRLookup[j];
                    double uI = uILookup[j];

                    for (int even = j; even < length; even += N)
                    {
                        int odd = even + M;

                        double r = data[odd].Re;
                        double i = data[odd].Im;

                        double odduR = r * uR - i * uI;
                        double odduI = r * uI + i * uR;

                        r = data[even].Re;
                        i = data[even].Im;

                        data[even].Re = r + odduR;
                        data[even].Im = i + odduI;

                        data[odd].Re = r - odduR;
                        data[odd].Im = i - odduI;
                    }
                }
            }

        }

        /// <summary>
        /// Compute a 1D real-symmetric fast fourier transform.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void RFFT(float[] data, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Fourier.RFFT(data, data.Length, direction);
        }

        /// <summary>
        /// Compute a 1D real-symmetric fast fourier transform.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="direction"></param>
        public static void RFFT(float[] data, int length, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < length)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be at least as large as 'data.Length' parameter");
            }
            if (Fourier.IsPowerOf2(length) == false)
            {
                throw new ArgumentOutOfRangeException("length", length, "must be a power of 2");
            }

            float c1 = 0.5f, c2;
            float theta = (float)Math.PI / (length / 2);

            if (direction == FourierDirection.Forward)
            {
                c2 = -0.5f;
                FFT(data, length / 2, direction);
            }
            else
            {
                c2 = 0.5f;
                theta = -theta;
            }

            float wtemp = (float)Math.Sin(0.5 * theta);
            float wpr = -2 * wtemp * wtemp;
            float wpi = (float)Math.Sin(theta);
            float wr = 1 + wpr;
            float wi = wpi;

            // do / undo packing
            for (int i = 1; i < length / 4; i++)
            {
                int a = 2 * i;
                int b = length - 2 * i;
                float h1r = c1 * (data[a] + data[b]);
                float h1i = c1 * (data[a + 1] - data[b + 1]);
                float h2r = -c2 * (data[a + 1] + data[b + 1]);
                float h2i = c2 * (data[a] - data[b]);
                data[a] = h1r + wr * h2r - wi * h2i;
                data[a + 1] = h1i + wr * h2i + wi * h2r;
                data[b] = h1r - wr * h2r + wi * h2i;
                data[b + 1] = -h1i + wr * h2i + wi * h2r;
                wr = (wtemp = wr) * wpr - wi * wpi + wr;
                wi = wi * wpr + wtemp * wpi + wi;
            }

            if (direction == FourierDirection.Forward)
            {
                float hir = data[0];
                data[0] = hir + data[1];
                data[1] = hir - data[1];
            }
            else
            {
                float hir = data[0];
                data[0] = c1 * (hir + data[1]);
                data[1] = c1 * (hir - data[1]);
                Fourier.FFT(data, length / 2, direction);
            }
        }

        /// <summary>
        /// Compute a 2D fast fourier transform on a data set of complex numbers (represented as pairs of floats)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="direction"></param>
        public static void FFT2(float[] data, int xLength, int yLength, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < xLength * yLength * 2)
            {
                throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength * 2' parameter");
            }
            if (Fourier.IsPowerOf2(xLength) == false)
            {
                throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(yLength) == false)
            {
                throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
            }

            int xInc = 1;
            int yInc = xLength;

            if (xLength > 1)
            {
                Fourier.SyncLookupTableLength(xLength);
                for (int y = 0; y < yLength; y++)
                {
                    int xStart = y * yInc;
                    Fourier.LinearFFT_Quick(data, xStart, xInc, xLength, direction);
                }
            }

            if (yLength > 1)
            {
                Fourier.SyncLookupTableLength(yLength);
                for (int x = 0; x < xLength; x++)
                {
                    int yStart = x * xInc;
                    Fourier.LinearFFT_Quick(data, yStart, yInc, yLength, direction);
                }
            }
        }

        /// <summary>
        /// Compute a 2D fast fourier transform on a data set of complex numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="direction"></param>
        public static void FFT2(ComplexF[] data, int xLength, int yLength, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < xLength * yLength)
            {
                throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength' parameter");
            }
            if (Fourier.IsPowerOf2(xLength) == false)
            {
                throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(yLength) == false)
            {
                throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
            }

            int xInc = 1;
            int yInc = xLength;

            if (xLength > 1)
            {
                Fourier.SyncLookupTableLength(xLength);
                for (int y = 0; y < yLength; y++)
                {
                    int xStart = y * yInc;
                    Fourier.LinearFFT_Quick(data, xStart, xInc, xLength, direction);
                }
            }

            if (yLength > 1)
            {
                Fourier.SyncLookupTableLength(yLength);
                for (int x = 0; x < xLength; x++)
                {
                    int yStart = x * xInc;
                    Fourier.LinearFFT_Quick(data, yStart, yInc, yLength, direction);
                }
            }
        }

        /// <summary>
        /// Compute a 2D fast fourier transform on a data set of complex numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="direction"></param>
        public static void FFT2(Complex[] data, int xLength, int yLength, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < xLength * yLength)
            {
                throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength' parameter");
            }
            if (Fourier.IsPowerOf2(xLength) == false)
            {
                throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(yLength) == false)
            {
                throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
            }

            int xInc = 1;
            int yInc = xLength;

            if (xLength > 1)
            {
                Fourier.SyncLookupTableLength(xLength);
                for (int y = 0; y < yLength; y++)
                {
                    int xStart = y * yInc;
                    Fourier.LinearFFT_Quick(data, xStart, xInc, xLength, direction);
                }
            }

            if (yLength > 1)
            {
                Fourier.SyncLookupTableLength(yLength);
                for (int x = 0; x < xLength; x++)
                {
                    int yStart = x * xInc;
                    Fourier.LinearFFT_Quick(data, yStart, yInc, yLength, direction);
                }
            }
        }

        /// <summary>
        /// Compute a 3D fast fourier transform on a data set of complex numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        /// <param name="direction"></param>
        public static void FFT3(ComplexF[] data, int xLength, int yLength, int zLength, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < xLength * yLength * zLength)
            {
                throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength * zLength' parameter");
            }
            if (Fourier.IsPowerOf2(xLength) == false)
            {
                throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(yLength) == false)
            {
                throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(zLength) == false)
            {
                throw new ArgumentOutOfRangeException("zLength", zLength, "must be a power of 2");
            }

            int xInc = 1;
            int yInc = xLength;
            int zInc = xLength * yLength;

            if (xLength > 1)
            {
                Fourier.SyncLookupTableLength(xLength);
                for (int z = 0; z < zLength; z++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        int xStart = y * yInc + z * zInc;
                        Fourier.LinearFFT_Quick(data, xStart, xInc, xLength, direction);
                    }
                }
            }

            if (yLength > 1)
            {
                Fourier.SyncLookupTableLength(yLength);
                for (int z = 0; z < zLength; z++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        int yStart = z * zInc + x * xInc;
                        Fourier.LinearFFT_Quick(data, yStart, yInc, yLength, direction);
                    }
                }
            }

            if (zLength > 1)
            {
                Fourier.SyncLookupTableLength(zLength);
                for (int y = 0; y < yLength; y++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        int zStart = y * yInc + x * xInc;
                        Fourier.LinearFFT_Quick(data, zStart, zInc, zLength, direction);
                    }
                }
            }
        }

        /// <summary>
        /// Compute a 3D fast fourier transform on a data set of complex numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        /// <param name="zLength"></param>
        /// <param name="direction"></param>
        public static void FFT3(Complex[] data, int xLength, int yLength, int zLength, FourierDirection direction)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < xLength * yLength * zLength)
            {
                throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength * zLength' parameter");
            }
            if (Fourier.IsPowerOf2(xLength) == false)
            {
                throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(yLength) == false)
            {
                throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
            }
            if (Fourier.IsPowerOf2(zLength) == false)
            {
                throw new ArgumentOutOfRangeException("zLength", zLength, "must be a power of 2");
            }

            int xInc = 1;
            int yInc = xLength;
            int zInc = xLength * yLength;

            if (xLength > 1)
            {
                Fourier.SyncLookupTableLength(xLength);
                for (int z = 0; z < zLength; z++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        int xStart = y * yInc + z * zInc;
                        Fourier.LinearFFT_Quick(data, xStart, xInc, xLength, direction);
                    }
                }
            }

            if (yLength > 1)
            {
                Fourier.SyncLookupTableLength(yLength);
                for (int z = 0; z < zLength; z++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        int yStart = z * zInc + x * xInc;
                        Fourier.LinearFFT_Quick(data, yStart, yInc, yLength, direction);
                    }
                }
            }

            if (zLength > 1)
            {
                Fourier.SyncLookupTableLength(zLength);
                for (int y = 0; y < yLength; y++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        int zStart = y * yInc + x * xInc;
                        Fourier.LinearFFT_Quick(data, zStart, zInc, zLength, direction);
                    }
                }
            }
        }

    }
}
