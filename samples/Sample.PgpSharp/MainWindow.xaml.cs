using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PgpSharp;
using PgpSharp.GnuPG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Sample.PgpSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPgpTool _tool;

        public MainWindow()
        {
            InitializeComponent();
            _tool = new GnuPGTool();
            exePath.Text = GnuPGConfig.GnuPGExePath;
        }

        private void btnChooseKeyring_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.Title = "Choose Keyring Folder";
                dlg.IsFolderPicker = true;

                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;
                var oldPath = keyringPath.Text;
                if (File.Exists(oldPath))
                {
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(oldPath);
                }
                if (dlg.ShowDialog(this) == CommonFileDialogResult.Ok)
                {
                    keyringPath.Text = dlg.FileName;
                }
            }
        }

        private void btnChooseExe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose GnuPG exe file";
            ofd.Multiselect = false;
            ofd.Filter = "exe files|*.exe";
            var oldPath = exePath.Text;
            if (File.Exists(oldPath))
            {
                ofd.FileName = System.IO.Path.GetFileName(oldPath);
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(oldPath);
            }
            if (ofd.ShowDialog(this).GetValueOrDefault())
            {
                exePath.Text = ofd.FileName;
            }
        }

        private void exePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            GnuPGConfig.GnuPGExePath = exePath.Text;
            ReloadKeys();
        }

        private void keyringPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            _tool.KeyringFolder = keyringPath.Text;
            ReloadKeys();
        }

        private void ReloadKeys()
        {
            try
            {
                pubKeyList.ItemsSource = _tool.ListKeys(KeyTarget.Public).ToList();
                priKeyList.ItemsSource = _tool.ListKeys(KeyTarget.Secret).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load keys: " + ex.Message);
            }
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var key = pubKeyList.SelectedItem as KeyId;
            if (key == null)
            {
                pubKeyList.Focus();
                pubKeyList.IsDropDownOpen = true;
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Choose file to encrypt";
                ofd.Multiselect = false;
                ofd.Filter = "All files|*.*";
                if (ofd.ShowDialog(this).GetValueOrDefault())
                {
                    var srcFile = ofd.FileName;
                    var outFile = srcFile + ".encrypted.txt";

                    var encryptArg = new FileDataInput
                    {
                        Armor = true,
                        AlwaysTrustPublicKey = trustPubKey.IsChecked.GetValueOrDefault(),
                        InputFile = srcFile,
                        OutputFile = outFile,
                        Operation = DataOperation.Encrypt,
                        Recipient = key.Id,
                    };
                    try
                    {
                        _tool.ProcessData(encryptArg);
                        SelectFileInExplorer(outFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to encrypt: " + ex.Message);
                    }
                }
            }
        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var key = priKeyList.SelectedItem as KeyId;
            if (key == null)
            {
                priKeyList.Focus();
                priKeyList.IsDropDownOpen = true;
            }
            else
            {
                var pp = passphrase.SecurePassword;
                if (pp.Length == 0)
                {
                    passphrase.Focus();
                }
                else
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Title = "Choose file to decrypt";
                    ofd.Multiselect = false;
                    ofd.Filter = "All files|*.*";
                    if (ofd.ShowDialog(this).GetValueOrDefault())
                    {
                        var srcFile = ofd.FileName;
                        var outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFile),
                            System.IO.Path.GetFileNameWithoutExtension(srcFile) + ".decrypted" + System.IO.Path.GetExtension(srcFile));

                        if (File.Exists(outFile))
                        {
                            if (MessageBox.Show("Decrypted file already exists, overwrite?", "Overwrite Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                        }

                        var decryptArg = new FileDataInput
                        {
                            InputFile = srcFile,
                            OutputFile = outFile,
                            Operation = DataOperation.Decrypt,
                            Passphrase = pp
                        };
                        try
                        {
                            _tool.ProcessData(decryptArg);
                            SelectFileInExplorer(outFile);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to decrypt: " + ex.Message);
                        }
                    }
                }
            }

        }

        private void SelectFileInExplorer(string outFile)
        {
            using (Process.Start("explorer", string.Format("/select,{0}", outFile))) { }
        }
    }
}
