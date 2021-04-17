using System.Collections.Generic;

namespace Library
{
    public static class Countries
    {
        //static Dictionary<string, string> countries = new Dictionary<string, string>();
        public static Dictionary<string, string> countriesDict= new Dictionary<string, string>()
        {
            { "Russia", Constants.SHARD_RUS },
            { "France", Constants.SHARD_EU },
            { "Germany", Constants.SHARD_EU },
            { "USA", Constants.SHARD_OTHER },
            { "India", Constants.SHARD_OTHER }
        };
    }
}