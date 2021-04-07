namespace AzureStorageTierDemo
{
    public class Config
    {
        public string Run { get; set; }
        public string Prefix { get; set; }
        public string StorageConnectionString { get; set; }
        public string Container { get; set; }

        public int ThreadCount { get; set; }
        public bool WhatIf { get; set; }


    }
    
}