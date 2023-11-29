using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace VaultUnsealPoc.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly string _clientId;
    private readonly string _tenant;
    
    private List<string> _scopes = new()
    {
        "openid", 
        "profile",
        "offline_access",
        "email",
        "user.read"
    };

    private IPublicClientApplication? _msal;
    [ObservableProperty] private bool loggedIn;

    [ObservableProperty] private string tokenInfo;
    [ObservableProperty] private string accessToken;
    [ObservableProperty] private string identityToken;
    [ObservableProperty] private string yubikey;

    public MainWindowViewModel()
    {
        LoggedIn = false;
        (_clientId, _tenant) = LoadAppConfig();

        GetYubikey();
        SetupAuth();
    }

    private void SetupAuth()
    {
        _msal = PublicClientApplicationBuilder.Create(_clientId)
            .WithAuthority($"{Constants.AzureAd.Instance}{_tenant}")
            .WithRedirectUri("http://localhost")
            .Build();

        // Add token cache serialization
        var storageProperties =
            new StorageCreationPropertiesBuilder(
                    Constants.Keyring.CacheFileName,
                    Constants.Keyring.CacheDir
                )
                .WithLinuxKeyring(
                    Constants.Keyring.LinuxKeyRingSchema,
                    Constants.Keyring.LinuxKeyRingCollection,
                    Constants.Keyring.LinuxKeyRingLabel,
                    Constants.Keyring.LinuxKeyRingAttr1,
                    Constants.Keyring.LinuxKeyRingAttr2)
                .WithMacKeyChain(
                    Constants.Keyring.KeyChainServiceName,
                    Constants.Keyring.KeyChainAccountName)
                .Build();
        var cacheHelper = MsalCacheHelper.CreateAsync(storageProperties).GetAwaiter().GetResult();
        cacheHelper.RegisterCache(_msal.UserTokenCache);
    }

    private void GetYubikey()
    {
        // var key = YubiKeyDevice.FindAll().FirstOrDefault();
        // Yubikey = key is null
        //     ? "No Yubikey found!"
        //     : $"Serial: {key.SerialNumber} | Firmware: {key.FirmwareVersion} | Form factor: {key.FormFactor}";
        Yubikey = "No Yubikey found!";
    }

    [RelayCommand]
    private async Task Login()
    {
        await ClearCache();
        var accounts = (await _msal.GetAccountsAsync()).ToList();

        AuthenticationResult? authResult = null;

        try
        {
            var builder = _msal.AcquireTokenInteractive(_scopes)
                .WithAccount(accounts.FirstOrDefault())
                .WithPrompt(Prompt.SelectAccount);
            if (!_msal.IsEmbeddedWebViewAvailable())
            {
                // You app should install the embedded browser WebView2 https://aka.ms/msal-net-webview2
                // but if for some reason this is not possible, you can fall back to the system browser 
                // in this case, the redirect uri needs to be set to "http://localhost"
                var systemWebViewOptions = new SystemWebViewOptions
                {
                    // iOSHidePrivacyPrompt = true,
                    // OpenBrowserAsync = SystemWebViewOptions.OpenWithChromeEdgeBrowserAsync
                };

                builder.WithSystemWebViewOptions(systemWebViewOptions);
                builder = builder.WithUseEmbeddedWebView(false);
            }

            authResult = await builder.ExecuteAsync();
            LoggedIn = true;
        }
        catch (Exception ex)
        {
            TokenInfo = $"Error Acquiring Token Silently:{Environment.NewLine}{ex}";
            return;
        }

        if (authResult is not null)
        {
            TokenInfo = DumpAuthenticationResult(authResult);
            AccessToken = authResult.AccessToken;
            IdentityToken = authResult.IdToken;
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        await ClearCache();

        LoggedIn = false;
        TokenInfo = string.Empty;
        AccessToken = string.Empty;
        IdentityToken = string.Empty;
    }

    private async Task ClearCache()
    {
        var accounts = (await _msal.GetAccountsAsync()).ToList();

        // clear cache
        while (accounts.Any())
        {
            try
            {
                await _msal.RemoveAsync(accounts.First());
            }
            catch (MsalException ex)
            {
                TokenInfo = $"Error clear cache: {ex.Message}";
                return;
            }

            accounts = (await _msal.GetAccountsAsync()).ToList();
        }
    }
    
    private (string? clientId, string? tenant) LoadAppConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",true, true);

        var configuration = builder.Build();
        var clientId = configuration.GetSection("App:ClientId").Value;
        var tenant = configuration.GetSection("App:Tenant").Value;
        return (clientId, tenant);
    }

    private string DumpAuthenticationResult(AuthenticationResult result)
    {
        var sb = new StringBuilder();

        sb.AppendFormat($"Username: {result.Account.Username}");
        sb.AppendLine();
        sb.AppendFormat($"Tenant: {result.Account.HomeAccountId.TenantId}");
        sb.AppendLine();
        sb.AppendFormat($"Provider: {result.Account.Environment}");
        sb.AppendLine();
        foreach (var claim in result.ClaimsPrincipal.Claims)
        {
            sb.AppendFormat($"Claim: {claim.Type} - {claim.Value}");
            sb.AppendLine();
        }

        sb.AppendFormat($"Scopes: {string.Join(", ", result.Scopes)}");
        sb.AppendLine();

        return sb.ToString();
    }
}