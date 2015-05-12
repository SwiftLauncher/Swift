using System.ComponentModel.Composition;
using Swift.Toolkit.Data;

namespace Swift.Extensibility.Internal
{
    [InheritedExport]
    public interface IVaultManager
    {
        EncryptedStorageVault Vault { get; }
    }
}
