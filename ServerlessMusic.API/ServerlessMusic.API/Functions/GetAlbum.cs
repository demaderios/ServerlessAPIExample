using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ServerlessMusic.API.Models;

namespace ServerlessMusic.API.Functions
{
    public class GetAlbum
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public GetAlbum(MongoClient mongoClient, ILogger<GetAlbum> logger, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;
            
            var database = _mongoClient.GetDatabase("ServerlessMusic");
            _albums = database.GetCollection<Album>("albums");
        }

        [FunctionName(nameof(GetAlbum))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Album/{id}")]
            HttpRequest req, string id)
        {
            IActionResult returnValue = null;

            try
            {
                var result = _albums.Find(album => album.AlbumId == id).FirstOrDefault();

                if (result == null)
                {
                    _logger.LogWarning("That item does not exist!");
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't find Album with id: {id}. Exception thrown: {e.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}
