About PgpSharp
==========================

This project is an attempt at providing very basic PGP functionality 
through a C# interface, including listing keys, encrypting/signing
data, and decrypting data. It does not generate or manage keys at the moment.


Get it on NuGet
--------------------------
See the [package page](http://www.nuget.org/packages/pgpsharp), or run
```
Install-Package PgpSharp
```

Using the lib
--------------------------
The main interface to use is IPgpTool. The lib provides a default
impelementation that merely wraps around the [GnuPG](https://gnupg.org/) 
binary for windows and can be instantiated like this:

```
IPgpTool tool = new GnuPGTool();
```

By default the GnuPGTool will try to find the GnuPG binary in the standard install
folder. To override the binary file path set this property:

```
GnuPGConfig.GnuPGExePath = @"your\path\to\gpg2.exe";
```

To encrypt/sign/decrypt data, you would use the ProcessData() method
of the interface, and you have the option of using the StreamDataInput
(for processing streams) or FileDataInput (for processing files).

```
// example encrypting a file
var encryptArg = new FileDataInput
{
    Armor = true,
    InputFile = @"path\to\src\file",
    OutputFile = @"path\to\output\file",
    Operation = DataOperation.Encrypt,
    Recipient = "recipient's key id",
};
tool.ProcessData(encryptArg);

// example decrypting a file
SecureString yourPassphrase = ... // somehow generate it
var decryptArg = new FileDataInput
{
    InputFile = @"path\to\encrypted\file",
    OutputFile = @"path\to\output\file",
    Operation = DataOperation.Decrypt,
    Passphrase = yourPassphrase
};
tool.ProcessData(decryptArg);



```

Details on which properties to use for different sign/encrypt/decrypt operation 
is available in the intellisense of that property. The sample WPF project also allows
you to play with the different operations.