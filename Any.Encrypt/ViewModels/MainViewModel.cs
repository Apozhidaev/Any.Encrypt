using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Ap.Encrypt.Mvvm;
using Microsoft.Win32;

namespace Ap.Encrypt.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _status;
        private string _password;
        private string _confirmPassword;
        private readonly ICommand _encryptCommand;
        private readonly ICommand _decryptCommand;

        public MainViewModel()
        {
            _encryptCommand = new DelegateCommand(Encrypt, CanDo);
            _decryptCommand = new DelegateCommand(Decrypt, CanDo);
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

        public ICommand EncryptCommand
        {
            get { return _encryptCommand; }
        }

        public ICommand DecryptCommand
        {
            get { return _decryptCommand; }
        }

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
                        File.WriteAllBytes(String.Format("{0}.edata", ofd.FileName), eContent);
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
                                Path.Combine(Path.GetDirectoryName(ofd.FileName),
                                    Path.GetFileNameWithoutExtension(ofd.FileName)), dContent);
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