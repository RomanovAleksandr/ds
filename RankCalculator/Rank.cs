namespace Valuator
{
    public class Rank
    {
        public string Id { get; set; }
        public double Value { get; set; }

        public Rank(string id, double value) 
        {
            Id = id;
            Value = value;
        }
    }
}