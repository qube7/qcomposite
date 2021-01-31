using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Qube7.Composite.Design.Generators
{
    /// <summary>
    /// Provides a base class for the single file generator implementations.
    /// </summary>
    public abstract class BaseCodeGenerator : IVsSingleFileGenerator
    {
        #region Fields

        /// <summary>
        /// The <see cref="IVsGeneratorProgress"/> through which the generator can report its progress to the project system.
        /// </summary>
        private IVsGeneratorProgress progress;

        #endregion

        #region Properties

        /// <summary>
        /// When overridden in a derived class, gets the file extension that is given to the output file name.
        /// </summary>
        /// <value>The file extension.</value>
        protected abstract string DefaultExtension { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCodeGenerator"/> class.
        /// </summary>
        protected BaseCodeGenerator()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the file extension that is given to the output file name.
        /// </summary>
        /// <param name="pbstrDefaultExtension">When this method returns, contains the file extension that is to be given to the output file name.</param>
        /// <returns>A value of zero indicating a successful method call.</returns>
        int IVsSingleFileGenerator.DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = DefaultExtension;

            return 0;
        }

        /// <summary>
        /// Executes the transformation and returns the newly generated output file, whenever a custom tool is loaded, or the input file is saved.
        /// </summary>
        /// <param name="wszInputFilePath">The full path of the input file.</param>
        /// <param name="bstrInputFileContents">The contents of the input file.</param>
        /// <param name="wszDefaultNamespace">The namespace into which the generated code will be placed.</param>
        /// <param name="rgbOutputFileContents">When this method returns, contains an array of bytes to be written to the generated file.</param>
        /// <param name="pcbOutput">When this method returns, contains the count of bytes in the <paramref name="rgbOutputFileContents"/> array.</param>
        /// <param name="pGenerateProgress">The <see cref="IVsGeneratorProgress"/> through which the generator can report its progress to the project system.</param>
        /// <returns>A value of zero indicating a successful method call.</returns>
        int IVsSingleFileGenerator.Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
                progress = pGenerateProgress;

                byte[] bytes = Generate(bstrInputFileContents, wszInputFilePath, wszDefaultNamespace);

                int length = bytes.Length;

                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);

                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);

                pcbOutput = (uint)length;

                return 0;
            }
            finally
            {
                progress = null;
            }
        }

        /// <summary>
        /// Executes the transformation and returns the contents of the generated output file.
        /// </summary>
        /// <param name="inputText">The contents of the input file.</param>
        /// <param name="filePath">The full path of the input file.</param>
        /// <param name="defaultNamespace">The namespace into which the generated code will be placed.</param>
        /// <returns>The contents of the output file.</returns>
        private byte[] Generate(string inputText, string filePath, string defaultNamespace)
        {
            string result = null;

            try
            {
                result = GenerateCode(inputText, filePath, defaultNamespace);
            }
            catch (Exception e)
            {
                GenerateError(e.Message);
            }

            return Encoding.UTF8.GetBytes(result ?? string.Empty);
        }

        /// <summary>
        /// When overridden in a derived class, executes the transformation and returns the contents of the generated output file.
        /// </summary>
        /// <param name="inputText">The contents of the input file.</param>
        /// <param name="filePath">The full path of the input file.</param>
        /// <param name="defaultNamespace">The namespace into which the generated code will be placed.</param>
        /// <returns>The contents of the output file.</returns>
        protected abstract string GenerateCode(string inputText, string filePath, string defaultNamespace);

        /// <summary>
        /// Returns the error information to the project system.
        /// </summary>
        /// <param name="message">The text message of the error to be displayed to the user.</param>
        /// <param name="line">The zero-based line number that indicates where in the source file the error occurred.</param>
        /// <param name="column">The one-based column number that indicates where in the source file the error occurred.</param>
        protected void GenerateError(string message, int line, int column)
        {
            progress?.GeneratorError(0, 0, message, (uint)line, (uint)column);
        }

        /// <summary>
        /// Returns the error information to the project system.
        /// </summary>
        /// <param name="message">The text message of the error to be displayed to the user.</param>
        /// <param name="line">The zero-based line number that indicates where in the source file the error occurred.</param>
        protected void GenerateError(string message, int line)
        {
            progress?.GeneratorError(0, 0, message, (uint)line, 0xFFFFFFFF);
        }

        /// <summary>
        /// Returns the error information to the project system.
        /// </summary>
        /// <param name="message">The text message of the error to be displayed to the user.</param>
        protected void GenerateError(string message)
        {
            progress?.GeneratorError(0, 0, message, 0xFFFFFFFF, 0xFFFFFFFF);
        }

        /// <summary>
        /// Returns the warning information to the project system.
        /// </summary>
        /// <param name="message">The text message of the warning to be displayed to the user.</param>
        /// <param name="line">The zero-based line number that indicates where in the source file the warning occurred.</param>
        /// <param name="column">The one-based column number that indicates where in the source file the warning occurred.</param>
        protected void GenerateWarning(string message, int line, int column)
        {
            progress?.GeneratorError(1, 0, message, (uint)line, (uint)column);
        }

        /// <summary>
        /// Returns the warning information to the project system.
        /// </summary>
        /// <param name="message">The text message of the warning to be displayed to the user.</param>
        /// <param name="line">The zero-based line number that indicates where in the source file the warning occurred.</param>
        protected void GenerateWarning(string message, int line)
        {
            progress?.GeneratorError(1, 0, message, (uint)line, 0xFFFFFFFF);
        }

        /// <summary>
        /// Returns the warning information to the project system.
        /// </summary>
        /// <param name="message">The text message of the warning to be displayed to the user.</param>
        protected void GenerateWarning(string message)
        {
            progress?.GeneratorError(1, 0, message, 0xFFFFFFFF, 0xFFFFFFFF);
        }

        /// <summary>
        /// Sets an index that specifies how much of the generation has been completed.
        /// </summary>
        /// <param name="complete">An index that specifies how much of the generation has been completed.</param>
        /// <param name="total">The maximum value for the <paramref name="complete"/>.</param>
        protected void ReportProgress(int complete, int total)
        {
            progress?.Progress((uint)complete, (uint)total);
        }

        #endregion
    }
}
