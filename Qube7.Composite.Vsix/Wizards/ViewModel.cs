using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;

namespace Qube7.Composite.Design.Wizards
{
    /// <summary>
    /// Represents the ViewModel (MVVM) template wizard extension.
    /// </summary>
    public class ViewModel : Wizard
    {
        #region Fields

        /// <summary>
        /// Specifies the name of the full path property of the project item.
        /// </summary>
        private const string FullPath = "FullPath";

        /// <summary>
        /// Specifies the name of the dependent-upon property of the project item.
        /// </summary>
        private const string DependentUpon = "DependentUpon";

        /// <summary>
        /// Specifies the extension of the partial class file.
        /// </summary>
        private const string Partial = ".Partial.cs";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (replacementsDictionary.ItemName(out string itemName))
            {
                replacementsDictionary[Constants.SafeViewModel] = itemName;
            }

            if (replacementsDictionary.SafeName(nameof(ViewModel), out string safeName))
            {
                replacementsDictionary[Constants.SafeName] = string.Format(safeName, string.Empty);

                replacementsDictionary[Constants.SafeView] = string.Format(safeName, nameof(View));
            }

            if (replacementsDictionary.FileName(nameof(ViewModel), out string fileName))
            {
                replacementsDictionary[Constants.View] = string.Format(fileName, nameof(View));
            }
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            string fullPath = projectItem.Properties.Item(FullPath)?.Value?.ToString();

            if (string.IsNullOrEmpty(fullPath))
            {
                return;
            }

            string fileName = Path.GetFileName(fullPath);
            if (fileName.EndsWith(Partial, StringComparison.CurrentCultureIgnoreCase))
            {
                Property property = projectItem.Properties.Item(DependentUpon);
                if (property != null)
                {
                    property.Value = fileName.Remove(fileName.Length - Partial.Length, Partial.Length - Path.GetExtension(fileName).Length);
                }
            }
        }

        #endregion
    }
}
