using System;

namespace Meritocious.Common.DTOs.Auth
{
    public class TwoFactorSetupResult
    {
        public string SharedKey { get; set; }
        public string QrCodeUrl { get; set; }
    }
}