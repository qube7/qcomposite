using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides support for creating <see cref="ImportDefinition"/> objects.
    /// </summary>
    internal static class ImportFactory
    {
        #region Methods

        /// <summary>
        /// Creates an import definition for the specified type by using the specified contract name and cardinality.
        /// </summary>
        /// <param name="type">The type to import into.</param>
        /// <param name="contractName">The contract name to use for the import.</param>
        /// <param name="cardinality">The cardinality of the import.</param>
        /// <returns>An import definition created for the specified type.</returns>
        internal static ImportDefinition CreateImportDefinition(Type type, string contractName, ImportCardinality cardinality)
        {
            string typeIdentity = null;

            if (type != typeof(object))
            {
                typeIdentity = AttributedModelServices.GetTypeIdentity(type);
            }

            Dictionary<string, object> metadata = null;

            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                metadata = new Dictionary<string, object>();

                metadata.Add(CompositionConstants.GenericContractMetadataName, AttributedModelServices.GetTypeIdentity(type.GetGenericTypeDefinition()));
                metadata.Add(CompositionConstants.GenericParametersMetadataName, type.GetGenericArguments());
            }

            return new ContractBasedImportDefinition(contractName, typeIdentity, null, cardinality, false, false, CreationPolicy.Any, metadata);
        }

        /// <summary>
        /// Creates an import definition for the specified type by using the specified cardinality.
        /// </summary>
        /// <param name="type">The type to import into.</param>
        /// <param name="cardinality">The cardinality of the import.</param>
        /// <returns>An import definition created for the specified type.</returns>
        internal static ImportDefinition CreateImportDefinition(Type type, ImportCardinality cardinality)
        {
            return CreateImportDefinition(type, AttributedModelServices.GetContractName(type), cardinality);
        }

        #endregion
    }
}
