﻿using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace eStation_PTL_Demo
{
    /// <summary>
    /// Interaction logic for TagList.xaml
    /// </summary>
    public partial class TagList : Window
    {
        private List<string> _tagIDList = [];

        public delegate void UpdateTagIDList(List<string> tagIDList);
        public event UpdateTagIDList UpdateTagIDListHandler;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="list"></param>
        public TagList(List<string> list)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            if (list != null) _tagIDList.AddRange(list);
        }

        /// <summary>
        /// Window load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder builder = new();
                foreach (var id in _tagIDList) builder.AppendLine(id);
                txtIDList.Text = builder.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Button save click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reg = new Regex("^[0-9A-F]{12}$");
                var hash = new HashSet<string>();
                var items = txtIDList.Text.Trim().ToUpper().Split('\r');
                var builder = new StringBuilder();
                foreach (var item in items)
                {
                    var id = item.Trim('\n');
                    if (!reg.IsMatch(id) || hash.Contains(id)) continue;
                    hash.Add(id);
                    builder.AppendLine(id);
                }
                File.WriteAllText("TagList.txt", builder.ToString());
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
