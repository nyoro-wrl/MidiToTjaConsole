using Fractions;
using NAudio.Midi;
using System.Text;

namespace MidiTjaConsole
{
    public static class Program
    {
        /// <summary>
        /// 1拍あたりのティック数
        /// </summary>
        private const int BeatTick = 480;

        /// <summary>
        /// イベントの僅かなズレを補正
        /// </summary>
        public static bool TimeCorrection { get; set; } = true;

        public static void Main(string[] args)
        {
            var unti1 = MyMath.LCM([3, 4]);
            var unti2 = MyMath.LCM([3]);
            var unti3 = MyMath.LCM([]);

            var path = GetPath(args);
            ReadMidi(path);
        }

        private static string GetPath(string[] args)
        {
            string path = "";
            if (path == "" && args.Length > 0)
            {
                path = args[0];
            }
            while (path == "")
            {
                Console.WriteLine("Input Path.");
                path = Console.ReadLine() ?? "";
            }
            path = path.Trim('"', '\'');
            return path;
        }

        private static void ReadMidi(string path)
        {
            using var stream = File.OpenRead(path);
            var midiFile = new MidiFile(stream, false);
            var tempoTrack = midiFile.Events.GetTrackEvents(0);
            var timeSignatureEvents = tempoTrack.OfType<TimeSignatureEvent>();
            var tempoEvents = tempoTrack.OfType<TempoEvent>();
            var endAbsoluteTime = tempoTrack.Max(x => x.AbsoluteTime);
            var measures = GetMeasure(timeSignatureEvents, endAbsoluteTime);

            foreach (var measure in measures)
            {
                var measureTempoEvents = tempoEvents.Where(x => measure.OnTime(x.AbsoluteTime));
                measure.AddTempo(measureTempoEvents);
            }

            var builder = new StringBuilder();
            foreach (var measure in measures)
            {
                var measureLine = measure.ToTJA();
                Console.WriteLine(measureLine);
            }
            Console.WriteLine(builder.ToString());
        }

        /// <summary>
        /// 小節毎の時間を取得する
        /// </summary>
        /// <param name="timeSignatureEvents"></param>
        /// <param name="endAbsoluteTime"></param>
        /// <returns></returns>
        private static List<Measure> GetMeasure(IEnumerable<TimeSignatureEvent> timeSignatureEvents, long endAbsoluteTime)
        {
            var measureTimes = new List<Measure>();
            var nowAbsoluteTime = 0L;
            while (nowAbsoluteTime <= endAbsoluteTime)
            {
                var timeSignatureEvent = timeSignatureEvents.First(x => x.AbsoluteTime <= nowAbsoluteTime);
                var timeSignature = timeSignatureEvent.GetTimeSignature();
                var elapsed = (long)(BeatTick * 4 * timeSignature);
                var end = nowAbsoluteTime + elapsed;
                measureTimes.Add(new(nowAbsoluteTime, end));
                nowAbsoluteTime = end;
            }
            return measureTimes;
        }

        private static Fraction GetTimeSignature(this TimeSignatureEvent timeSignatureEvent)
        {
            return new Fraction(timeSignatureEvent.Numerator, (int)Math.Pow(2, timeSignatureEvent.Denominator));
        }
    }
}