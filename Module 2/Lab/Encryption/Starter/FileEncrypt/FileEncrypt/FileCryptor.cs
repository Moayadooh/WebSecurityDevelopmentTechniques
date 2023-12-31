using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileEncrypt
{
    public class FileCryptor
    {
        private ISaltGenerator saltGen;
        private IKeyGenerator keyGen;
        private ISymmetricAlgorithmFactory aesFactory;

        public FileCryptor(ISaltGenerator saltGen, IKeyGenerator keyGen, ISymmetricAlgorithmFactory aesFactory)
        {
            this.saltGen = saltGen ?? throw new ArgumentNullException(nameof(saltGen));
            this.keyGen = keyGen ?? throw new ArgumentNullException(nameof(keyGen));
            this.aesFactory = aesFactory ?? throw new ArgumentNullException(nameof(keyGen));
        }

        public int EncryptFiles(EncryptOptions options)
        {
            if (options.Valid())
            {
                var salt = saltGen.GenerateSalt(options.KeyLength);
                byte[] key = keyGen.GenerateKeyFromPassword(options.Password, options.KeyLength, salt, options.Iterations);

                SymmetricAlgorithm aes = aesFactory.Create(key, options.KeyLength);
                ICryptoTransform enc = aes.CreateEncryptor();

                // Open options.OutputFile and write the salt, initialization vector and encrypted file
                using (Stream output = File.OpenWrite(options.OutputFile))
                {
                    output.Write(salt, offset: 0, count: options.KeyLength);
                    output.Write(aes.IV, offset: 0, count: aes.IV.Length);

                    using (var cs = new CryptoStream(output, enc, CryptoStreamMode.Write))
                    {
                        Stream s = File.OpenRead(options.InputFile);
                        byte[] buf = new byte[2048];
                        int read = 0;
                        while ((read = s.Read(buf, 0, buf.Length)) > 0)
                        {
                            cs.Write(buf, offset: 0, count: read);
                        }
                    }
                }
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int DecryptFiles(DecryptOptions options)
        {
            if (options.Valid())
            {
                Stream input = File.OpenRead(options.InputFile);
                byte[] salt = new byte[options.KeyLength];
                input.Read(salt, offset: 0, count: options.KeyLength);

                var keyGen = new KeyGenerator();
                byte[] key = keyGen.GenerateKeyFromPassword(options.Password,
                               options.KeyLength, salt, options.Iterations);

                SymmetricAlgorithm aes = aesFactory.Create(key, options.KeyLength);

                byte[] IV = new byte[aes.IV.Length];
                input.Read(IV, 0, IV.Length);
                aes.IV = IV;

                ICryptoTransform enc = aes.CreateDecryptor();
                using (var cs = new CryptoStream(input, enc, CryptoStreamMode.Read))
                using (var output = File.OpenWrite(options.OutputFile))
                {
                    byte[] buf = new byte[2048];
                    int read = 0;
                    while ((read = cs.Read(buf, 0, buf.Length)) > 0)
                    {
                        output.Write(buf, offset: 0, count: read);
                    }
                }
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
