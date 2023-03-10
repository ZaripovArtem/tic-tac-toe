using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tick_tack_toe.Models;

[Serializable, BsonIgnoreExtraElements]
public class Game
{
    [BsonId, BsonElement("game_id"), BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; }

    [BsonElement("field")]
    //public IEnumerable<string> Field { get; set; }
    public string[] Field { get; set; }

    [BsonElement("move")]
    public int Move { get; set; }

    public string Status { get; set; }
}
