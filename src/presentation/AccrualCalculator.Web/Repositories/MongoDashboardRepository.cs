using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppName.Web.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AppName.Web.Repositories
{
    public class MongoDashboardRepository : IDashboardRepository
    {
        private readonly IMongoDatabase _database;

        public IMongoCollection<Accrual> Accruals => _database.GetCollection<Accrual>("accruals");

        public MongoDashboardRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<bool> AddActionForAccrualAsync(string userId, Guid accrualId, AccrualActionRecord action)
        {
            var filter = Builders<Accrual>.Filter.And(
                Builders<Accrual>.Filter.Eq(e => e.AccrualId, accrualId),
                Builders<Accrual>.Filter.Eq(e => e.UserId, userId)
            );
            var update = Builders<Accrual>.Update.Push(e => e.Actions, action);

            await Accruals.FindOneAndUpdateAsync(filter, update);

            return true;
        }

        public async Task<bool> DeleteActionAsync(string userId, Guid accrualId, Guid accrualActionId)
        {
            string id = accrualActionId.ToString();
            var filter = Builders<Accrual>.Filter.And(
                Builders<Accrual>.Filter.Eq(e => e.AccrualId, accrualId),
                Builders<Accrual>.Filter.Eq(e => e.UserId, userId)
            );
            var update = Builders<Accrual>.Update.PullFilter(e => e.Actions, a => a.AccrualActionId == id);

            await Accruals.FindOneAndUpdateAsync(filter, update);

            return true;
        }

        public async Task<List<Accrual>> GetAllAccrualsForUser(string userId)
        {
            var result = await (
                from a in Accruals.AsQueryable()
                where a.UserId == userId
                select a
            ).ToListAsync();

            return result;
        }

        public async Task<Accrual> GetAccrualForUserByAccrualIdAsync(string userId, Guid accrualId)
        {
            var result = await (
                from a in Accruals.AsQueryable()
                where a.UserId == userId && a.AccrualId == accrualId
                select a
            ).SingleOrDefaultAsync();

            return result;
        }

        public async Task<bool> AddAccrual(Accrual accrual)
        {
            await Accruals.InsertOneAsync(accrual);

            return true;
        }
        
        public async Task<bool> ModifyAccrualAsync(Accrual accrual)
        {
            var filter = Builders<Accrual>.Filter.And(
                Builders<Accrual>.Filter.Eq(e => e.AccrualId, accrual.AccrualId),
                Builders<Accrual>.Filter.Eq(e => e.UserId, accrual.UserId)
            );

            await Accruals.ReplaceOneAsync(filter, accrual);

            return true;
        }
    }
}