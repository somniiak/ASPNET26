using Microsoft.AspNetCore.DataProtection;

namespace WebApplication3
{
    public class ByteRevDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose) =>
            new ByteRevDataProtector();

        public byte[] Protect(byte[] plaintext) =>
            plaintext.Reverse().ToArray();

        public byte[] Unprotect(byte[] protectedData) =>
            Protect(protectedData);
    }

    public class ByteRevDataProtectionProvider : IDataProtectionProvider
    {
        IDataProtector IDataProtectionProvider.CreateProtector(string purpose) =>
            new ByteRevDataProtector();
    }
}
