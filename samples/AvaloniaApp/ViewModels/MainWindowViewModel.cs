using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PgpSharp;
using PgpSharp.Gpg;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly GpgTool _pgpTool;
    
    public MainWindowViewModel()
    {
    }
    
    public MainWindowViewModel(GpgTool pgpTool)
    {
        _pgpTool = pgpTool;
        ReloadKeys();
    }
    
    public string? GpgPath
    {
        get => _pgpTool.Options.GpgPath;
        set
        {
            _pgpTool.Options.GpgPath = value;
            OnPropertyChanged();
        }
    }
    
    public string? KeyringPath
    {
        get => _pgpTool.Options.KeyringFolder;
        set
        {
            _pgpTool.Options.KeyringFolder = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<KeyId> PrivateKeys { get; } = new ObservableCollection<KeyId>();
    public ObservableCollection<KeyId> PublicKeys { get; } = new ObservableCollection<KeyId>();
    
    [ObservableProperty] private KeyId? _selectedPrivateKey;
    [ObservableProperty] private KeyId? _selectedPublicKey;
    [ObservableProperty] private string? _passphrase; // not secure but whatev
    [ObservableProperty] private bool _alwaysTrustKey = true;
    
    private void ReloadKeys()
    {
        try
        {
            PublicKeys.Clear();
            foreach (var key in _pgpTool.ListKeys(KeyTarget.Public))
            {
                PublicKeys.Add(key);
            }
    
            if (SelectedPublicKey != null)
                SelectedPublicKey = PublicKeys.FirstOrDefault(k => k.Id == SelectedPublicKey.Id);
    
            PrivateKeys.Clear();
            foreach (var key in _pgpTool.ListKeys(KeyTarget.Secret))
            {
                PrivateKeys.Add(key);
            }
    
            if (SelectedPrivateKey != null)
                SelectedPrivateKey = PrivateKeys.FirstOrDefault(k => k.Id == SelectedPrivateKey.Id);
        }
        catch (Exception ex)
        {
            // MessageBox.Show("Failed to load keys: " + ex.Message);
        }
    }
    
    [RelayCommand]
    private void Encrypt()
    {
        if (SelectedPublicKey == null) return;
    
        // var msg = new ChooseFileMessage(newFiles =>
        // {
        //     var srcFile = newFiles.First();
        //     var outFile = srcFile + ".encrypted.txt";
        //
        //     var encryptArg = new FileDataInput
        //     {
        //         Armor = true,
        //         AlwaysTrustPublicKey = trustPubKey.IsChecked.GetValueOrDefault(),
        //         InputFile = srcFile,
        //         OutputFile = outFile,
        //         Operation = DataOperation.Encrypt,
        //         Recipient = key.Id,
        //     };
        //     try
        //     {
        //         _tool.ProcessData(encryptArg);
        //         SelectFileInExplorer(outFile);
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show("Failed to encrypt: " + ex.Message);
        //     }
        // })
        // {
        //     Caption = "Choose file to encrypt",
        //     Filters = "All files|*.*",
        //     Purpose = FilePurpose.OpenSingle,
        // };
        //
        // msg.HandleWithPlatform(this);
    }
    
    [RelayCommand]
    private void Decrypt()
    {
        if (SelectedPrivateKey == null ||
            string.IsNullOrEmpty(Passphrase)) return;
    
    
        // var msg = new ChooseFileMessage(newFiles =>
        // {
        //     var srcFile = newFiles.First();
        //     var outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(srcFile),
        //         System.IO.Path.GetFileNameWithoutExtension(srcFile) + ".decrypted" +
        //         System.IO.Path.GetExtension(srcFile));
        //
        //     if (File.Exists(outFile))
        //     {
        //         if (MessageBox.Show("Decrypted file already exists, overwrite?", "Overwrite Confirmation",
        //                 MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        //         {
        //             return;
        //         }
        //     }
        //
        //     var decryptArg = new FileDataInput
        //     {
        //         InputFile = srcFile,
        //         OutputFile = outFile,
        //         Operation = DataOperation.Decrypt,
        //         Passphrase = pp
        //     };
        //     try
        //     {
        //         _tool.ProcessData(decryptArg);
        //         SelectFileInExplorer(outFile);
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show("Failed to decrypt: " + ex.Message);
        //     }
        // })
        // {
        //     Caption = "Choose file to decrypt",
        //     Filters = "All files|*.*",
        //     Purpose = FilePurpose.OpenSingle,
        // };
        //
        // msg.HandleWithPlatform(this);
    }


    public string Greeting => "Welcome to Avalonia!";
}