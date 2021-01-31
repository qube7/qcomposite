using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qube7.Composite.Presentation
{
    [TestClass]
    public class ViewModel_Tests
    {
        #region Methods

        [TestMethod]
        [Description("Given sample view model, when culture has changed, then the event is raised for culture property.")]
        public void Test_01()
        {
            Sample_01 sample = new Sample_01();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            Culture.Current = CultureInfo.InvariantCulture;

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(nameof(Sample_01.Culture), list[0]);
        }

        [TestMethod]
        [Description("Given sample view model with localized property, when culture has changed, then the event is raised for culture property and localized property.")]
        public void Test_02()
        {
            Sample_02 sample = new Sample_02();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };

            Culture.Current = CultureInfo.InvariantCulture;

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(nameof(Sample_02.Culture), list[0]);
            Assert.AreEqual(nameof(Sample_02.Prop1), list[1]);
        }

        [TestMethod]
        [Description("Given disposed sample view model, when culture has changed, then the event is not raised.")]
        public void Test_03()
        {
            Sample_03 sample = new Sample_03();

            List<string> list = new List<string>();

            sample.PropertyChanged += (s, e) => { list.Add(e.PropertyName); };
            Disposable.Dispose(sample);

            Culture.Current = CultureInfo.InvariantCulture;

            Assert.AreEqual(0, list.Count);
        }

        #endregion

        #region Nested types

        public class Sample_01 : ViewModel
        {
        }

        public class Sample_02 : ViewModel
        {
            #region Properties

            [Localized]
            public string Prop1
            {
                get { return null; }
            }

            #endregion
        }

        public class Sample_03 : ViewModel
        {
        }

        #endregion
    }
}
