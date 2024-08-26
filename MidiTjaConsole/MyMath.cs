namespace MidiTjaConsole
{
    internal class MyMath
    {
        public static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        public static int LCM(int a, int b)
        {
            return a / GCD(a, b) * b;
        }

        public static int LCM(IEnumerable<int> numbers)
        {
            if (!numbers.Any())
            {
                return 0;
            }
            return numbers.Aggregate(LCM);
        }
    }
}
