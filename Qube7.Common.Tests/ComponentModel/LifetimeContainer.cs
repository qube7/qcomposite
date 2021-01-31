using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qube7.ComponentModel
{
    [TestClass]
    public class LifetimeContainer_Tests
    {
        #region Methods

        [TestMethod]
        [Description("Given lifetime container populated with sample objects, when container is disposed, then the contained objects are disposed in the reverse order.")]
        public void Test_01()
        {
            LifetimeContainer container = new LifetimeContainer();

            List<object> list = new List<object>();

            Random random = new Random();

            for (int i = 0; i < random.Next(5, 100); i++)
            {
                if (random.Next() % 2 == 0)
                {
                    container.Add(new Disposable(list));

                    continue;
                }

                container.Add(new object());
            }

            object[] array = container.ToArray();

            container.Dispose();

            int count = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] is IDisposable)
                {
                    count++;

                    Assert.IsTrue(list.Count >= count);
                    Assert.AreSame(array[i], list[list.Count - count]);
                }
            }
        }

        #endregion

        #region Nested types

        public class Disposable : IDisposable
        {
            #region Fields

            private readonly List<object> list;

            #endregion

            #region Constructors

            public Disposable(List<object> list)
            {
                this.list = list;
            }

            #endregion

            #region Methods

            public void Dispose()
            {
                list.Add(this);
            }

            #endregion
        }

        #endregion
    }
}
