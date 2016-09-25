using System;
using Xamarin.Auth;

namespace PlayList.Android.VkService
{
    public class NeedAuthorizeException : Exception
    {
        public NeedAuthorizeException(OAuth2Authenticator oauth)
            : base("����� �����")
        {
            Oauth = oauth;
        }

        public OAuth2Authenticator Oauth { set; get; }
    }
}