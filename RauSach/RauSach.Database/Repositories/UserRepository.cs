using MongoDB.Bson;
using MongoDB.Driver;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Repositories;

namespace RauSach.Database.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase db) : base(db)
        {
        }

// Method to asynchronously find users based on the provided filter
        public Task<List<User>> FindAsync(FilterModel filter)
        {
            // Get the filter builder for User
            var builder = Builders<User>.Filter;

            // Initialize the filter builder with regex conditions for Name, Email, UserName, and PhoneNumber
            var filterBuilder = builder.Regex(m => m.Name, new BsonRegularExpression($"{filter.query}", "i")) |
                builder.Regex(m => m.Email, new BsonRegularExpression($"{filter.query}", "i")) |
                builder.Regex(m => m.UserName, new BsonRegularExpression($"{filter.query}", "i")) |
                builder.Regex(m => m.PhoneNumber, new BsonRegularExpression($"{filter.query}", "i"));

            // Check if custom filter is not empty
            if (!string.IsNullOrWhiteSpace(filter.custom))
            {
                // Apply additional filter conditions based on the custom filter value
                if (filter.custom == "Nhân viên")
                {
                    filterBuilder &= builder.SizeGt(m => m.CustomRoles, 0);
                }
                else
                    filterBuilder &= builder.Size(m => m.CustomRoles, 0);
            }

            // Return the result of the find operation with applied filter and limit
            return _collection.Find(filterBuilder).Limit(filter.limit).ToListAsync();
        }

        public Task<User> GetByIdAsync(string id)
        {
            return _collection.Find(Builders<User>.Filter.Eq(m => m.Id, ObjectId.Parse(id))).FirstOrDefaultAsync();
        }

        public User GetByUsername(string username) => _collection.Find(Builders<User>.Filter.Eq(m => m.UserName, username)).FirstOrDefault();

        public override async Task<User> UpdateAsync(User model)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == model.Id);
            var options = new FindOneAndReplaceOptions<User, User>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var updatedEntity = await _collection.FindOneAndReplaceAsync(filter, model, options);
            return model;
        }
    }
}
