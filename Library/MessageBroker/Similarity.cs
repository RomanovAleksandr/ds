namespace Library
{
    public class Similarity
    {
        public string Id { get; set; }
        public int Value { get; set; }

        public Similarity(string id, int value) 
        {
            Id = id;
            Value = value;
        }
    }
}