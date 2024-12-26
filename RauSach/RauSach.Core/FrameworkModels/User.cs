using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;

namespace RauSach.Core.FrameworkModels
{
    [BsonIgnoreExtraElements]
    public class User : MongoUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public DateTime? Updated { get; set; }
        public bool IsLocked { get; set; }

        public List<Guid> Groups { get; set; } = new List<Guid>();
        public List<string> CustomRoles { get; set; } = new List<string>();

        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}
