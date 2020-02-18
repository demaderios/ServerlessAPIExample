using System;
using System.Collections.Generic;
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
using ServerlessMusic.API.Models;

namespace ServerlessMusic.API.Functions
{
    public class DeleteAlbum
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public DeleteAlbum(MongoClient mongoClient, ILogger<DeleteAlbum> logger, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("ServerlessMusic");
            _albums = database.GetCollection<Album>("albums");
        }

        [FunctionName(nameof(DeleteAlbum))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Album/{id}")]
            HttpRequest req, string id)
        {
            IActionResult returnValue = null;

            try
            {
                var albumToDelete = _albums.DeleteOne(album => album.AlbumId == id);

                if (albumToDelete == null)
                {
                    _logger.LogInformation($"Album with id: {id} does not exist.  Delete failed");
                    returnValue = new StatusCodeResult(StatusCodes.Status404NotFound);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not delete item.  Exception thrown: {e.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}
