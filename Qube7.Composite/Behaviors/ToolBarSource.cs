using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Qube7.ComponentModel;

namespace Qube7.Composite.Behaviors
{
    /// <summary>
    /// Provides the attached behavior that populates the <see cref="ToolBarTray.ToolBars"/> collection of the associated <see cref="ToolBarTray"/>.
    /// </summary>
    public static class ToolBarSource
    {
        #region Fields

        /// <summary>
        /// Identifies the <see cref="P:Source"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(IEnumerable), typeof(ToolBarSource), new PropertyMetadata(OnSourceChanged));

        /// <summary>
        /// Identifies the <see cref="P:Behavior"/> attached dependency property.
        /// </summary>
        private static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached("Behavior", typeof(SourceBehavior), typeof(ToolBarSource));

        #endregion

        #region Methods

        /// <summary>
        /// Called when the effective value of the <see cref="SourceProperty"/> changes.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolBarTray tray = d as ToolBarTray;
            if (tray != null)
            {
                SourceBehavior behavior = tray.ReadLocalValue(BehaviorProperty) as SourceBehavior;
                if (behavior == null)
                {
                    behavior = new SourceBehavior(tray);

                    tray.SetValue(BehaviorProperty, behavior);
                }

                behavior.OnSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
            }
        }

        /// <summary>
        /// Gets a collection used to populate the content of the specified <see cref="ToolBarTray"/>.
        /// </summary>
        /// <param name="tray">The <see cref="ToolBarTray"/> for which to get the collection.</param>
        /// <returns>A collection that is used to populate the content of the <paramref name="tray"/>.</returns>
        public static IEnumerable GetSource(ToolBarTray tray)
        {
            Requires.NotNull(tray, nameof(tray));

            return (IEnumerable)tray.GetValue(SourceProperty);
        }

        /// <summary>
        /// Sets a collection used to populate the content of the specified <see cref="ToolBarTray"/>.
        /// </summary>
        /// <param name="tray">The <see cref="ToolBarTray"/> for which to set the collection.</param>
        /// <param name="value">A collection to use to populate the content of the <paramref name="tray"/>.</param>
        public static void SetSource(ToolBarTray tray, IEnumerable value)
        {
            Requires.NotNull(tray, nameof(tray));

            tray.SetValue(SourceProperty, value);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Populates the <see cref="ToolBarTray.ToolBars"/> collection of the associated <see cref="ToolBarTray"/>.
        /// </summary>
        private class SourceBehavior : IEventListener<NotifyCollectionChangedEventArgs>
        {
            #region Fields

            /// <summary>
            /// The behavior associated object.
            /// </summary>
            private readonly ToolBarTray target;

            /// <summary>
            /// The source collection.
            /// </summary>
            private readonly List<ToolBar> source = new List<ToolBar>();

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SourceBehavior"/> class.
            /// </summary>
            /// <param name="target">The behavior associated object.</param>
            internal SourceBehavior(ToolBarTray target)
            {
                this.target = target;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called when the value of the <see cref="SourceProperty"/> changes.
            /// </summary>
            /// <param name="oldValue">The old value of the property.</param>
            /// <param name="newValue">The new value of the property.</param>
            internal void OnSourceChanged(IEnumerable oldValue, IEnumerable newValue)
            {
                if (oldValue != newValue)
                {
                    INotifyCollectionChanged collection1 = oldValue as INotifyCollectionChanged;
                    if (collection1 != null)
                    {
                        CollectionChangedEvent.RemoveListener(collection1, this);
                    }

                    OnItemsReset(newValue);

                    INotifyCollectionChanged collection2 = newValue as INotifyCollectionChanged;
                    if (collection2 != null)
                    {
                        CollectionChangedEvent.AddListener(collection2, this);
                    }
                }
            }

            /// <summary>
            /// Called when the subscribed event occurs.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
            void IEventListener<NotifyCollectionChangedEventArgs>.OnEvent(object sender, NotifyCollectionChangedEventArgs e)
            {
                target.VerifyAccess();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        VerifyRange(e.NewItems);
                        OnItemAdded(e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        VerifyRange(e.OldItems);
                        OnItemRemoved(e.OldItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        VerifyRange(e.OldItems);
                        VerifyRange(e.NewItems);
                        OnItemReplaced(e.OldItems[0], e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        VerifyRange(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        OnItemsReset(sender as IEnumerable);
                        break;
                }
            }

            /// <summary>
            /// Verifies whether the collection change is single item change.
            /// </summary>
            /// <param name="items">The list of items involved in the change.</param>
            private static void VerifyRange(IList items)
            {
                if (items.Count == 1)
                {
                    return;
                }

                throw Error.NotSupported(Strings.RangeAction);
            }

            /// <summary>
            /// Called when an item was added to the collection.
            /// </summary>
            /// <param name="item">The item that was added to the collection.</param>
            private void OnItemAdded(object item)
            {
                ToolBar control = item as ToolBar;
                if (control != null)
                {
                    source.Add(control);

                    target.ToolBars.Add(control);
                }
            }

            /// <summary>
            /// Called when an item was removed from the collection.
            /// </summary>
            /// <param name="item">The item that was removed from the collection.</param>
            private void OnItemRemoved(object item)
            {
                ToolBar control = item as ToolBar;
                if (control != null)
                {
                    target.ToolBars.Remove(control);

                    source.Remove(control);
                }
            }

            /// <summary>
            /// Called when an item was replaced in the collection.
            /// </summary>
            /// <param name="oldItem">The item that was replaced.</param>
            /// <param name="newItem">The item that replaced <paramref name="oldItem"/>.</param>
            private void OnItemReplaced(object oldItem, object newItem)
            {
                if (oldItem != newItem)
                {
                    OnItemRemoved(oldItem);

                    OnItemAdded(newItem);
                }
            }

            /// <summary>
            /// Called when the contents of the collection changed dramatically.
            /// </summary>
            /// <param name="items">The source collection that was reset.</param>
            private void OnItemsReset(IEnumerable items)
            {
                if (source.Count > 0)
                {
                    for (int i = source.Count - 1; i >= 0; i--)
                    {
                        target.ToolBars.Remove(source[i]);
                    }

                    source.Clear();
                }

                if (items != null)
                {
                    foreach (object item in items)
                    {
                        OnItemAdded(item);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
