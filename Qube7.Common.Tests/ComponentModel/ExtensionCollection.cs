using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qube7.ComponentModel
{
    [TestClass]
    public class ExtensionCollection_Tests
    {
        #region Methods

        [TestMethod]
        [Description("Given extension collection, when extension is added, then the extension is attached.")]
        public void Test_01()
        {
            object owner = new object();

            ExtensionCollection<object> collection = new ExtensionCollection<object>(owner);

            List<object> attach = new List<object>();
            List<object> detach = new List<object>();

            Extension extension = new Extension();
            extension.Attached += (s, e) => { attach.Add(e.Data); };
            extension.Detached += (s, e) => { detach.Add(e.Data); };

            collection.Add(extension);

            Assert.AreEqual(1, attach.Count);
            Assert.AreEqual(0, detach.Count);
            Assert.AreEqual(owner, attach[0]);
        }

        [TestMethod]
        [Description("Given extension collection populated with extension, when extension is removed, then the extension is detached.")]
        public void Test_02()
        {
            object owner = new object();

            ExtensionCollection<object> collection = new ExtensionCollection<object>(owner);

            Extension extension = new Extension();

            collection.Add(extension);

            List<object> attach = new List<object>();
            List<object> detach = new List<object>();

            extension.Attached += (s, e) => { attach.Add(e.Data); };
            extension.Detached += (s, e) => { detach.Add(e.Data); };

            collection.Remove(extension);

            Assert.AreEqual(0, attach.Count);
            Assert.AreEqual(1, detach.Count);
            Assert.AreEqual(owner, detach[0]);
        }

        [TestMethod]
        [Description("Given extension collection populated with extension, when attempting to replace extension, then the operation throws exception.")]
        public void Test_03()
        {
            object owner = new object();

            ExtensionCollection<object> collection = new ExtensionCollection<object>(owner);

            Extension extension = new Extension();

            collection.Add(extension);

            try
            {
                collection[0] = new Extension();
            }
            catch (NotSupportedException)
            {
                return;
            }

            Assert.Fail();
        }

        #endregion

        #region Nested types

        public class Extension : Extension<object>
        {
            #region Events

            public event EventHandler<EventArgs<object>> Attached;

            public event EventHandler<EventArgs<object>> Detached;

            #endregion

            #region Methods

            protected override void Attach(object owner)
            {
                Event.Raise(Attached, this, new EventArgs<object>(owner));
            }

            protected override void Detach(object owner)
            {
                Event.Raise(Detached, this, new EventArgs<object>(owner));
            }

            #endregion
        }

        #endregion
    }
}
