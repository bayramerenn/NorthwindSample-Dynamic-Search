namespace NorthwindSample.Encrypt
{
    public interface IEncryption
    {
        string EncryptText(string text, string privateKey = "");
        string DecryptText(string text, string privateKey = "");
    }
}
