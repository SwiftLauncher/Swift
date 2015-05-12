using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Swift.Infrastructure.Extensibility
{
    public class TrustedPluginCatalog : ComposablePartCatalog
    {
        AggregateCatalog _aggregateCatalog = new AggregateCatalog();

        public TrustedPluginCatalog(string path, params byte[][] trustedKeys)
        {
            try
            {
                // TODO get trustedKeys from vault
                // TODO report successful/unsuccessful loads
                foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                {
                    AssemblyName assemblyName = null;
                    try
                    {
                        assemblyName = AssemblyName.GetAssemblyName(file);
                    }
                    catch (ArgumentException)
                    {
                        //  According to MSDN, ArgumentException can be thrown
                        //  if the assembly file is invalid
                    }
                    catch (BadImageFormatException)
                    {
                        //  Not a valid assembly
                    }

                    if (assemblyName != null)
                    {
                        var publicKey = assemblyName.GetPublicKey();
                        //if (publicKey != null)
                        //{
                        //    bool trusted = false;
                        //    foreach (var trustedKey in trustedKeys)
                        //    {
                        //        if (publicKey.SequenceEqual(trustedKey))
                        //        {
                        //            trusted = true;
                        //            break;
                        //        }
                        //    }
                        //    trusted = true; // TODO handle differently
                        //    if (trusted)
                        //    {
                        _aggregateCatalog.Catalogs.Add(new AssemblyCatalog(file));
                        //}
                        //else
                        //{
                        //    System.Windows.MessageBox.Show("Did not trust '" + file + "'.");
                        //}
                        //}
                    }
                    else
                    {
                        // TODO
                        //MessageBox.Show("Could not load assembly '" + file + "'.");
                    }
                }
            }
            catch (Exception)
            {
                // TODO
                //MessageBox.Show(ex.Message);
            }
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _aggregateCatalog.Parts;
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return _aggregateCatalog.GetExports(definition);
        }
    }

}
