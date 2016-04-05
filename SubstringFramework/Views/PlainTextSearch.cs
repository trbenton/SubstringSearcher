using ProtoBuf;

namespace SubstringFramework.Views
{
    [ProtoContract]
    public class PlainTextSearch
    {
        [ProtoMember(1, IsRequired = true)]
        public bool UseBlocks { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Path { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string Pattern { get; set; }


        public PlainTextSearch(bool useBlocks, string path, string pattern)
        {
            UseBlocks = useBlocks;
            Path = path;
            Pattern = pattern;
        }

        public PlainTextSearch()
        {
            
        }
    }
}
