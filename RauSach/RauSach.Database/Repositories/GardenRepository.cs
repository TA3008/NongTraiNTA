﻿using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class GardenRepository : BaseRepository<Garden>, IGardenRepository
    {
        public GardenRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}