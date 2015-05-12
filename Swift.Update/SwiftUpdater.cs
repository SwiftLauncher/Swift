using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace Swift.Update
{
    public class SwiftUpdater
    {
        private const string REPOSITORY_PATH = "https://www.myget.org/F/swift/";
        private string _localPath;
        private IEnumerable<string> _shortcutPaths;

        public SwiftUpdater(string localRepositoryPath, params string[] shortcutPaths)
        {
            _localPath = localRepositoryPath;
            _shortcutPaths = shortcutPaths;
        }

        public bool CheckForUpdate()
        {
            try
            {
                var pm = new PackageManager(PackageRepositoryFactory.Default.CreateRepository(REPOSITORY_PATH), new DefaultPackagePathResolver(REPOSITORY_PATH), new PhysicalFileSystem(_localPath));
                var swiftp = pm.LocalRepository.FindPackage("Swift");
                var updates = pm.LocalRepository.GetUpdates(Enumerable.Repeat(swiftp, 1), false, false);
                return updates != null && updates.Count() > 0;
            }
            catch { return false; }
        }

        public void ApplyUpdate()
        {
            // download and install
            var pm = new PackageManager(PackageRepositoryFactory.Default.CreateRepository(REPOSITORY_PATH), new DefaultPackagePathResolver(REPOSITORY_PATH), new PhysicalFileSystem(_localPath));
            pm.UpdatePackage("Swift", false, false);

            // redo shortcuts
            EnsureShortcuts();
        }

        public void EnsureShortcuts()
        {
            foreach (var p in _shortcutPaths)
            {
                ShellLink sc;
                try
                {
                    sc = new ShellLink(p);
                }
                catch
                {
                    sc = new ShellLink();
                    sc.ShortCutFile = p;
                }
                var swiftexes = Directory.GetFiles(_localPath, "Swift.exe", SearchOption.AllDirectories);
                sc.Target = swiftexes.OrderByDescending(_ => _).First();
                sc.Description = "Start Swift";
                sc.Save();
            }
        }
    }
}
