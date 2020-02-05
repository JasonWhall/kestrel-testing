using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;


namespace kestrel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, serverOptions) =>
                    {

                    var certificatePath = "certs/cert.pem";
                    var privateKeyPath = "certs/key.pem";
                    
                    var cert = LoadPemCertificate(certificatePath, privateKeyPath);

                    serverOptions.Listen(IPAddress.Any, 5001, listenOptions =>
                    {
                        listenOptions.UseHttps(cert);
                    });

                    serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
                    {
                    });

                    }).UseStartup<Startup>();
                });

        public static X509Certificate2 LoadPemCertificate(string certificatePath, string privateKeyPath)
        {
            using var publicKey = new X509Certificate2(certificatePath);

            var privateKeyText = File.ReadAllText(privateKeyPath);
            var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            using var rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }

            var keyPair = publicKey.CopyWithPrivateKey(rsa);
            return new X509Certificate2(keyPair.Export(X509ContentType.Pfx));
        }
    }
}
