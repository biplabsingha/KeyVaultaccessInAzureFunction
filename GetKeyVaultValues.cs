using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AccessKeyVault
{
    public static class GetKeyVaultValues
    {
        [FunctionName("GetKeyVaultValues")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string linkKeyVaultUrl = $"https://keyVaultname.vault.azure.net/secrets/";
            string keyvaultKey = $"KeyVaultKey";
            var secretURL = linkKeyVaultUrl + keyvaultKey;

            //Get token from managed service principal
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            try
            {
                var clientIdRecord = await kvClient.GetSecretAsync(secretURL).ConfigureAwait(false);

                string KeyvaultValue = clientIdRecord.Value;

                return req.CreateErrorResponse(HttpStatusCode.OK, "Key vault secret value is  :  " + KeyvaultValue);
            }
            catch (System.Exception ex)
            {

               return req.CreateResponse(HttpStatusCode.BadRequest, "Key vault value request is not successfull");
            }
           
        }
    }
}
