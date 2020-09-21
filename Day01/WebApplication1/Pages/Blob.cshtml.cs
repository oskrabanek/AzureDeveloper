using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class BlobModel : PageModel
    {
        public string Containers;

        public async Task OnGet()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            var containers = blobServiceClient.GetBlobContainers();

            foreach (var item in containers)
            {
                Containers += item.Name + "\n";
            }

            Containers.TrimEnd();
        }
    }
}