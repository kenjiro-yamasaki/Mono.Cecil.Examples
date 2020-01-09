using System;

namespace SoftCube
{
    /// <summary>
    /// クラスのサンプル。
    /// </summary>
    class Class
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        internal Class()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メソッドのサンプル。
        /// </summary>
        internal void Method()
        {
            Console.WriteLine("Method");
        }

        /// <summary>
        /// 仮想メソッドのサンプル。
        /// </summary>
        internal virtual void VirtualMethod()
        {
            Console.WriteLine("Virtual Method");
        }

        #endregion
    }

    class DerivedClass : Class
    {
        internal override void VirtualMethod()
        {

            Console.WriteLine("Virtual Method (override)");
        }
    }

}
