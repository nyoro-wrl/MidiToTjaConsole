using Fractions;
using NAudio.Midi;

namespace MidiTjaConsole
{
    internal static class Extensions
    {
        public static Fraction GetTimeSignature(this TimeSignatureEvent timeSignatureEvent)
        {
            var numerator = timeSignatureEvent.Numerator;
            var denominator = (int)Math.Pow(2, timeSignatureEvent.Denominator);
            while (denominator < 4)
            {
                numerator *= 2;
                denominator *= 2;
            }
            return new Fraction(numerator, denominator, false);
        }
    }
}
