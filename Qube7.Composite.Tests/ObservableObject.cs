using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qube7.Composite
{
    [TestClass]
    public class ObservableObject_Tests
    {
        #region Methods

        [TestMethod]
        [Description("Given sample observable with single property, when property value has changed, then the event is raised for this property.")]
        public void Test_01()
        {
            Sample_01 sample = new Sample_01();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample.Prop1 = "Value";

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(nameof(Sample_01.Prop1), list[0]);
        }

        [TestMethod]
        [Description("Given sample observable, when notifying change for all properties, then the event is raised with 0-length string.")]
        public void Test_02()
        {
            Sample_02 sample = new Sample_02();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample.Update();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(string.Empty, list[0]);
        }

        [TestMethod]
        [Description("Given sample observable with master and dependent property, when master property value has changed, then the event is raised first for this property and next for dependent property.")]
        public void Test_03()
        {
            Sample_03 sample = new Sample_03();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample.Prop1 = "Value";

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(nameof(Sample_03.Prop1), list[0]);
            Assert.AreEqual(nameof(Sample_03.Prop2), list[1]);
        }

        [TestMethod]
        [Description("Given sample observable with circular dependency between properties, when property value has changed, then the event is raised first for this property and next for other properties in the dependency graph.")]
        public void Test_04()
        {
            Sample_04 sample = new Sample_04();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample.Prop1 = "Value";

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(nameof(Sample_04.Prop1), list[0]);
            Assert.IsTrue(list.Contains(nameof(Sample_04.Prop2)));
            Assert.IsTrue(list.Contains(nameof(Sample_04.Prop3)));
        }

        [TestMethod]
        [Description("Given sample observable with indexer property, when indexer value has changed, then the event is raised for the indexer property.")]
        public void Test_05()
        {
            Sample_05 sample = new Sample_05();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample["key1"] = "Value";

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(Binding.IndexerName, list[0]);
        }

        [TestMethod]
        [Description("Given sample observable with indexer property with non-default name, when indexer value has changed, then the event is raised for the indexer property.")]
        public void Test_06()
        {
            Sample_06 sample = new Sample_06();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample["key1"] = "Value";

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(Binding.IndexerName, list[0]);
        }

        [TestMethod]
        [Description("Given sample observable with master property and dependent indexer property, when master property value has changed, then the event is raised first for this property and next for dependent indexer property.")]
        public void Test_07()
        {
            Sample_07 sample = new Sample_07();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample.Prop1 = "Value";

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(nameof(Sample_07.Prop1), list[0]);
            Assert.AreEqual(Binding.IndexerName, list[1]);
        }

        [TestMethod]
        [Description("Given sample observable with master indexer property and dependent property, when master indexer property value has changed, then the event is raised first for this indexer property and next for dependent property.")]
        public void Test_08()
        {
            Sample_08 sample = new Sample_08();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            sample["key1"] = "Value";

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(Binding.IndexerName, list[0]);
            Assert.AreEqual(nameof(Sample_08.Prop1), list[1]);
        }

        #endregion

        #region Nested types

        public class Sample_01 : ObservableObject
        {
            #region Fields

            private string prop1;

            #endregion

            #region Properties

            public string Prop1
            {
                get { return prop1; }
                set { Set(ref prop1, value); }
            }

            #endregion
        }

        public class Sample_02 : ObservableObject
        {
            #region Methods

            public void Update()
            {
                NotifyChanged(null);
            }

            #endregion
        }

        public class Sample_03 : ObservableObject
        {
            #region Fields

            private string prop1;

            #endregion

            #region Properties

            public string Prop1
            {
                get { return prop1; }
                set { Set(ref prop1, value); }
            }

            [DependsOn(nameof(Prop1))]
            public string Prop2
            {
                get { return prop1?.ToUpper(); }
            }

            #endregion
        }

        public class Sample_04 : ObservableObject
        {
            #region Fields

            private string prop1;

            #endregion

            #region Properties

            [DependsOn(nameof(Prop3))]
            public string Prop1
            {
                get { return prop1; }
                set { Set(ref prop1, value); }
            }

            [DependsOn(nameof(Prop1))]
            public string Prop2
            {
                get { return prop1?.ToUpper(); }
            }

            [DependsOn(nameof(Prop2))]
            public bool Prop3
            {
                get { return string.IsNullOrEmpty(Prop2); }
            }

            #endregion
        }

        public class Sample_05 : ObservableObject
        {
            #region Indexers

            public string this[string key]
            {
                get { return key; }
                set { NotifyChanged(); }
            }

            #endregion
        }

        public class Sample_06 : ObservableObject
        {
            #region Fields

            private new const string IndexerName = "Indexer1";

            #endregion

            #region Indexers

            [IndexerName(IndexerName)]
            public string this[string key]
            {
                get { return key; }
                set { NotifyChanged(); }
            }

            #endregion
        }

        public class Sample_07 : ObservableObject
        {
            #region Fields

            private string prop1;

            #endregion

            #region Properties

            public string Prop1
            {
                get { return prop1; }
                set { Set(ref prop1, value); }
            }

            #endregion

            #region Indexers

            [DependsOn(nameof(Prop1))]
            public string this[string key]
            {
                get { return key; }
                set { NotifyChanged(); }
            }

            #endregion
        }

        public class Sample_08 : ObservableObject
        {
            #region Fields

            private new const string IndexerName = "Indexer1";

            private string prop1;

            #endregion

            #region Properties

            [DependsOn(IndexerName)]
            public string Prop1
            {
                get { return prop1; }
                set { Set(ref prop1, value); }
            }

            #endregion

            #region Indexers

            [IndexerName(IndexerName)]
            public string this[string key]
            {
                get { return key; }
                set { NotifyChanged(); }
            }

            #endregion
        }

        #endregion
    }
}
