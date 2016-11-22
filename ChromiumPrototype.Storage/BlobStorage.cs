using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ChromiumPrototype.Storage
{
    public class BlobStorage
    {
        private readonly CloudStorageAccount storageAccount;
        private CloudBlobContainer container;
        private string containerName;

        public BlobStorage(string containerName)
        {
            this.containerName = containerName;

            storageAccount =
                CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=chromiumprototype;AccountKey=Ldxnd00sM6g1umV8xgO/pCnE0YB6a0wJk8l+AeBE/ZNy/C5OuZ095aTcgmx84AG7q5px1WYgEUC+OgiG5Liwog==");

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();

        }
        
        public void CreateOrUpdate(string fileName, string conteudo)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadText(conteudo);
        }

        public void CreateOrUpdate(string fileName, byte[] bytes)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromByteArray(bytes, 0, bytes.Length);
        }
        
        public string Read(string caminho)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(caminho);

            var conteudo = blockBlob.DownloadText();

            return conteudo;
        }
    }
}
