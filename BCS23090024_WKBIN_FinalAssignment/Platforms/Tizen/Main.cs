using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace BCS23090024_WKBIN_FinalAssignment
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
