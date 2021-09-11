using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Qube7.ComponentModel;
using Qube7.Composite.Data;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the view model implementations.
    /// </summary>
    public abstract class ViewModel : ObservableObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// The <see cref="Culture.Current"/> property changed event listener.
        /// </summary>
        private readonly CultureChangedListener listener;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> object that represents the current user interface culture.
        /// </summary>
        /// <value>The current user interface culture.</value>
        public CultureInfo Culture
        {
            get { return Composite.Culture.Current; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        protected ViewModel()
        {
            listener = new CultureChangedListener(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the <see cref="DataTemplate"/> for the view template defined with the specified name.
        /// </summary>
        /// <param name="name">The name of the view template to select.</param>
        /// <returns>The selected <see cref="DataTemplate"/>.</returns>
        internal DataTemplate ProvideTemplate(string name)
        {
            return TemplateManager.ProvideTemplate(name, GetType());
        }

        /// <summary>
        /// Raises the <see cref="ObservableObject.PropertyChanged"/> event for the <see cref="Culture"/> property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnCultureChanged()
        {
            NotifyChanged(nameof(Culture));
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ViewModel"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="ViewModel"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                listener.Dispose();
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Defines the view template for a <see cref="ViewModel"/> derived class.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        protected sealed class ViewAttribute : Attribute
        {
            #region Properties

            /// <summary>
            /// Gets the name of defined view template.
            /// </summary>
            /// <value>The name of the view template.</value>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the type of the control for the view template.
            /// </summary>
            /// <value>The type of the view control.</value>
            public Type Type { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewAttribute"/> class.
            /// </summary>
            /// <param name="name">The name of the view template.</param>
            /// <param name="type">The type of the view control.</param>
            public ViewAttribute(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewAttribute"/> class.
            /// </summary>
            /// <param name="type">The type of the view control.</param>
            public ViewAttribute(Type type) : this(null, type)
            {
            }

            #endregion
        }

        /// <summary>
        /// Provides the <see cref="DataTemplate"/> object for defined view template.
        /// </summary>
        private class TemplateProvider
        {
            #region Fields

            /// <summary>
            /// The type of the view control.
            /// </summary>
            private readonly Type viewType;

            /// <summary>
            /// The type of the <see cref="ViewModel"/>.
            /// </summary>
            private readonly Type modelType;

            /// <summary>
            /// The cached <see cref="DataTemplate"/> object.
            /// </summary>
            private DataTemplate template;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the name of defined view template.
            /// </summary>
            /// <value>The name of the view template.</value>
            internal string Name { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TemplateProvider"/> class.
            /// </summary>
            /// <param name="name">The name of the view template.</param>
            /// <param name="viewType">The type of the view control.</param>
            /// <param name="modelType">The type of the <see cref="ViewModel"/>.</param>
            internal TemplateProvider(string name, Type viewType, Type modelType)
            {
                Name = name;

                this.viewType = viewType;
                this.modelType = modelType;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns the <see cref="DataTemplate"/> for defined view template.
            /// </summary>
            /// <returns>The generated <see cref="DataTemplate"/>.</returns>
            internal DataTemplate ProvideTemplate()
            {
                if (template == null && viewType != null)
                {
                    template = TemplateFactory.CreateTemplate(viewType);

                    template.DataType = modelType;

                    template.Seal();
                }

                return template;
            }

            #endregion
        }

        /// <summary>
        /// Manages defined <see cref="DataTemplate"/> objects for the <see cref="ViewModel"/>.
        /// </summary>
        private class TemplateManager : KeyedCollection<string, TemplateProvider>
        {
            #region Fields

            /// <summary>
            /// The template managers table.
            /// </summary>
            private static readonly ConditionalWeakTable<Type, TemplateManager> table = new ConditionalWeakTable<Type, TemplateManager>();

            /// <summary>
            /// The default template provider.
            /// </summary>
            private readonly TemplateProvider provider;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TemplateManager"/> class.
            /// </summary>
            /// <param name="type">The type of the <see cref="ViewModel"/>.</param>
            private TemplateManager(Type type) : base(StringComparer.InvariantCulture, 4)
            {
                foreach (ViewAttribute view in type.GetCustomAttributes<ViewAttribute>(false))
                {
                    if (string.IsNullOrEmpty(view.Name))
                    {
                        if (provider == null)
                        {
                            provider = new TemplateProvider(null, view.Type, type);
                        }

                        continue;
                    }

                    if (Contains(view.Name))
                    {
                        continue;
                    }

                    Add(new TemplateProvider(view.Name, view.Type, type));
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns the <see cref="DataTemplate"/> for the view template defined with the specified name.
            /// </summary>
            /// <param name="name">The name of the view template to select.</param>
            /// <param name="type">The type of the <see cref="ViewModel"/>.</param>
            /// <returns>The selected <see cref="DataTemplate"/>.</returns>
            internal static DataTemplate ProvideTemplate(string name, Type type)
            {
                TemplateManager manager = table.GetValue(type, t => new TemplateManager(t));

                return manager.ProvideTemplate(name);
            }

            /// <summary>
            /// Returns the <see cref="DataTemplate"/> for the view template defined with the specified name.
            /// </summary>
            /// <param name="name">The name of the view template to select.</param>
            /// <returns>The selected <see cref="DataTemplate"/>.</returns>
            private DataTemplate ProvideTemplate(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return provider?.ProvideTemplate();
                }

                if (TryGetValue(name, out TemplateProvider item))
                {
                    return item.ProvideTemplate();
                }

                return provider?.ProvideTemplate();
            }

            /// <summary>
            /// Extracts the key from the specified element.
            /// </summary>
            /// <param name="item">The element from which to extract the key.</param>
            /// <returns>The key for the specified element.</returns>
            protected override string GetKeyForItem(TemplateProvider item)
            {
                return item.Name;
            }

            #endregion
        }

        /// <summary>
        /// Indicates that the value of the property is culture-sensitive.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = false)]
        protected sealed class LocalizedAttribute : DependsOnAttribute
        {
            #region Properties

            /// <summary>
            /// Gets the names of the related properties declared in this <see cref="LocalizedAttribute"/>.
            /// </summary>
            /// <value>The name of the <see cref="Culture"/> property.</value>
            public override IEnumerable<string> Properties
            {
                get { yield return nameof(Culture); }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LocalizedAttribute"/> class.
            /// </summary>
            public LocalizedAttribute()
            {
            }

            #endregion
        }

        /// <summary>
        /// Represents the <see cref="Culture.Current"/> property changed event listener.
        /// </summary>
        private class CultureChangedListener : EventListener<EventArgs, ViewModel>, IDisposable
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureChangedListener"/> class.
            /// </summary>
            /// <param name="handler">The event handler object.</param>
            internal CultureChangedListener(ViewModel handler) : base(handler)
            {
                CultureChangedEvent.AddListener(this);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Handles the subscribed event using the specified handler object.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
            /// <param name="handler">The event handler object.</param>
            protected override void HandleEvent(object sender, EventArgs e, ViewModel handler)
            {
                handler.OnCultureChanged();
            }

            /// <summary>
            /// Releases all resources used by the <see cref="CultureChangedListener"/>.
            /// </summary>
            public void Dispose()
            {
                CultureChangedEvent.RemoveListener(this);
            }

            #endregion
        }

        #endregion
    }
}
