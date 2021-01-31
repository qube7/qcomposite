using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qube7.Composite
{
    [TestClass]
    public class PropertyObserver_1_Tests
    {
        #region Methods

        [TestMethod]
        [Description("Given property observer, when attempting to instantiate without passing source object, then the constructor throws exception.")]
        public void Test_01()
        {
            try
            {
                PropertyObserver<Sample> observer = new PropertyObserver<Sample>(null);

                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("source", e.ParamName);
            }
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when property value has changed, then the callback is executed.")]
        public void Test_02()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(nameof(Sample.Prop1), () => { count++; });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(1, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when property value has changed, then the callback is executed.")]
        public void Test_03()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(nameof(Sample.Prop1), s => { list.Add(s); });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(sample, list[0]);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when notifying change for all properties, then the callback is executed.")]
        public void Test_04()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(nameof(Sample.Prop1), () => { count++; });

            sample.Update();

            Assert.AreEqual(1, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when notifying change for all properties, then the callback is executed.")]
        public void Test_05()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(nameof(Sample.Prop1), s => { list.Add(s); });

            sample.Update();

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(sample, list[0]);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for all properties, when notifying change for all properties, then the callback is executed.")]
        public void Test_06()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(null, () => { count++; });

            sample.Update();

            Assert.AreEqual(1, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for all properties, when notifying change for all properties, then the callback is executed.")]
        public void Test_07()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(null, s => { list.Add(s); });

            sample.Update();

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(sample, list[0]);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for all properties, when property value has changed, then the callback is not executed.")]
        public void Test_08()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(null, () => { count++; });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(0, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for all properties, when property value has changed, then the callback is not executed.")]
        public void Test_09()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(null, s => { list.Add(s); });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(0, list.Count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for any property, when notifying change for all properties, then the callback is executed.")]
        public void Test_10()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(() => { count++; });

            sample.Update();

            Assert.AreEqual(1, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for any property, when notifying change for all properties, then the callback is executed.")]
        public void Test_11()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(s => { list.Add(s); });

            sample.Update();

            Assert.AreEqual(1, list.Count);
            Assert.AreSame(sample, list[0]);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for any property, when property value has changed, then the callback is executed.")]
        public void Test_12()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            int count = 0;

            ISubscriber<Sample> subscriber = observer.Subscribe(() => { count++; });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(2, count);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for any property, when property value has changed, then the callback is executed.")]
        public void Test_13()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            List<Sample> list = new List<Sample>();

            ISubscriber<Sample> subscriber = observer.Subscribe(s => { list.Add(s); });

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreEqual(2, list.Count);
            Assert.AreSame(sample, list[0]);
            Assert.AreSame(sample, list[1]);
            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when the callback is unregistered and property value has changed, then the callback is not executed.")]
        public void Test_14()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            Action callback = () => { Assert.Fail(); };

            observer.Subscribe(nameof(Sample.Prop1), callback);

            ISubscriber<Sample> subscriber = observer.Unsubscribe(nameof(Sample.Prop1), callback);

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given property observer for the sample observable and the callback is registered for property, when the callback is unregistered and property value has changed, then the callback is not executed.")]
        public void Test_15()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            Action<Sample> callback = s => { Assert.Fail(); };

            observer.Subscribe(nameof(Sample.Prop1), callback);

            ISubscriber<Sample> subscriber = observer.Unsubscribe(nameof(Sample.Prop1), callback);

            sample.Prop1 = "Value";
            sample.Prop2 = "Value";

            Assert.AreSame(observer, subscriber);
        }

        [TestMethod]
        [Description("Given disposed property observer, when attempting to access programming interface, then the member call throws exception.")]
        public void Test_16()
        {
            Sample sample = new Sample();

            PropertyObserver<Sample> observer = new PropertyObserver<Sample>(sample);

            observer.Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => observer.Subscribe(nameof(Sample.Prop1), s => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Subscribe(nameof(Sample.Prop1), () => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Subscribe(s => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Subscribe(() => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Unsubscribe(nameof(Sample.Prop1), s => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Unsubscribe(nameof(Sample.Prop1), () => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Unsubscribe(s => { }));
            Assert.ThrowsException<ObjectDisposedException>(() => observer.Unsubscribe(() => { }));
        }

        #endregion

        #region Nested types

        public class Sample : ObservableObject
        {
            #region Fields

            private string prop1;

            private string prop2;

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

            public string Prop2
            {
                get { return prop2; }
                set
                {
                    prop2 = value;

                    NotifyChanged(nameof(Prop2));
                }
            }

            #endregion

            #region Methods

            public void Update()
            {
                NotifyChanged(null);
            }

            #endregion
        }

        #endregion
    }
}
