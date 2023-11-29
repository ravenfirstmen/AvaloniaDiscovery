using System.Collections.Generic;
using Microsoft.Identity.Client.Extensions.Msal;

namespace VaultUnsealPoc.ViewModels;

public static class Constants
{
    public static class AzureAd
    {
        public static string Instance => "https://login.microsoftonline.com/";
    }

    public static class Keyring
    {
        public const string CacheFileName = "msal_cache.txt";

        public const string KeyChainServiceName = "msal_service";
        public const string KeyChainAccountName = "msal_account";

        public const string LinuxKeyRingSchema = "app.tokencache";
        public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
        public const string LinuxKeyRingLabel = "MSAL token cache for the app.";
        public static readonly string CacheDir = MsalCacheHelper.UserRootDirectory;
        public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new("Version", "1");
        public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new("ProductGroup", "App");
    }
}