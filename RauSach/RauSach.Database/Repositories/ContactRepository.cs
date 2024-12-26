using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
