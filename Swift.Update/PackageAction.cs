using System;
using System.IO;
using NuGet;
using Swift.Update.Exceptions;

namespace Swift.Update
{
    /// <summary>
    /// Installs/Updates/Removes a package.
    /// </summary>
    public class PackageAction : UpdateAction
    {
        /// <summary>
        /// Gets the package identifier of the package that should be installed.
        /// </summary>
        public string PackageID { get; private set; }

        /// <summary>
        /// Gets the type of the action.
        /// </summary>
        public PackageActionType ActionType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this action has to be successful in order to continue with updating.
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Gets the sub directory path that this package should be installed to.
        /// </summary>
        public string SubDirectoryPath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageAction"/> class.
        /// </summary>
        /// <param name="packageID">The package identifier.</param>
        public PackageAction(string packageID, PackageActionType actionType, string subdirectoryPath, bool required = false)
        {
            PackageID = packageID;
            ActionType = actionType;
            SubDirectoryPath = subdirectoryPath;
            Required = required;
        }

        /// <summary>
        /// Executes this action, reporting progress using the provided callback.
        /// </summary>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns></returns>
        public override UpdateResult Execute(ProgressCallback progressCallback)
        {
            try
            {
                switch (ActionType)
                {
                    case PackageActionType.Install:
                        InstallPackage(progressCallback);
                        break;
                    case PackageActionType.Uninstall:
                        UninstallPackage(progressCallback);
                        break;
                    case PackageActionType.Update:
                        UpdatePackage(progressCallback);
                        break;
                    case PackageActionType.UpdateOrInstall:
                        UpdateOrInstallPackage(progressCallback);
                        break;
                    default:
                        break;
                }
                return new UpdateResult(true);
            }
            catch (PackageNotFoundException pex)
            {
                progressCallback(0, pex.Message);
                return new UpdateResult(false, pex, !Required);
            }
            catch (Exception ex)
            {
                progressCallback(0, "An error occurred while trying to install package '" + PackageID + "'.");
                return new UpdateResult(false, ex, !Required);
            }
        }

        private void UpdateOrInstallPackage(ProgressCallback progressCallback)
        {
            try
            {
                UpdatePackage(progressCallback);
            }
            catch (PackageNotFoundException)
            {
                InstallPackage(progressCallback);
            }
        }

        private void UpdatePackage(ProgressCallback progressCallback)
        {
            progressCallback(0, "Connecting to Swift package server...");
            var manager = GetPackageManager();
            progressCallback(-1, "Searching for package '" + PackageID + "'...");
            var remotep = manager.SourceRepository.FindPackage(PackageID);
            if (remotep == null) throw new PackageNotFoundException(PackageID, true);
            var localp = manager.LocalRepository.FindPackage(PackageID);
            if (localp == null) throw new PackageNotFoundException(PackageID, false);

            progressCallback(-1, "Downloading and updating package '" + PackageID + "'...");
            manager.UpdatePackage(remotep, true, false);
            progressCallback(100, "Successfully updated package '" + PackageID + "'.");
        }

        private void UninstallPackage(ProgressCallback progressCallback)
        {
            progressCallback(0, "Connecting to Swift package server...");
            var manager = GetPackageManager();
            progressCallback(-1, "Searching for local package '" + PackageID + "'...");
            var localp = manager.LocalRepository.FindPackage(PackageID);
            if (localp == null) throw new PackageNotFoundException(PackageID, false);

            progressCallback(-1, "Uninstalling package '" + PackageID + "'...");
            manager.UninstallPackage(localp, true, true);
            progressCallback(100, "Successfully uninstalled package '" + PackageID + "'.");
        }

        private void InstallPackage(ProgressCallback progressCallback)
        {
            progressCallback(0, "Connecting to Swift package server...");
            var manager = GetPackageManager();
            progressCallback(-1, "Searching for package '" + PackageID + "'...");
            var p = manager.SourceRepository.FindPackage(PackageID);
            progressCallback(-1, "Downloading and installing package '" + PackageID + "'...");
            manager.InstallPackage(p, false, false);
            progressCallback(100, "Successfully installed package '" + PackageID + "'.");
        }

        private IPackageManager GetPackageManager()
        {
            return new PackageManager(PackageRepositoryFactory.Default.CreateRepository("https://www.myget.org/F/swift/"), new DefaultPackagePathResolver("https://www.myget.org/F/swift/"), new PhysicalFileSystem(Path.Combine(Utilities.GetSwiftRootPath(), SubDirectoryPath)));
        }
    }

    /// <summary>
    /// Represents the types of actions that a PackageAction can perform.
    /// </summary>
    public enum PackageActionType
    {
        Install,
        Uninstall,
        Update,
        UpdateOrInstall
    }
}
