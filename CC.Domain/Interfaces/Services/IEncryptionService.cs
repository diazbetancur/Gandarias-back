using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Domain.Interfaces.Services;

public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText);

    Task<string> DecryptAsync(string encryptedText);
}