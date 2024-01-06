using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using EscapeFromTheWoods.BL.Records;
using EscapeFromTheWoods.BL.Interfaces;
using EscapeFromTheWoods.BL.Objects;
using EscapeFromTheWoods.DL.Mappers;
using SharpCompress.Writers;

namespace EscapeFromTheWoods.MongoDB
{
    public class MongoDBWriter : IMongoDBWriter
    {
        private IMongoDatabase database;

        public MongoDBWriter(string connectionString)
        {
            var client = new MongoClient(connectionString);
            this.database = client.GetDatabase("EscapeFromTheWoodsDB");
        }

        public async Task WriteWoodToDBAsync(Tree tree, int woodId)
        {
            try
            {
                var record = new DBWoodRecord(woodId, tree.treeID, tree.x, tree.y);
                var collection = database.GetCollection<DBWoodRecord>("woods");
                await collection.InsertOneAsync(record);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing record to database: {ex.Message}");
            }
        }

        public async Task WriteMonkeyRecordAsync(Monkey monkey, int woodId, int jumpNumber, List<Tree> route)
        {
            var collection = database.GetCollection<DBMonkeyRecord>("monkeys");

            try
            {
                await collection.InsertOneAsync(MonkeyRecordMapper.MapToDBMonkeyRecord(monkey, woodId, jumpNumber, route));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing record to database: {ex.Message}");
            }
        }

        public async Task WriteLogRecordAsync(LogEntry logEntry)
        {
            try
            {
                var collection = database.GetCollection<DBLogRecord>("Logs");
                var dbLogRecord = LogRecordMapper.MapToDBLogRecord(logEntry);
                await collection.InsertOneAsync(dbLogRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing the log record asynchronously: {ex.Message}");
            }
        }
    }
}
