namespace TESTS_PI2
{
    public class Entry
    {
        private string source;
        private string symbol;
        private DateTime timestamp;
        private double bid;
        private double ask;
        private double mid;

        public Entry()
        {
            source = "";
            symbol = "";
            timestamp = DateTime.Now;
            bid = 0;
            ask = 0;
            mid = 0;
        }

        public Entry(string source, string symbol, DateTime timestamp, double bid, double ask)
        {
            this.source = source;
            this.symbol = symbol;
            this.timestamp = timestamp;
            this.bid = bid;
            this.ask = ask;
            this.mid = bid + ask / 2;
        }

        public string Source() { return source; }
        public string Symbol() { return symbol; }
        public DateTime Timestamp() { return timestamp; }
        public double Bid() { return bid; }
        public double Ask() { return ask; }
        public double Mid() { return mid; }
        public override string ToString()
        {
            return symbol + " " + timestamp.ToString() + " " + bid.ToString() + " " + ask.ToString();
        }
    }
}
