using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application
{
    public class LoggerAdapter<TType> : ILoggerAdapter<TType>
    {

        private readonly ILogger<LoggerAdapter<TType>> _logger;

        public LoggerAdapter(ILogger<LoggerAdapter<TType>> logger)
        {
                _logger = logger;
        }
        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args); 
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }
    }
}
