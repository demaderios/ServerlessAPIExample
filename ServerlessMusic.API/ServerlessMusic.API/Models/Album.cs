using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerlessMusic.API.Models
{
    public class Album
    { 
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlbumId { get; set; }

        [BsonElement("Name")]
        public string AlbumName { get; set; }
        public string Artist { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
    }
}
