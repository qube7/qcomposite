using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides support for creating <see cref="ExportDefinition"/> objects.
    /// </summary>
    internal static class ExportFactory
    {
        #region Methods

        /// <summary>
        /// Creates an export definition from the specified type, with the specified contract name and scope of visibility.
        /// </summary>
        /// <param name="type">The type to export.</param>
        /// <param name="contractName">The contract name to use for the export.</param>
        /// <param name="exportScope">The scope of visibility to use for the export.</param>
        /// <returns>An export definition created from the specified type.</returns>
        internal static ExportDefinition CreateExportDefinition(Type type, string contractName, ExportScope exportScope)
        {
            Dictionary<string, object> metadata = new Dictionary<string, object>();

            metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(type));
            metadata.Add(MetadataName.ExportScope, exportScope);

            return new ExportDefinition(contractName, metadata);
        }

        /// <summary>
        /// Creates an export definition from the specified type, with the specified scope of visibility.
        /// </summary>
        /// <param name="type">The type to export.</param>
        /// <param name="exportScope">The scope of visibility to use for the export.</param>
        /// <returns>An export definition created from the specified type.</returns>
        internal static ExportDefinition CreateExportDefinition(Type type, ExportScope exportScope)
        {
            return CreateExportDefinition(type, AttributedModelServices.GetContractName(type), exportScope);
        }

        #endregion
    }
}
