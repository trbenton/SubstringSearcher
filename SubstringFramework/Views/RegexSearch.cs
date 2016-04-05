using ProtoBuf;

namespace SubstringFramework.Views
{
    [ProtoContract]
    public class RegexSearch
    {
        [ProtoMember(1, IsRequired = true)]
        public string Path { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Regex { get; set; }

        public RegexSearch(string path, string regex)
        {
            Path = path;
            Regex = regex;
        }

        public RegexSearch()
        {

        }
    }
}
