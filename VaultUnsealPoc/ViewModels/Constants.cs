namespace VaultUnsealPoc.ViewModels;

public static class Constants
{
    public static class AzureAd
    {
        // Below are the clientId (Application Id) of your app registration and the tenant information. 
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
        //   - For Work or School account in your org, use your tenant ID, or domain
        //   - for any Work or School accounts, use organizations
        //   - for any Work or School accounts, or Microsoft personal account, use cdf76b6c-55fb-4bd4-960c-c68c4f6345e4
        //   - for Microsoft Personal account, use consumers
        public static string ClientId => "d5138855-e1eb-497e-ae3b-4bc2ac02e0c3";

        // Note: Tenant is important for the quickstart.
        public static string Tenant => "cdf76b6c-55fb-4bd4-960c-c68c4f6345e4";
        public static string Instance => "https://login.microsoftonline.com/";
        public static string RedirectUri => "https://login.microsoftonline.com/common/oauth2/nativeclient";
    }

}