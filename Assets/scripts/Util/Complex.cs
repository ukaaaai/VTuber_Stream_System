namespace Util
{
    public readonly struct Complex
    {
        public readonly float Real;
        public readonly float Imaginary;
        
        public Complex(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        private unsafe float Invsqrt()
        {
            var x = Real * Real +Imaginary * Imaginary;

            var buf = *(long*)&x;
            buf = 0x5F3759DF - (buf >> 1);
            var y = *(float*)&buf;

            y *= 1.5f - x * 0.5f * y * y;
            return y;
        }

        public float Magnitude => 1 / Invsqrt();
        public float SqrMagnitude => Real * Real + Imaginary * Imaginary;
        
        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }
        
        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }
        
        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary, a.Real * b.Imaginary + a.Imaginary * b.Real);
        }
        
        public static Complex operator *(Complex a, float b)
        {
            return new Complex(a.Real * b, a.Imaginary * b);
        }
        
        public static Complex operator *(float a, Complex b)
        {
            return new Complex(a * b.Real, a * b.Imaginary);
        }
        
        public static Complex operator /(Complex a, float b)
        {
            return new Complex(a.Real / b, a.Imaginary / b);
        }
        
        public static Complex operator /(float a, Complex b)
        {
            return new Complex(a * b.Real, a * b.Imaginary);
        }
        
        public static Complex operator /(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real + a.Imaginary * b.Imaginary, a.Imaginary * b.Real - a.Real * b.Imaginary) / (b.Real * b.Real + b.Imaginary * b.Imaginary);
        }
        
        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Real, -a.Imaginary);
        }
        
        public static Complex operator ~(Complex a)
        {
            return new Complex(a.Real, -a.Imaginary);
        }
        
        public static Complex operator ++(Complex a)
        {
            return new Complex(a.Real + 1, a.Imaginary);
        }
        
        public static Complex operator --(Complex a)
        {
            return new Complex(a.Real - 1, a.Imaginary);
        }
    }
}