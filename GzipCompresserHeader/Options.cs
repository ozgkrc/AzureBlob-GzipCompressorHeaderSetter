namespace GzipCompresserHeader
{
    internal class Options
    {

        public string StorageAccount { get; set; }


        public string StorageKey { get; set; }


        public string[] Extensions { get; set; }


        public bool Simulate { get; set; }

        public string[] Containers { get; set; }

        public int MaxAgeSeconds { get; set; }

    }
}
