using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace GzipCompresserHeader
{
    static class Utility
    {
        public static void EnsureGzipFiles(CloudBlobContainer container, IEnumerable<string> extensions, int cacheControlMaxAgeSeconds, bool simulate)
        {
            Console.WriteLine("Enumerating files.");

            string cacheControlHeader = "public, max-age=" + cacheControlMaxAgeSeconds.ToString();

            var blobInfos = container.ListBlobs(null, true, BlobListingDetails.Metadata);

            Parallel.ForEach(blobInfos, (blobInfo) =>
            {
                CloudBlob blob = (CloudBlob)blobInfo;

                // Only work with desired extensions
                string extension = Path.GetExtension(blobInfo.Uri.LocalPath);
                if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    return;
                }

                // Check if it is already done

                if (string.Equals(blob.Properties.ContentEncoding, "gzip", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Skipping already compressed blob: " + blob.Name);
                    return;
                }


                // Compress blob contents
                Console.WriteLine("Downloading blob: " + blob.Name);

                byte[] compressedBytes;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    using (var blobStream = blob.OpenRead())
                    {
                        blobStream.CopyTo(gzipStream);
                    }

                    compressedBytes = memoryStream.ToArray();
                }

                // Blob to write to 
                CloudBlockBlob destinationBlob;


                destinationBlob = (CloudBlockBlob)blob;


                if (simulate)
                {
                    Console.WriteLine("NOT writing blob, due to simulation: " + blob.Name);
                }
                else
                {
                    // Upload the compressed bytes to the new blob
                    Console.WriteLine("Writing blob: " + blob.Name);
                    destinationBlob.UploadFromByteArray(compressedBytes, 0, compressedBytes.Length);

                    // Set the blob headers
                    Console.WriteLine("Configuring headers");
                    destinationBlob.Properties.CacheControl = cacheControlHeader;
                    destinationBlob.Properties.ContentType = blob.Properties.ContentType;
                    destinationBlob.Properties.ContentEncoding = "gzip";
                    destinationBlob.SetProperties();
                }

            });
        }
    }
}