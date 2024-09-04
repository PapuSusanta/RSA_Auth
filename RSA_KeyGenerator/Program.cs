using System.Security.Cryptography;
using System.Text;

var keyDirectory = "./RSAKeys";
if (!Directory.Exists(keyDirectory))
{
    Directory.CreateDirectory(keyDirectory);
}

var rsa = RSA.Create();
var privateKeyXml = rsa.ToXmlString(true);
var publicKeyXml = rsa.ToXmlString(false);

using var privatefile = File.Create(Path.Combine(keyDirectory, "PrivateKey.xml"));
using var publicfile = File.Create(Path.Combine(keyDirectory, "PublicKey.xml"));

privatefile.Write(Encoding.UTF8.GetBytes(privateKeyXml));
publicfile.Write(Encoding.UTF8.GetBytes(publicKeyXml));

