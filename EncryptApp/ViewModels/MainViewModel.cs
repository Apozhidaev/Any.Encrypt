using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;

namespace EncryptApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _status;
        private string _password;
        private string _confirmPassword;

        public MainViewModel()
        {
            EncryptCommand = new DelegateCommand(Encrypt, CanDo);
            DecryptCommand = new DelegateCommand(Decrypt, CanDo);
        }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }

        public ICommand EncryptCommand { get; }

        public ICommand DecryptCommand { get; }

        private async void Encrypt()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                Status = "Encrypting...";
                await Task.Run(() =>
                {
                    using (var file = ofd.OpenFile())
                    using (var reader = new BinaryReader(file))
                    {
                        var sh = new SecurityHelper();
                        var content = reader.ReadBytes((int)file.Length);
                        var eContent = sh.Encrypt(content, Password);
                        File.WriteAllBytes($"{ofd.FileName}.edata", eContent);
                        Status = "Ok";
                    }
                });
            }
            
            
        }

        private async void Decrypt()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                Status = "Decrypting...";
                await Task.Run(() =>
                {
                    using (var file = ofd.OpenFile())
                    using (var reader = new BinaryReader(file))
                    {
                        var sh = new SecurityHelper();
                        var content = reader.ReadBytes((int) file.Length);
                        try
                        {
                            var dContent = sh.Decrypt(content, Password);
                            File.WriteAllBytes(
                                Path.Combine(Path.GetDirectoryName(ofd.FileName) ?? "",
                                    Path.GetFileNameWithoutExtension(ofd.FileName) ?? ""), dContent);
                            Status = "Ok";
                        }
                        catch (Exception)
                        {
                            Status = "Password is not correct";
                        }
                    }
                });
            }
        }

        private bool CanDo()
        {
            var passwordEmpty = String.IsNullOrEmpty(Password);
            var passwordsMatch = Password == ConfirmPassword;
            if (passwordEmpty)
            {
                Status = "Password is empty";
            }
            else if (!passwordsMatch)
            {
                Status = "Passwords aren't match";
            }
            else if (Status == "Password is empty" || Status == "Passwords aren't match")
            {
                Status = "";
            }
            return !passwordEmpty && passwordsMatch;
        }

    }
}