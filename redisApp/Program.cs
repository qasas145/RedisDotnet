using StackExchange.Redis;

public class Program
{

    public static IConnectionMultiplexer _redis;
    static int consumer = 1;

    const string CHANNEL = "Muhammad";
    public static async Task Main(string[] args)
    {

        _redis = ConnectionMultiplexer.Connect("localhost:6379");

        await BasicData();
        await CallSubscribePublish();
        await FireAndForget();
        await NullValues();
        
        Console.ReadLine();
    }
    private static async Task  BasicData()
    {
        var database = _redis.GetDatabase();
        await database.StringSetAsync("counter", 1);
        await database.StringIncrementAsync("counter", 20);
        Console.WriteLine(database.StringGet("counter"));

        const string sortedSetKey = "my:names";

        var sortedSetEntry00 = new SortedSetEntry("Muhammad2 Elsayed Elqasas", 1002);
        var sortedSetEntry01 = new SortedSetEntry("Ahmed2 Elsayed Osman Elqassa", 502);

        await Task.WhenAll(
            database.SortedSetAddAsync(sortedSetKey, [sortedSetEntry00, sortedSetEntry01]),
            Task.Delay(10000),
            database.SortedSetAddAsync(sortedSetKey, "Osman2 Mahmoud Ebraheem Elqasas",845),
            Task.Delay(10000),
            database.SortedSetAddAsync(sortedSetKey, "Mahmoud2 Abdelhady",84),
            Task.Delay(10000),
            database.SortedSetAddAsync(sortedSetKey, "Mahmoud2 Abdelhady",21),
            Task.Delay(10000),
            database.SortedSetAddAsync(sortedSetKey, "Mahmoud2 Abdelhady",45)
        );

        //database.sorteds
        var result = database.SortedSetRangeByScore("my:names", 0, 200);
        
        Console.WriteLine(string.Concat(",", result));
        foreach (var item in result)
        {
            Console.WriteLine(item);
        }

    }
    private static async Task CallSubscribePublish()
    {

        Console.Write("want to publish or subscribe 0/1? any number to resume ");
        var condition = Console.ReadLine();
        var c = true;

        if (int.Parse(condition) == 0)
        {
            while (c)
            {
                await Publisher();
                Console.Write("Complete ? y/n");
                c = Console.ReadLine() == "n" ? false : true;
            }
        }
        else if (int.Parse(condition) == 1)
        {

            while (c)
            {
                await Subscriber();
                consumer++;
                Console.Write("Complete ? y/n");
                c = Console.ReadLine() == "n" ? false : true;
            }

        }

    }
    private static async Task Publisher()
    {
        var queue = _redis.GetSubscriber();
        await queue.PublishAsync(CHANNEL, "hello " + DateTime.Now.ToShortDateString());

    }
    private static async Task Subscriber()
    {

        var queue = _redis.GetSubscriber();

        #region this method was synchronized
        //var subObj = await queue.SubscribeAsync(CHANNEL);

        //subObj.OnMessage((msg) =>
        //{
        //    Thread.Sleep(10000);
        //    Console.WriteLine("The message recieved was {0} consumer {1}", msg, consumer);
        //});

        #endregion

        #region async 

        await queue.SubscribeAsync(CHANNEL, (channel, msg) =>
        {
            Thread.Sleep(10000);
            Console.WriteLine("The message recieved was {0} consumer {1}", msg, consumer);
        });
        #endregion

    }

    private static async Task FireAndForget()
    {
        var database = _redis.GetDatabase();
        const string KEY = "my:number";

        await database.StringSetAsync(KEY, "Random");
        database.KeyExpire(KEY, TimeSpan.FromMinutes(5), CommandFlags.FireAndForget);

        var value = database.StringGet(KEY);
        Console.WriteLine(value);
    }
    
    private static async Task NullValues()
    {
        var database = _redis.GetDatabase();
        const string KEY = "missing:key";

        var value = await database.StringGetAsync(KEY);
        Console.WriteLine(value);
        Console.WriteLine(value.IsNull);

        Console.WriteLine((int)value);

        var res = (int?)value;
        Console.WriteLine(res == null);
    }
}
