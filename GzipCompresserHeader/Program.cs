using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GzipCompresserHeader
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            options.StorageAccount = ConfigurationManager.AppSettings["StorageAccount"];
            options.StorageKey = ConfigurationManager.AppSettings["StorageKey"];
            options.Extensions = ConfigurationManager.AppSettings["Extensions"].Split(',');
            options.Simulate = Boolean.Parse(ConfigurationManager.AppSettings["Simulate"]);
            options.Containers = ConfigurationManager.AppSettings["Containers"].Split(',');
            options.MaxAgeSeconds = Int32.Parse(ConfigurationManager.AppSettings["MaxAgeSeconds"]);

            CloudStorageAccount storageAccount;

            if (!string.IsNullOrEmpty(options.StorageAccount) && !String.IsNullOrEmpty(options.StorageKey))
            {
                storageAccount = new CloudStorageAccount(new StorageCredentials(options.StorageAccount, options.StorageKey), true);
            }
            else
            {
                Console.WriteLine("Connection info is wrong");
                return;
            }

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            foreach (var container in options.Containers)
            {
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);

                // Do the compression work
                Utility.EnsureGzipFiles(blobContainer, options.Extensions, options.MaxAgeSeconds, options.Simulate);

                Console.WriteLine("Container: " + container + " complete.");
            }
            Console.WriteLine("Complete.");

        }
    }
}

