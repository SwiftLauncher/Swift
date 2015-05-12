using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Swift.Infrastructure.Extensibility
{
    /// <summary>
    /// Provides exports based on configuration settings.
    /// </summary>
    public class ConfigurationBasedFilteringCatalog : AggregateCatalog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBasedFilteringCatalog"/> class.
        /// </summary>
        public ConfigurationBasedFilteringCatalog()
            : base()
        {
        }

        /// <summary>
        /// Ruft die Exportdefinitionen ab, die mit der durch die angegebene Definition festgelegten Einschränkung übereinstimmen.
        /// </summary>
        /// <param name="definition">Die Bedingungen der zurückzugebenden <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" />-Objekte.</param>
        /// <returns>
        /// Eine Auflistung von <see cref="T:System.Tuple`2" />, die die <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" />-Objekte und ihre zugeordneten <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" />-Objekte für Objekte enthält, die mit der durch <paramref name="definition" /> angegebenen Einschränkung übereinstimmen.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            var targetCardinality = definition.Cardinality;
            var newdef = new ImportDefinition(definition.Constraint, definition.ContractName, ImportCardinality.ZeroOrMore, definition.IsRecomposable, definition.IsPrerequisite, definition.Metadata);
            var exports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            foreach (var catalog in Catalogs)
            {
                exports.AddRange(catalog.GetExports(newdef));
            }
            // TODO filter correctly
            switch (targetCardinality)
            {
                case ImportCardinality.ExactlyOne:
                    return exports.Take(1);
                case ImportCardinality.ZeroOrMore:
                    return exports;
                case ImportCardinality.ZeroOrOne:
                    return exports.Take(1);
                default:
                    break;
            }
            return exports;
        }
    }
}
