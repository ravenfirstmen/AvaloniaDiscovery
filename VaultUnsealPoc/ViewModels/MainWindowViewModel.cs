using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Dialogs.Internal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Identity.Client;
using Yubico.YubiKey;

namespace VaultUnsealPoc.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string tokenInfo;
    [ObservableProperty] private string yubikey;
    
    public MainWindowViewModel()
    {
        GetYubikey();
        SetupAuth();
    }
    
    private IPublicClientApplication? _clientApp = null;
    private void SetupAuth()
    {
        _clientApp = PublicClientApplicationBuilder.Create(Constants.AzureAd.ClientId)
            .WithAuthority($"{Constants.AzureAd.Instance}{Constants.AzureAd.Tenant}")
            .WithRedirectUri("http://localhost")
            .Build();
    }

    private void GetYubikey()
    {
        var key = YubiKeyDevice.FindAll().FirstOrDefault();
        Yubikey = key is null
            ? "No Yubikey found!"
            : $"Serial: {key.SerialNumber} | Firmware: {key.FirmwareVersion} | Form factor: {key.FormFactor}";
    }

    [RelayCommand]
    private async Task Login()
    {
        AuthenticationResult? authResult = null;
        IAccount? firstAccount = null;
        string[] scopes = { "user.read" };
        
        try
        {
            // var accounts = await _clientApp.GetAccountsAsync();
            // firstAccount = accounts.FirstOrDefault();
            // authResult = await _clientApp.AcquireTokenSilent(scopes, firstAccount)
            //     .ExecuteAsync();
            authResult = await _clientApp.AcquireTokenInteractive(scopes)
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            TokenInfo = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
            return;
        }

        if (authResult != null)
        {
            TokenInfo = DumpAuthenticationResult(authResult);
        }        
    }
    
    [RelayCommand]
    private async Task Logout()
    {
        var accounts = await _clientApp.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                await _clientApp.RemoveAsync(accounts.FirstOrDefault());
            }
            catch (MsalException ex)
            {
                TokenInfo = $"Error signing-out user: {ex.Message}";
                return;
            }
        }
        TokenInfo = String.Empty;
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
        sb.AppendFormat($"Scopes: {String.Join(", ", result.Scopes)}");
        sb.AppendLine();

        return sb.ToString();
    }
}