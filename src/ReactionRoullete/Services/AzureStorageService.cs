using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace ReactionRoullete.Services
{
    public class AzureStorageService : IDisposable
    {
        const string STORAGEACCOUNTNAME = "reactionroulette";
        const string STORAGEACCOUNTKEY = "/BN1UtylszcojZDrejjMdok7unBvehQSg0h+U/YP/tVzIi+OtA3wvEbgvZLAcTnhV8x5r/Py3L1LVawnqIHnbg==";

        private CloudBlobClient _Client;
        private CloudBlobContainer _PreflightContainer;

        public AzureStorageService()
        {
            _Client = new CloudBlobClient(new Uri("https://reactionroulette.blob.core.windows.net/"), new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(STORAGEACCOUNTNAME, STORAGEACCOUNTKEY));
            _PreflightContainer = _Client.GetContainerReference("preflight");
        }

        private async Task CreateContainerIfNotExist()
        {
            bool created = await _PreflightContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());


        }

        public async Task<Uri> PutPreflightFileAsync(string localfile)
        {
            await CreateContainerIfNotExist();

            var filename = System.IO.Path.GetFileName(localfile);

            var blob = _PreflightContainer.GetBlockBlobReference(filename);

            await blob.UploadFromFileAsync(localfile, System.IO.FileMode.Open);

            return blob.Uri;
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AzureStorageService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
