namespace TESTS_PI2
{
    public class DataImport
    {
        public static List<Entry> ImportPriceData(String fileName)
        {
            List<Entry> entries = new List<Entry>();
            // Read a csv file and store the rows in a list of Entry objects
            // The csv file is assumed to be in the following format:
            // source,symbol,timestamp,bid,ask
            // The first line is assumed to be a header
            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null || line == "")
                    {
                        continue;
                    }
                    else
                    {
                        var values = line.Split(',');
                        entries.Add(new Entry(values[0], values[1], DateTime.Parse(values[2]), double.Parse(values[3]), double.Parse(values[4])));
                    }
                }
            }
            entries.Sort((x, y) => x.Timestamp().CompareTo(y.Timestamp()));
            return entries;
        }

        public static List<Entry> ExtractData(List<Entry> entries, DateTime start, int duration)
        {
            //probably a very inefficient way to do this depending on the actual implementation of these functions, but can be improved by taking into account the fact that the list is sorted
            List<Entry> FilteredEntries = entries.Where(x => x.Timestamp() >= start && x.Timestamp() <= (start.AddSeconds(duration))).ToList();
            return FilteredEntries;
        }

        
    }
}
