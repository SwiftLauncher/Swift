﻿using System;
using System.Windows;

namespace Swift
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SwiftProgram
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            App.Main(); // Run WPF startup code.
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
