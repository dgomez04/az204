using Microsoft.Azure.Cosmos; 

public class Program
{
    private static readonly string EndpointUri = "{endpoint-uri}";

    private static readonly string PrimaryKey = "{primary-key}";

    private CosmosClient cosmosClient;

    private Database database;

    private Container container;

    private string database_id = "az204_database";
    private string container_id = "az204_container";

    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Begginig operations... \n");
            Program p = new Program();
            await p.CosmosAsync();
        }
        catch (Exception e) 
        {
            Console.WriteLine("Error: {0}", e);
        }
        finally 
        {
            Console.WriteLine("End of operations... \n");
            Console.ReadKey();
        }
    }

    public async Task CosmosAsync()
    {
        this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        await this.CreateDatabaseAsync();
        await this.CreateContainerAsync();
    }

    private async Task CreateDatabaseAsync() 
    {
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(database_id);
        Console.WriteLine("Created Database: {0}\n", this.database.Id);
    }

    private async Task CreateContainerAsync() 
    {
        this.container = await this.database.CreateContainerIfNotExistsAsync(container_id, "/id");
        Console.WriteLine("Created Container: {0}\n", this.container.Id);
    }
}