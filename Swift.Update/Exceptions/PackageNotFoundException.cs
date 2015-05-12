using System;

namespace Swift.Update.Exceptions
{
    public class PackageNotFoundException : Exception
    {
        public PackageNotFoundException(string packageID, bool remote)
            : base("The package '" + packageID + "' could not be found on the " + (remote ? "remote" : "local") + " repository.")
        { }
    }
}
