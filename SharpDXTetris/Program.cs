using System;
using System.Collections.Generic;

namespace SharpDXTetris
{
    /// <summary>
    /// Simple SharpDXTetris application using SharpDX.Toolkit.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
#if NETFX_CORE
        [MTAThread]
#else
        [STAThread]
#endif
        static void Main()
        {
            global::Windows.UI.Xaml.Application.Start((p) => new App());

        }
    }
}