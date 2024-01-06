using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using EscapeFromTheWoods.BL.Records;
using EscapeFromTheWoods.BL.Interfaces;
using EscapeFromTheWoods.BL.Objects;

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

        public void WriteMonkeyRecords(List<DBMonkeyRecord> data)
        {
            var collection = database.GetCollection<DBMonkeyRecord>("monkeys");
            foreach (var record in data)
            {
                collection.InsertOne(record);
            }
        }

        public void WriteWoodRecord(DBWoodRecord record)
        {
            try
            {
                var collection = database.GetCollection<DBWoodRecord>("woods");
                collection.InsertOne(record);
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                Console.WriteLine($"Error writing record to database: {ex.Message}");
            }
        }


        public void WriteLogRecords(List<DBLogRecord> data)
        {
            var collection = database.GetCollection<DBLogRecord>("Logs");
            foreach (var record in data)
            {
                collection.InsertOne(record);
            }
        }

        public void WriteLogRecord(DBLogRecord logEntry)
        {
            try
            {
                var collection = database.GetCollection<DBLogRecord>("Logs");

                // Directly insert the single log record
                collection.InsertOne(logEntry);
            }
            catch (Exception ex)
            {
                // Handle the exception
                // This could be logging the error, rethrowing the exception, or other appropriate actions
                Console.WriteLine($"An error occurred while writing the log record: {ex.Message}");
                // Optionally rethrow to allow higher-level handling
                // throw;
            }
        }

    }
}
