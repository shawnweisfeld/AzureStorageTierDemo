# AzureStorageTierDemo

The goal of this sample is to show an efficient way to queue all the objects in an Azure Storage Container for moving from Archive to Hot or Cool storage.

The sample leverages:

1. [Multi-threaded Architecture](https://docs.microsoft.com/dotnet/api/system.threading.semaphoreslim) to increase total throughput
1. Use of the [Batch API](https://docs.microsoft.com/rest/api/storageservices/blob-batch) to reduce calls to Azure
1. Deployment to an [Azure Container Instance](https://azure.microsoft.com/services/container-instances/) to reduce network latency vs running over the internet

TODO: need instructions on how to deploy to ACI