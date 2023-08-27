using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace FileBackup.Classes
{
    /// <summary>
    /// Настройки папок.
    /// </summary>
    class DirectoryOptions
    {
        public List<string> Sources { get; set; }
        public string Target { get; set; }

        /// <summary>
        /// Инициализирует настройки папок.
        /// </summary>
        /// <param name="configuration">Конфигруация приложения.</param>
        public DirectoryOptions(IConfiguration configuration)
        {
            Sources = configuration
                .GetSection("Directories:sources")
                .GetChildren()
                .Select(x => x.Value)
                .ToList();
            Target = configuration.GetSection("Directories:target").Value;
        }
    }
}
