using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace HomeSecurityApi
{
    /// <summary>
    /// Make a logger with loggerfactory, but finally the system use of the log method of the context object
    /// </summary>
    class LambdaLogger
    {
        private IConfiguration Configuration { get; }
        public ILogger MachineLogger { get; private set; }

        public LambdaLogger(ILoggerFactory loggerFactory)
        {
            // Create a logging provider based on the configuration information passed through the appsettings.json
            // You can even provide your custom formatting.
            loggerFactory.AddAWSProvider(this.Configuration.GetAWSLoggingConfigSection("AWS.Logging"), 
                formatter: (logLevel, message, exception) => $"[{DateTime.UtcNow}] {logLevel}: {message}");

            // Create a logger instance from the loggerFactory
            MachineLogger = loggerFactory.CreateLogger("a");
            MachineLogger.LogInformation("Check the AWS Console CloudWatch Logs");
        }
    }
}
