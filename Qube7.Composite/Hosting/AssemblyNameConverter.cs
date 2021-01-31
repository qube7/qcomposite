using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Converts a <see cref="String"/> type to a <see cref="AssemblyName"/> type, and vice versa.
    /// </summary>
    internal class AssemblyNameConverter : TypeConverter
    {
        #region Methods

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type that you want to convert from.</param>
        /// <returns><c>true</c> if <paramref name="sourceType"/> is a <see cref="String"/> type; otherwise, <c>false</c>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <returns>An <see cref="Object"/> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string source = value as string;

            if (source != null)
            {
                return new AssemblyName(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type that you want to convert to.</param>
        /// <returns><c>true</c> if <paramref name="destinationType"/> is of type <see cref="String"/>, or <see cref="InstanceDescriptor"/>; otherwise, <c>false</c>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts a given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. If <c>null</c> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>An <see cref="Object"/> that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            AssemblyName source = value as AssemblyName;

            if (source != null)
            {
                if (destinationType == typeof(string))
                {
                    return source.FullName;
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    if (string.IsNullOrEmpty(source.FullName))
                    {
                        ConstructorInfo constructor = typeof(AssemblyName).GetConstructor(Array.Empty<Type>());
                        if (constructor != null)
                        {
                            return new InstanceDescriptor(constructor, Array.Empty<object>());
                        }
                    }
                    else
                    {
                        ConstructorInfo constructor = typeof(AssemblyName).GetConstructor(new Type[] { typeof(string) });
                        if (constructor != null)
                        {
                            return new InstanceDescriptor(constructor, new object[] { source.FullName });
                        }
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
