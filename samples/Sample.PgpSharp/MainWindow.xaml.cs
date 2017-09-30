using ModernWpf.Messages;
using PgpSharp;
using PgpSharp.GnuPG;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            var msg = new ChooseFolderMessage(newPath => keyringPath.Text = newPath)
            {
                Caption = "Choose Keyring Folder"
            };
            var oldPath = keyringPath.Text;
            if (Directory.Exists(oldPath))
            {
                msg.InitialFolder = System.IO.Path.GetDirectoryName(oldPath);
            }
            msg.HandleWithPlatform(this);

        }

        private void btnChooseExe_Click(object sender, RoutedEventArgs e)
        {
            var msg = new ChooseFileMessage(newFiles => exePath.Text = newFiles.First())
            {
                Caption = "Choose GnuPG exe file",
                Filters = "GnuPG exe file|gpg*.exe",
                Purpose = FilePurpose.OpenSingle,
            };

            var oldPath = exePath.Text;
            if (File.Exists(oldPath))
            {
                msg.InitialFileName = System.IO.Path.GetFileName(oldPath);
                msg.InitialFolder = System.IO.Path.GetDirectoryName(oldPath);
            }

            msg.HandleWithPlatform(this);
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
                var msg = new ChooseFileMessage(newFiles =>
                {
                    var srcFile = newFiles.First();
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
                })
                {
                    Caption = "Choose file to encrypt",
                    Filters = "All files|*.*",
                    Purpose = FilePurpose.OpenSingle,
                };

                msg.HandleWithPlatform(this);
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
                    var msg = new ChooseFileMessage(newFiles =>
                    {
                        var srcFile = newFiles.First();
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
                    })
                    {
                        Caption = "Choose file to decrypt",
                        Filters = "All files|*.*",
                        Purpose = FilePurpose.OpenSingle,
                    };

                    msg.HandleWithPlatform(this);
                }
            }

        }

        private void SelectFileInExplorer(string outFile)
        {
            new OpenExplorerMessage { SelectedPath = outFile }.HandleWithPlatform();
        }
    }
}
