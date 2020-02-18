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
    public class GetAllAlbums
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public GetAllAlbums(MongoClient mongoClient, ILogger<GetAlbum> logger, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("ServerlessMusic");
            _albums = database.GetCollection<Album>("albums");
        }

        [FunctionName(nameof(GetAllAlbums))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Albums")]
            HttpRequest req)
        {
            IActionResult returnValue = null;

            try
            {
                var result = _albums.Find(album => true).ToList();

                if (result == null)
                {
                    _logger.LogInformation($"There are no albums in the collection");
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception thrown: {e.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}
