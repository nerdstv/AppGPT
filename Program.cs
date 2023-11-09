using WinFormsApp3.models;
using Serilog;
using Serilog.Core;

namespace WinFormsApp3
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(@"C:\AppGPT\LogFile.log").CreateLogger();


            //
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
           
            Application.Run(new Form1());
            Log.Information("Finish");
            //Log.CloseAndFlush();
        }
    }
}