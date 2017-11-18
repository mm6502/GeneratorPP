using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Digital.Slovensko.Ekosystem.GeneratorPP
{
    /// <summary>
    /// The main program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Program.BuildWebHost(args).Run();
        }

        /// <summary>
        /// Builds the web host.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}
