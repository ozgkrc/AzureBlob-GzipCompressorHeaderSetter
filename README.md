# AzureBlob-GzipCompressorHeaderSetter
Webjobs ready Azure Blob storage Gzip compressor and Cache-Control metadata setter

NOTE: Original work done by https://github.com/stefangordon/azure-storage-gzip-encoding
Instead of commandline arguments, this application reads from app.config so you can easily change configs from Azure Portal


Other changes

Added: 

+Multiple container supoort

Removed:

-Connection string support(Works with account key)

-Adding files with new extension(Just replaces)

-CORS support(It will work as a webjob,so omitted)
