using System.Collections.Generic;
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
                set
                {
                    prop1 = value;

                    NotifyChanged(nameof(Prop1));
                }
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
                set
                {
                    prop1 = value;

                    NotifyChanged(nameof(Prop1));
                }
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
                set
                {
                    prop1 = value;

                    NotifyChanged(nameof(Prop1));
                }
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

        #endregion
    }
}
