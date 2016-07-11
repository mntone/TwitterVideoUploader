namespace Mntone.TwitterVideoUploader.Core.FormatChecker.Units
{
	public struct Fraction
	{
		public int Numerator { get; }
		public int Denominator { get; }

		public Fraction(int numerator, int denominator)
		{
			this.Numerator = numerator;
			this.Denominator = denominator;
		}

		public double ToDouble() => (double)this.Denominator / this.Numerator;

		public static Fraction FromUInt64(long fraction)
		{
			var numerator = (int)(0xffff & fraction);
			var denominator = (int)(0xffff & (fraction >> 32));
			return new Fraction(numerator, denominator);
		}
	}
}