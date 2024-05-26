namespace Kursova2023_2024.Classes.Tool_classes
{
    public class Pair<TFirst, TSecond>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }
        public DateTime time { get; set; }

        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public Pair(TFirst first, TSecond second, DateTime timeTemp)
        {
            First = first;
            Second = second;
            time = timeTemp;
        }
    }
}
