using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SharpDXTetris
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly SharpDXTetris game;

        public MainPage()
        {
            this.InitializeComponent();
            game = new SharpDXTetris();
            game.Page = this;
            this.Loaded += (sender, args) => game.Run(this);
        }

        private void NewGame(object sender, RoutedEventArgs e)
        {
            game.NewGame();
        }

        private void Left(object sender, RoutedEventArgs e)
        {
            game.Left();
        }
        private void Right(object sender, RoutedEventArgs e)
        {
            game.Right();
        }
    }
}
