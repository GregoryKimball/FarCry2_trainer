using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FarCry2_trainer
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public bool IsClosed { get; private set; }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }


        public MapWindow()
        {
            InitializeComponent();
        }

        public void DisplayImage(string title, string uri)
        {
            this.Title = title;
            this.image.Source = new BitmapImage(new Uri(uri));
            this.Show();
            this.Activate();

            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Width = height + 20;
            this.Height = height + 40;         

            this.image.Height = height;
            this.image.Width = height;

            this.Top = 0;
        }

    }
}
