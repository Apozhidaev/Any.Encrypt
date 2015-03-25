using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Any.Encrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnEncryptClick(object sender, RoutedEventArgs e)
        {
            if (passText.Password != confPassText.Password)
            {
                MessageBox.Show("Passwords doesn't match");
                return;
            }
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                using (var file = ofd.OpenFile())
                using (var reader = new BinaryReader(file))
                {
                    var sh = new SecurityHelper(passText.Password);
                    var content = reader.ReadBytes((int)file.Length);
                    var eContent = sh.Encrypt(content);
                    File.WriteAllBytes(String.Format("{0}.edata",ofd.FileName), eContent);
                    MessageBox.Show("Ok");
                }
            }
        }

        private void OnDecryptClick(object sender, RoutedEventArgs e)
        {
            if (passText.Password != confPassText.Password)
            {
                MessageBox.Show("Passwords doesn't match");
                return;
            }
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                using (var file = ofd.OpenFile())
                using (var reader = new BinaryReader(file))
                {
                    var sh = new SecurityHelper(passText.Password);
                    var content = reader.ReadBytes((int)file.Length);
                    var dContent = sh.Decrypt(content);
                    File.WriteAllBytes(String.Format("{0}.ddata", ofd.FileName), dContent);
                    MessageBox.Show("Ok");
                }
            }
        }
    }
}
