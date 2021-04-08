# AzureStorageTierDemo

The goal of this sample application is to show an efficient way to queue all the objects in an Azure Storage Container for moving from Archive to Hot or Cool storage. The goal is to enqueue objects to be moved from Archive to Hot or Cool as fast as possible. The time it takes to do the actual move will depend on a number of factors. 

More info on the rehydration process can be found [here](https://docs.microsoft.com/azure/storage/blobs/storage-blob-rehydration)

The sample leverages:

1. [Multi-threaded Architecture](https://docs.microsoft.com/dotnet/api/system.threading.semaphoreslim) to increase total throughput. Threads are spawned based on the naming convention in your storage account using an `/` as the delimiter, see [here](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobcontainerclient.getblobsbyhierarchy) for more info.
1. Use of the [Batch API](https://docs.microsoft.com/rest/api/storageservices/blob-batch) to reduce calls to Azure
1. Deployment to an [Azure Container Instance](https://azure.microsoft.com/services/container-instances/) to reduce network latency vs running over the internet

## Setup

This application makes use of Log Analytics and Application Insights for logging. Follow the instructions [here](https://docs.microsoft.com/azure/azure-monitor/app/create-workspace-resource#create-workspace-based-resource) to deploy both of these services.

You will also need a storage account filled with objects that you want to move from Archive to Hot/Cool.

For ease of deployment I publish a build of this code to my [Docker Hub account](https://hub.docker.com/repository/docker/sweisfel/azurestoragetierdemo). However, please feel free to download and compile the code yourself.

## Deploy to ACI

- I will typically run this from the bash [Azure Cloud Shell](https://docs.microsoft.com/azure/cloud-shell/overview).
- Update the values to match your environment

Explanation of what each of the arguments in the deployment script do

```
az container create \
    --name "azurestoragetierdemo" \ <-- the name you want to use for the ACI instance
    --resource-group "tierdemo" \ <-- the name of the resource group that you want to deploy the ACI instance to
    --location southcentralus \ <-- the region you want to deploy the ACI instance to (this should match the region the storage account is in)
    --cpu 2 \ <-- the amount of CPU to give your ACI instance
    --memory 4 \ <-- the amount of Memory to give your ACI instance
    --image "sweisfel/azurestoragetierdemo:latest" \ <-- the docker container to deploy into the ACI instance
    --restart-policy Never \ <-- We dont want ACI to restart if it fails/finishes as every time we run the container it starts over
    --no-wait \ <-- We dont want the CLI to wait for the deployment to complete before it returns control back to you
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="key" \ <-- the key from App Insights (its a GUID)
        StorageConnectionString="connection string" \ <-- the connection string to your storage account (looks like this "DefaultEndpointsProtocol=https;AccountName=foo;AccountKey=bar;EndpointSuffix=core.windows.net")
        Container="container name" \ <-- the storage container that your blobs are in
        Prefix="" \ <-- the path to the blobs if you only want to scan a subset of the container
        WhatIf="false" \ <-- runs the app in whatif mode, in this mode it will do the scan, but not make any changes
        ThreadCount="0" \ <-- set to 0 to use the default number of threads, or enter a custom value
        TargetAccessTier="Hot" <-- what tier you want the blobs moved to, either "Hot" or "Cool"
```

Script without comments for you to copy/paste

``` bash

az container create \
    --name "azurestoragetierdemo" \
    --resource-group "my rg" \
    --location southcentralus \
    --cpu 2 \
    --memory 4 \
    --image "sweisfel/azurestoragetierdemo:latest" \
    --restart-policy Never \
    --no-wait \
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="my key" \
        StorageConnectionString="my connection string" \
        Container="container name" \
        Prefix="" \
        WhatIf="false" \
        ThreadCount="0" \
        TargetAccessTier="Hot"


```

## Tips

- Deploy the ACI instance to the SAME region your storage account is in. This will reduce network latency on the calls between the app and the storage account.
- Running the application in `WhatIf` mode is a good way to get an idea of if the files have been read off of archive and put back in your tier of choice (hot/cool). However, it needs to scan each object in the container to do this. For larger containers this will take time and consume storage transactions.
- You can rerun the above bash script with different values for your environmental variables to change them, without needing to delete and recreate the ACI instance.
- You can run multiple instances of ACI (with different names) if you want to process multiple storage accounts/containers at the same time.

## Monitoring

You can run this Kusto query against the Log Analytics Workspace to see logs for the last run.

``` sql

let runs = AppDependencies | summarize TimeGenerated=max(TimeGenerated) by run=tostring(Properties["Run"]) | order by TimeGenerated | take 1 ;
AppDependencies
| where tostring(Properties["Run"]) in (runs)
| extend run=tostring(Properties["Run"])
| order by TimeGenerated 

```

## Clean up

When you finish you can delete the ACI instances, App Insights and Log Analytics. However, you might want to hang onto the App Insights and Log Analytics logs incase you might need the info later or might need to do another run later.
