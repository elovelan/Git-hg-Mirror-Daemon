﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHgMirror.Runner.Services;

namespace GitHgMirror.Runner
{
    public class UntouchedRepositoriesCleaner
    {
        private readonly MirroringSettings _settings;
        private readonly EventLog _eventLog;


        public UntouchedRepositoriesCleaner(MirroringSettings settings, EventLog eventLog)
        {
            _settings = settings;
            _eventLog = eventLog;
        }


        public void Clean()
        {
            _eventLog.WriteEntry("Starting cleaning untouched repositories.");

            var count = 0;
            if (Directory.Exists(_settings.RepositoriesDirectoryPath))
            {
                foreach (var letterDirectory in Directory.EnumerateDirectories(_settings.RepositoriesDirectoryPath))
                {
                    foreach (var repositoryDirectory in Directory.EnumerateDirectories(letterDirectory))
                    {
                        if (Directory.GetLastAccessTimeUtc(repositoryDirectory) < DateTime.UtcNow.Subtract(new TimeSpan(24, 0, 0)) &&
                            !File.Exists(Mirror.GetRepositoryLockFilePath(repositoryDirectory)))
                        {
                            Directory.Delete(repositoryDirectory, true);
                            _eventLog.WriteEntry("Removed untouched repository folder: " + repositoryDirectory);

                            count++;
                        }
                    }
                } 
            }

            _eventLog.WriteEntry("Finished cleaning untouched repositories, " + count + " folders removed.");
        }
    }
}
