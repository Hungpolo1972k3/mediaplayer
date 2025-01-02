using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaPlayerWinUI
{
    public class File
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("artist")]
        public string Artist { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("album_id")]
        public string AlbumId { get; set; }

        [BsonElement("playlist_id")]
        public string PlaylistId { get; set; }

        [BsonElement("thumbnail")]
        public string Thumbnail { get; set; }

        [BsonElement("runtime")]
        [BsonRepresentation(BsonType.String)]
        public TimeSpan Runtime { get; set; }

        [BsonElement("genre")]
        public string Genre { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
