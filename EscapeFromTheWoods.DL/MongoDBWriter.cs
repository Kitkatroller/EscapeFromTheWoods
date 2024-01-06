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

        public void WriteMonkeyRecords(List<DBMonkeyRecord> data)
        {
            var collection = database.GetCollection<DBMonkeyRecord>("monkeys");
            foreach (var record in data)
            {
                collection.InsertOne(record);
            }
        }

        public void WriteLogRecord(LogEntry logEntry)
        {
            try
            {
                var collection = database.GetCollection<DBLogRecord>("Logs");
                collection.InsertOne(LogRecordMapper.MapToDBLogRecord(logEntry));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing the log record: {ex.Message}");
            }
        }
    }
}
