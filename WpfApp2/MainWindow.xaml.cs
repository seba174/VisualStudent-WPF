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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Reflection;
using Contracts;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Regex csFile = new Regex(@".cs$");
        public ObservableCollection<ErrorListItem> errorListItems { get; private set; }
        List<(bool isEnabled, IPlugin plugin)> plugins;

        public int TextLength { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            errorListItems = new ObservableCollection<ErrorListItem>();
            plugins = new List<(bool, IPlugin)>();
            foreach (string str in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (str.Contains(".dll"))
                    if (TryLoadingPlugin(str))
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Header = plugins[plugins.Count - 1].plugin.Name;
                        menuItem.IsCheckable = true;
                        menuItem.Checked += MenuItem_Checked;
                        menuItem.Unchecked += MenuItem_Unchecked;
                        pluginsMenuItem.Items.Add(menuItem);
                    }
            }
            TextLength = 0;
            DataContext = this;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            int i = pluginsMenuItem.Items.IndexOf((MenuItem)sender);
            plugins[i] = (false, plugins[i].plugin);
            RefreshAllRichTextBoxes();
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            int i = pluginsMenuItem.Items.IndexOf((MenuItem)sender);
            plugins[i] = (true, plugins[i].plugin);
            RefreshAllRichTextBoxes();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is simple C# editor and compiler.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            AddTabControlItem("New File", "", "");
        }

        private void AddTabControlItem(string header, string text, string tag)
        {
            RichTextBox rtb = new RichTextBox();
            rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            rtb.Document.Blocks.Clear();
            rtb.Tag = 0;
            rtb.TextChanged += textChangedEventHandlerActivatePlugins;
            rtb.Document.Blocks.Add(new Paragraph(new Run(text)));
           // rtb.Tag = StringFromRichTextBox(rtb).Length;
            rtb.TextChanged += textChangedEventHandler;
 

            var z = new CloseableTab();

            z.Height = 27;
            z.Title = header;
            z.Content = rtb;
            z.Tag = tag;

            tabControl.Items.Add(z);
            tabControl.SelectedIndex = tabControl.Items.Count - 1;

            var x = tabControl.SelectedIndex;
            var xxx = (CloseableTab)tabControl.Items[x];
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "C# files (*.cs)|*.cs|All|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileText = File.ReadAllText(openFileDialog.FileName);
                AddTabControlItem(openFileDialog.SafeFileName, fileText, openFileDialog.FileName);
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TreeView treeViewParent = new TreeView();
                treeViewParent.Background = Brushes.Transparent;
                treeViewParent.BorderThickness = new Thickness(0);
                TreeViewItem treeView = new TreeViewItem();
                treeView.Header = openFolderDialog.SelectedPath.Substring(openFolderDialog.SelectedPath.LastIndexOf('\\') + 1);
                treeView.Tag = openFolderDialog.SelectedPath.Substring(openFolderDialog.SelectedPath.LastIndexOf('\\') + 1);
                treeView.FontWeight = FontWeights.Normal;
                treeViewParent.Items.Add(treeView);


                bool ok = false;
                foreach (string s in Directory.EnumerateDirectories(openFolderDialog.SelectedPath))
                {
                    TreeViewItem item = new TreeViewItem();

                    item.Header = s.Substring(s.LastIndexOf('\\') + 1);
                    item.Tag = s;
                    item.FontWeight = FontWeights.Normal;

                    FillTreeView(item, s);
                    treeView.Items.Add(item);
                }
                foreach (string str in Directory.EnumerateFiles(openFolderDialog.SelectedPath))
                {
                    TreeViewItem item = new TreeViewItem();
                    string nam = str.Substring(str.LastIndexOf('\\') + 1);
                    if (nam.Contains(".csproj"))
                    {
                        ok = true;
                        treeViewParent.Tag = str;
                    }
                    if (!csFile.IsMatch(nam))
                        continue;
                    item.Header = str.Substring(str.LastIndexOf('\\') + 1);
                    item.MouseDoubleClick += treeItem_DoubleClick;
                    item.Tag = str;
                    item.FontWeight = FontWeights.Normal;
                    treeView.Items.Add(item);
                }
                if (ok)
                    ProjectsList.Items.Add(treeViewParent);
                else
                {
                    ProjectsList.Items.Clear();
                    tabControl.Items.Clear();
                    MessageBox.Show("This is not C# project", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillTreeView(TreeViewItem parentItem, string path)
        {
            foreach (string str in Directory.EnumerateDirectories(path))
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str.Substring(str.LastIndexOf('\\') + 1);
                item.Tag = str;
                item.FontWeight = FontWeights.Normal;
                parentItem.Items.Add(item);
                FillTreeView(item, str);
            }
            foreach (string str in Directory.EnumerateFiles(path))
            {
                TreeViewItem item = new TreeViewItem();
                string nam = str.Substring(str.LastIndexOf('\\') + 1);
                if (!csFile.IsMatch(nam))
                    continue;

                item.MouseDoubleClick += treeItem_DoubleClick;
                item.Header = str.Substring(str.LastIndexOf('\\') + 1);
                item.Tag = str;
                item.FontWeight = FontWeights.Normal;
                parentItem.Items.Add(item);
            }
        }
        void treeItem_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                string str = (string)treeViewItem.Tag;
                string fileText = null;
                try
                {
                    fileText = File.ReadAllText((string)treeViewItem.Tag);
                }
                catch { }

                if (fileText != null)
                {
                    string nam = str.Substring(str.LastIndexOf('\\') + 1);
                    AddTabControlItem(nam, fileText, str);
                }
            }
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_3(null, null);
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var obj = GetCurrentActiveTab();
            if (obj == null)
                return;
            if (obj.StarVisibility == Visibility.Visible && (string)obj.Tag != "")
            {
                Save((string)obj.Tag, obj);
            }
            else if (obj.StarVisibility == Visibility.Visible)
            {
                SaveAs();
            }
        }

        private CloseableTab GetCurrentActiveTab()
        {
            if (tabControl.SelectedIndex >= tabControl.Items.Count || tabControl.SelectedIndex < 0)
                return null;
            var obj = (CloseableTab)tabControl.Items[tabControl.SelectedIndex];
            return obj;
        }

        private void Save(string path, CloseableTab obj)
        {
            File.WriteAllText(path, StringFromRichTextBox((RichTextBox)obj.Content));
            obj.StarVisibility = Visibility.Hidden;
        }

        private void SaveAs()
        {
            var obj = GetCurrentActiveTab();
            if (obj == null)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "C# file (*.cs)|*.cs";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, StringFromRichTextBox((RichTextBox)obj.Content));
                if (obj.StarVisibility == Visibility.Visible)
                {
                    obj.StarVisibility = Visibility.Hidden;
                }
            }
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem_Click_2(null, null);
        }

        private void textChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (tabControl.SelectedIndex >= tabControl.Items.Count || tabControl.SelectedIndex < 0)
                return;
            var par = (CloseableTab)tabControl.Items[tabControl.SelectedIndex];
            par.StarVisibility = Visibility.Visible;
            var rtb = (RichTextBox)par.Content;
            TextRange text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        }

        private void textChangedEventHandlerActivatePlugins(object sender, TextChangedEventArgs args)
        {
            if (sender is RichTextBox rtb)
            {
                TextRange text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                if ((int)rtb.Tag != text.Text.Length)
                {
                    rtb.Tag = text.Text.Length;
                    text.ClearAllProperties();
                    //text.applypropertyvalue(textelement.fontsizeproperty, 12.0);
                    //text.applypropertyvalue(textelement.fontstyleproperty, fontstyles.normal);
                    //text.applypropertyvalue(textelement.fontweightproperty, fontweights.normal);
                    //text.applypropertyvalue(textelement.foregroundproperty, (brush)brushes.black);
                    //text.applypropertyvalue(textelement.backgroundproperty, (brush)brushes.transparent);

                    foreach ((bool isEnabled, IPlugin p) plugin in plugins)
                        if (plugin.isEnabled)
                            plugin.p.Do(rtb);
                }
            }
        }

        private void RefreshAllRichTextBoxes()
        {
            var tab = GetCurrentActiveTab();
            Visibility visibility = Visibility.Visible;
            if (tab != null)
                visibility = tab.StarVisibility;

            foreach (CloseableTab ct in tabControl.Items)
            {
                if (ct == null)
                    continue;
                var tmp = ct.StarVisibility;
                TextRange text = new TextRange(((RichTextBox)ct.Content).Document.ContentStart, ((RichTextBox)ct.Content).Document.ContentEnd);
                text.ClearAllProperties();
                ((RichTextBox)ct.Content).Tag = StringFromRichTextBox((RichTextBox)ct.Content).Length;

                //TextLength = text.Text.Length;
                //text.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);
                //text.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                //text.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                //text.ApplyPropertyValue(TextElement.ForegroundProperty, (Brush)Brushes.Black);
                //text.ApplyPropertyValue(TextElement.BackgroundProperty, (Brush)Brushes.Transparent);

                foreach ((bool isEnabled, IPlugin p) plugin in plugins)
                    if (plugin.isEnabled)
                        plugin.p.Do(((RichTextBox)ct.Content));
                ct.StarVisibility = tmp;
            }
            if (tab != null)
                tab.StarVisibility = visibility;
        }


        private string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        public static bool CanRemoveItemFromTabControl(TabControl tabControl)
        {
            if (tabControl.SelectedIndex >= tabControl.Items.Count || tabControl.SelectedIndex < 0)
                return false;
            var obj = (CloseableTab)tabControl.Items[tabControl.SelectedIndex];
            if (obj == null)
                return false;
            if (obj.StarVisibility == Visibility.Visible)
            {
                var res = MessageBox.Show("Do you want to close unsaved document?", "Close document", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                    return true;
                return false;
            }
            return true;
        }

        private void executeButton_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"\\bin\\Debug\\.+\.exe");
            string str = ((TreeView)ProjectsList.SelectedItem)?.Tag?.ToString();
            if (string.IsNullOrEmpty(str))
                return;
            var ret = Builder.Build(str);
            outputRichTextBox.Text = ret.Item1;

            errorListItems.Clear();
            for (int i = 0; i < ret.Item2.Count / 2; i++)
                errorListItems.Add(new ErrorListItem(ret.Item2[i]));

            if (((ComboBoxItem)comboBox.SelectedItem)?.Content?.ToString() == "Build + run" && ret.Item2.Count == 0)
            {
                Match match = regex.Match(ret.Item1);
                if (match.Success)
                {
                    string s = str.Substring(0, str.LastIndexOf('\\')) + match.Value;
                    var process = Process.Start(s);
                }
            }
        }

        private bool TryLoadingPlugin(string path)
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(path);
                foreach (Type t in asm.GetTypes())
                {
                    foreach (Type iface in t.GetInterfaces())
                    {
                        if (iface.Equals(typeof(IPlugin)))
                        {
                            plugins.Add((false, (IPlugin)Activator.CreateInstance(t)));
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // No problem
                return false;
            }
            return false;
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            executeButton_Click(null, null);
        }
    }
}
