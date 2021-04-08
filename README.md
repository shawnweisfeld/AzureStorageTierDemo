# AzureStorageTierDemo

The goal of this sample is to show an efficient way to queue all the objects in an Azure Storage Container for moving from Archive to Hot or Cool storage.

The sample leverages:

1. [Multi-threaded Architecture](https://docs.microsoft.com/dotnet/api/system.threading.semaphoreslim) to increase total throughput
1. Use of the [Batch API](https://docs.microsoft.com/rest/api/storageservices/blob-batch) to reduce calls to Azure
1. Deployment to an [Azure Container Instance](https://azure.microsoft.com/services/container-instances/) to reduce network latency vs running over the internet

## Setup

This application makes use of Log Analytics and Application Insights for logging. Follow the instructions [here](https://docs.microsoft.com/azure/azure-monitor/app/create-workspace-resource#create-workspace-based-resource) to deploy both of these services.

You will also need a storage account filled with objects that you want to move from Archive to Hot/Cool.

For ease of deployment I publish a build of this code to my Docker Hub account. However, please feel free to download and compile the code yourself.

## Deploy to ACI

TODO: need instructions on how to deploy to ACI