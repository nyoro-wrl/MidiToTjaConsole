using Fractions;
using NAudio.Midi;
using System.Text;

namespace MidiTjaConsole
{
    internal class Measure(long start, long end)
    {
        private long StartTime { get; } = start;
        private long EndTime { get; } = end;
        private long Elapsed { get; } = end - start;

        private Fraction? timeSignature = null;
        private readonly SortedDictionary<Fraction, double> bpms = [];

        public string ToTJA()
        {
            var builder = new StringBuilder();

            var denominators = new List<int>() { 1 };
            denominators.AddRange(bpms.Keys.Select(x => (int)x.Denominator));
            denominators.Distinct();
            var denominator = MyMath.LCM(denominators);
            var step = new Fraction(1, denominator);

            if (timeSignature != null)
            {
                builder.AppendLine($"#MEASURE {timeSignature:n}/{timeSignature:d}");
            }

            for (Fraction position = 0; position < 1; position += step)
            {
                if (bpms.TryGetValue(position, out var bpm))
                {
                    if (position != 0)
                    {
                        builder.AppendLine();
                    }
                    builder.AppendLine($"#BPMCHANGE {bpm}");
                }
                builder.Append('0');
            }
            builder.Append(',');
            return builder.ToString();
        }

        public void AddTempo(TempoEvent tempoEvent)
        {
            var bpm = Math.Round(tempoEvent.Tempo, 3);
            var tempoElapsed = tempoEvent.AbsoluteTime - StartTime;
            if (Program.TimeCorrection)
            {
                tempoElapsed = (long)(Math.Round(tempoElapsed / 10.0) * 10);
            }
            var position = new Fraction(tempoElapsed, Elapsed);
            bpms.Add(position, bpm);
        }

        public void SetTimeSignature(TimeSignatureEvent timeSignatureEvent)
        {
            var timeSignature = timeSignatureEvent.GetTimeSignature();
            this.timeSignature = timeSignature;
        }

        /// <summary>
        /// 時間がこの小節内かどうか
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool OnTime(long time)
        {
            return StartTime <= time && time < EndTime;
        }
    }
}
