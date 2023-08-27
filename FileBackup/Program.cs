using FileBackup.Classes;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.IO;

namespace FileBackup
{
    /// <summary>
    /// Программа.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в приложение.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        static void Main(string[] args)
        {
            var config = GetConfiguration();
            var logger = GetLogger(config);
            logger.Info("Старт приложения");
            var backuper = new Backuper(config);
            backuper.Run();
            Console.WriteLine("Нажмите любую клавишу");
            Console.ReadKey();
            logger.Info("Завершение приложения");
        }

        /// <summary>
        /// Возвращает логгер.
        /// </summary>
        /// <param name="configuration">Конфигруация приложения.</param>
        /// <returns>Логгер.</returns>
        static Logger GetLogger(IConfiguration configuration)
        {
            return LogManager.Setup()
                .LoadConfigurationFromSection(configuration)
                .GetCurrentClassLogger();
        }

        /// <summary>
        /// Возвращает конфигруацию приложения.
        /// </summary>
        /// <returns>Конфигруация приложения.</returns>
        static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
