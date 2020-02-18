using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using ServerlessMusic.API.Models;

namespace ServerlessMusic.API.Functions
{
    public class UpdateAlbum
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public UpdateAlbum(MongoClient mongoClient, ILogger<UpdateAlbum> logger, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("ServerlessMusic");
            _albums = database.GetCollection<Album>("albums");
        }

        [FunctionName(nameof(UpdateAlbum))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Album/{id}")]
            HttpRequest req, string id)
        {
            IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            var updatedResult = JsonConvert.DeserializeObject<Album>(requestBody);

            updatedResult.AlbumId = id;

            try
            {
                var replacedItem = _albums.ReplaceOne(album => album.AlbumId == id, updatedResult);

                if (replacedItem == null)
                {
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(updatedResult);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not update Album with id: {id}.  Exception thrown: {e.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}
