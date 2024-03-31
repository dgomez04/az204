
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("blob storage exercise");

ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("done, press enter to exit the example");
Console.ReadLine();

static async Task ProcessAsync()
{ 

    // create blob service client (storage account)
    string connectionString = "DefaultEndpointsProtocol=https;AccountName={account-name};AccountKey={account-key};EndpointSuffix=core.windows.net";
    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

    // create a new container by calling the service client's CreateBlobContainerAsync method
    string containerName = "wtblob" + Guid.NewGuid().ToString();
    BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

    Console.WriteLine("A container named '" + containerName + "' has been created. " +
        "\nTake a minute and verify in the portal." + 
        "\nNext a file will be created and uploaded to the container.");
    
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();

    // create a local file in the path file for uploading and downloading.
    string local_path = "./data";
    string file_name = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string local_filepath = Path.Combine(local_path, file_name);

    // write text to the file
    await File.WriteAllTextAsync(local_filepath, "this is a test!");
    
    // get reference to the blob
    BlobClient blobClient = containerClient.GetBlobClient(file_name);

    Console.WriteLine("Uploading to Blob storage as blob:\n\t" + blobClient.Uri);

    using (FileStream uploadFileStream = File.OpenRead(local_filepath))
    {
        await blobClient.UploadAsync(uploadFileStream, true);
        uploadFileStream.Close();
    }

    Console.WriteLine("\nThe file was uploaded. We'll verify by listing" + 
        " the blobs next.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // list blobs in the container
    Console.WriteLine("listing blobs in the container.");
    await foreach(BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    Console.WriteLine("\nYou can also verify by looking inside the " + 
        "container in the portal." +
        "\nNext the blob will be downloaded with an altered file name.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // download blob
    string dowload_filepath = local_filepath.Replace(".txt", "DOWNLOADED.txt");
    Console.WriteLine("Downloading blob to\n\t" + dowload_filepath);

    BlobDownloadInfo download = await blobClient.DownloadAsync();

    using (FileStream downloadFileStream = File.OpenWrite(dowload_filepath))
    {
        await download.Content.CopyToAsync(downloadFileStream);
        downloadFileStream.Close();
    }

    Console.WriteLine("\nLocate the local file in the data directory created earlier to verify it was downloaded.");
    Console.WriteLine("The next step is to delete the container and local files.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // delete the container

    Console.WriteLine("Deleting the container.");
    await containerClient.DeleteAsync();

    Console.WriteLine("deleting the local source and downloaded files...");
    File.Delete(local_filepath);
    File.Delete(dowload_filepath);

    Console.WriteLine("The container and local files have been deleted.");

}