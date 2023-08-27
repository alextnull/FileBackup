using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileBackup.Classes
{
    /// <summary>
    /// Бэкапер.
    /// </summary>
    class Backuper
    {
        private Logger _logger;
        private DirectoryOptions _options;

        /// <summary>
        /// Инициализирует бэкапер.
        /// </summary>
        /// <param name="configuration">Конфигруация приложения.</param>
        public Backuper(IConfiguration configuration)
        {
            _logger = LogManager
                .Setup()
                .LoadConfigurationFromSection(configuration)
                .GetCurrentClassLogger();
            _options = new DirectoryOptions(configuration);
        }

        /// <summary>
        /// Производит резервное копирование.
        /// </summary>
        public void Run()
        {
            _logger.Info("Старт резервного копирования файлов");
            var timestampDirectory = CreateTimestampDirectory(_options.Target);
            CopyFiles(_options.Sources, timestampDirectory);
            _logger.Info("Завершение резервного копирования файлов");
        }

        /// <summary>
        /// Создаёт папку с временным штампом в целевой папке.
        /// </summary>
        /// <param name="directoryName">Целевая папка.</param>
        /// <returns>Путь к папке с временным штампом.</returns>
        private string CreateTimestampDirectory(string directoryName)
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);
                var timestampDirectory = Path.Combine(directoryName, timestamp);
                Directory.CreateDirectory(timestampDirectory);
                return timestampDirectory;
            }
            catch (IOException ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Копирует файлы из исходных папок в целевую папку.
        /// </summary>
        /// <param name="sourceDirectories">Исходные папки.</param>
        /// <param name="targetDirectory">Целевая папка.</param>
        private void CopyFiles(List<string> sourceDirectories, string targetDirectory)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                _logger.Info($"Старт обработки исходной папки '{sourceDirectory}'");
                CopyFiles(sourceDirectory, targetDirectory);
                _logger.Info($"Завершение обработки исходной папки '{sourceDirectory}'");
            }
        }

        /// <summary>
        /// Копирует файлы из исходной папки в целевую папку.
        /// </summary>
        /// <param name="sourceDirectory">Исходная папка.</param>
        /// <param name="targetDirectory">Целевая папка.</param>
        private void CopyFiles(string sourceDirectory, string targetDirectory)
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(sourceDirectory);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Error($"Не найдена исходная папка '{sourceDirectory}'", ex);
                return;
            }
            catch (IOException ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }

            foreach (var file in files)
            {
                FileStream fileSourceStream = null;
                FileStream fileTargetStream = null;

                try
                {
                    var fileName = Path.GetFileName(file);
                    var targetFile = Path.Combine(targetDirectory, fileName);

                    fileSourceStream = new FileStream(file, FileMode.Open);
                    fileTargetStream = new FileStream(targetFile, FileMode.Create);

                    int readByte;
                    do
                    {
                        readByte = fileSourceStream.ReadByte();
                        if (readByte != -1)
                        {
                            fileTargetStream.WriteByte((byte)readByte);
                        }
                    } while (readByte != -1);
                    _logger.Debug($"Скопирован файл '{file}'");
                }
                catch (IOException ex)
                {
                    _logger.Error(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
                finally
                {
                    fileSourceStream?.Dispose();
                    fileTargetStream?.Dispose();
                }
            }
        }
    }
}
