using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PgpSharp;
using PgpSharp.Gpg;

namespace AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly GpgTool _pgpTool;
    private readonly IStorageProvider? _storageProvider;

    public MainWindowViewModel()
    {
        _pgpTool = new GpgTool(new GpgOptions());
        ReloadKeys();
        LastError = "Design time sample error";
        LastSuccess = "Design time sample success";
    }

    public MainWindowViewModel(GpgTool pgpTool, IStorageProvider storageProvider)
    {
        _pgpTool = pgpTool;
        _storageProvider = storageProvider;
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

    public ObservableCollection<KeyId> PrivateKeys { get; } = [];
    public ObservableCollection<KeyId> PublicKeys { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private KeyId? _selectedPrivateKey;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private KeyId? _selectedPublicKey;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private string _passphrase = ""; // not secure but whatev

    [ObservableProperty] private bool _alwaysTrustKey = true;

    [ObservableProperty] private string? _lastError;
    [ObservableProperty] private string? _lastSuccess;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    private bool _encrypt = true;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    private bool _sign = true;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    private bool _clearSign;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private bool _decrypt = true;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private bool _verify = true;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(EncryptSignCommand))]
    private string? _clearDataFilePath;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(DecryptVerifyCommand))]
    private string? _cipherDataFilePath;

    private void ReloadKeys()
    {
        try
        {
            LastError = null;
            LastSuccess = null;

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
            LastError = "Failed to load keys: " + ex.Message;
        }
    }

    [RelayCommand]
    private async Task ChooseKeyFolderAsync()
    {
        if (_storageProvider == null) return;

        var chooseOptions = new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Choose keyring folder"
        };
        if (Directory.Exists(KeyringPath))
            chooseOptions.SuggestedStartLocation = await _storageProvider.TryGetFolderFromPathAsync(KeyringPath);

        var newFolder = (await _storageProvider.OpenFolderPickerAsync(chooseOptions)).FirstOrDefault();

        if (newFolder == null) return;

        KeyringPath = newFolder.Path.AbsolutePath;
        ReloadKeys();
    }


    [RelayCommand]
    private async Task ChooseGpgAsync()
    {
        if (_storageProvider == null) return;

        var chooseOptions = new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Any") { Patterns = new[] { "*.*" } }
            },
            AllowMultiple = false,
            Title = "Choose GnuPG executable file"
        };
        if (File.Exists(GpgPath))
            chooseOptions.SuggestedStartLocation =
                await _storageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(GpgPath)!);

        var newFile = (await _storageProvider.OpenFilePickerAsync(chooseOptions)).FirstOrDefault();
        if (newFile == null) return;

        GpgPath = newFile.Path.AbsolutePath;
        ReloadKeys();
    }

    [RelayCommand]
    private async Task ChooseClearFileAsync()
    {
        if (_storageProvider == null) return;

        var newFile = (await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Any") { Patterns = new[] { "*.*" } }
            },
            AllowMultiple = false,
            Title = "Choose an original data file"
        })).FirstOrDefault();
        if (newFile == null) return;

        ClearDataFilePath = newFile.Path.AbsolutePath;
    }

    [RelayCommand]
    private async Task ChooseCipherFileAsync()
    {
        if (_storageProvider == null) return;

        var newFile = (await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Any") { Patterns = new[] { "*.*" } }
            },
            AllowMultiple = false,
            Title = "Choose an processed data file"
        })).FirstOrDefault();
        if (newFile == null) return;

        CipherDataFilePath = newFile.Path.AbsolutePath;
    }

    public bool CanEncryptSign()
    {
        return ClearDataFilePath != null &&
               (Encrypt && SelectedPublicKey != null) ||
               (Sign && SelectedPrivateKey != null) ||
               (ClearSign && SelectedPrivateKey != null);
    }

    [RelayCommand(CanExecute = nameof(CanEncryptSign))]
    private void EncryptSign()
    {
        var srcFile = ClearDataFilePath;
        var outFile = srcFile + ".cipher.txt";

        DataOperation operation = DataOperation.Invalid;
        if (Encrypt) operation |= DataOperation.Encrypt;
        if (Sign) operation |= DataOperation.Sign;
        if (ClearSign) operation |= DataOperation.ClearSign;

        using var pass = MakeSecureString(Passphrase);
        var encryptArg = new FileDataInput
        {
            Armor = true,
            AlwaysTrustPublicKey = AlwaysTrustKey,
            InputFile = srcFile,
            OutputFile = outFile,
            Operation = operation,
            Recipient = SelectedPublicKey?.Id,
            Originator = SelectedPrivateKey?.Id,
            Passphrase = pass
        };
        try
        {
            LastError = null;
            LastSuccess = null;
            _pgpTool.ProcessData(encryptArg);
            LastSuccess = $"Produced file {outFile}";
            // SelectFileInExplorer(outFile);
        }
        catch (Exception ex)
        {
            LastError = "Failed to encrypt: " + ex.Message;
        }
    }

    public bool CanDecryptVerify()
    {
        return CipherDataFilePath != null &&
               (Decrypt && SelectedPrivateKey != null) ||
               (Verify && SelectedPublicKey != null);
    }

    [RelayCommand(CanExecute = nameof(CanDecryptVerify))]
    private void DecryptVerify()
    {
        var srcFile = CipherDataFilePath;

        var name = Path.GetFileName(srcFile).Replace(".cipher.txt", "");
        var outFile = Path.Combine(System.IO.Path.GetDirectoryName(srcFile),
            System.IO.Path.GetFileNameWithoutExtension(name) + ".cleared" +
            System.IO.Path.GetExtension(name));

        // if (File.Exists(outFile))
        // {
        //     if (MessageBox.Show("Decrypted file already exists, overwrite?", "Overwrite Confirmation",
        //             MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
        //     {
        //         return;
        //     }
        // }
        DataOperation operation = DataOperation.Invalid;
        if (Decrypt) operation |= DataOperation.Decrypt;
        if (Verify) operation |= DataOperation.Verify;

        using var pass = MakeSecureString(Passphrase);
        var decryptArg = new FileDataInput
        {
            AlwaysTrustPublicKey = AlwaysTrustKey,
            InputFile = srcFile,
            OutputFile = outFile,
            Operation = operation,
            Recipient = SelectedPrivateKey?.Id,
            Originator = SelectedPublicKey?.Id,
            Passphrase = pass
        };
        try
        {
            LastError = null;
            LastSuccess = null;
            _pgpTool.ProcessData(decryptArg);
            LastSuccess = $"Produced file {outFile}";
            // SelectFileInExplorer(outFile);
        }
        catch (Exception ex)
        {
            LastError = "Failed to decrypt: " + ex.Message;
        }
    }

    static SecureString MakeSecureString(string text)
    {
        SecureString ss = new();
        foreach (var c in text)
        {
            ss.AppendChar(c);
        }

        return ss;
    }
}