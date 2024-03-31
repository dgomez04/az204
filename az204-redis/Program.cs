using StackExchange.Redis;

string connection_string = "{connection-string}";

using (var cache = ConnectionMultiplexer.Connect(connection_string))
{
    IDatabase db = cache.GetDatabase();

    var result = await db.ExecuteAsync("ping");
    Console.WriteLine($"PING = {result.Type} : {result}");

    bool set_value = await db.StringSetAsync("test:key", "test-value");
    Console.WriteLine($"SET = {set_value}");

    string get_value = await db.StringGetAsync("test:key");
    Console.WriteLine($"GET = {get_value}");
}