using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;

namespace Qube7.Composite.Design.Wizards
{
    /// <summary>
    /// Represents the Controller template wizard extension.
    /// </summary>
    public class Controller : Wizard
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller()
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
            string safeName = Convention.SafeName(replacementsDictionary, nameof(Controller));
            if (safeName != null)
            {
                replacementsDictionary[Constants.SafeName] = string.Format(safeName, string.Empty);
            }
        }

        #endregion
    }
}
