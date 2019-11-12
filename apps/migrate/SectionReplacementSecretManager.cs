using System.Collections.Generic;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Amphora.Migrate
{
    public class SectionReplacementSecretManager : IKeyVaultSecretManager
    {
        private readonly string vault;
        private readonly string group;

        /// <summary>
        /// Replaces the name of a configuration section
        /// </summary>
        public SectionReplacementSecretManager(string vault, string group)
        {
            this.vault = vault;
            this.group = group;
        }

        public bool Load(SecretItem secret)
        {
            // load all secrets
            var res = secret.Identifier.Vault.Contains(vault.Trim('/'));
            return res;
        }

        public string GetKey(SecretBundle secret)
        {
            // Remove the prefix from the secret name and replace two 
            // dashes in any name with the KeyDelimiter, which is the 
            // delimiter used in configuration (usually a colon). Azure 
            // Key Vault doesn't allow a colon in secret names.
            var del = ConfigurationPath.KeyDelimiter;

            return $"{group}{del}{secret.SecretIdentifier.Name.Replace("--", del)}";

        }
    }
}