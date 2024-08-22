namespace minimal_api;

class Program
{
    static void Main(string[] args)
    {
        IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        CreateHostBuilder(args).Build().Run();
    }
}
