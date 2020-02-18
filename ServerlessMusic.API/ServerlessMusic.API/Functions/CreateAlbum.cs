using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using ServerlessMusic.API.Models;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace ServerlessMusic.API.Functions
{
    public class CreateAlbum
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Album> _albums;

        public CreateAlbum(MongoClient mongoClient, ILogger<CreateAlbum> logger, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("ServerlessMusic");
            _albums = database.GetCollection<Album>("albums");
        }

        [FunctionName(nameof(CreateAlbum))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Album")]
            HttpRequest req)
        {
            IActionResult returnValue = null;

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Album>(requestBody);

            var album = new Album
            {
                AlbumName = input.AlbumName,
                Artist = input.Artist,
                Price = input.Price,
                ReleaseDate = input.ReleaseDate,
                Genre = input.Genre
            };

            try
            {
                _albums.InsertOne(album);
                returnValue = new OkObjectResult(album);
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
