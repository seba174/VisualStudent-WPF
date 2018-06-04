using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public class CloseableTab : TabItem
    {
        public string Title
        {
            get { return (string)((CloseableHeader)this.Header).label_TabTitle.Text; }
            set
            {
                ((CloseableHeader)this.Header).label_TabTitle.Text = value;
            }
        }

        public Visibility StarVisibility
        {
            get { return ((CloseableHeader)this.Header).star.Visibility; }
            set
            {
                ((CloseableHeader)this.Header).star.Visibility = value;
            }
        }

        public CloseableTab() : base()
        {
            this.Header = new CloseableHeader();
            StarVisibility = Visibility.Hidden;
            ((CloseableHeader)this.Header).button_close.Click += new RoutedEventHandler(button_close_Click);

        }

        void button_close_Click(object sender, RoutedEventArgs e)
        {
            ((TabControl)this.Parent).SelectedItem = this;
            if (MainWindow.CanRemoveItemFromTabControl((TabControl)this.Parent))
            {
                ((TabControl)this.Parent).Items.Remove(this);
            }
        }
    }
}
