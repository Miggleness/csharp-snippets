using System;
using System.Data.Common;
using System.Data.SqlClient;
using Xunit;
using System.Collections;
using System.Collections.Generic;
using Shouldly;
using CodeSnippets.Database;

namespace CodeSnippets.Tests.Database
{
    public class LinearBlockIdGeneratorForMssqlTests : IDisposable
    {
        public static string ConnectionString => Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? @"Data Source=.;Initial Catalog=tempdb;User Id=sa;Password=Asdfasdf1";
        const string TableName = "LinearBlockTestTable";

        static Func<DbConnection> _dbConnectionFactory = () => new SqlConnection(ConnectionString);
        
        public LinearBlockIdGeneratorForMssqlTests()
        {
            Cleanup();
            Initialize();
        }

        [Fact]
        public void Should_create_if_new_dimension_and_increment_range_if_exceeded()
        {
            var generator = new LinearBlockIdGenerator(_dbConnectionFactory, range: 15, tableName: TableName);
            var resultStore = new List<long>();
            for (int ctr = 0; ctr < 20; ctr++)
            {
                resultStore.Add(generator.GetNextId("a"));
            }

            resultStore.Count.ShouldBe(20);
            GetDimensionNextValInDb("a").ShouldBe(31);
        }


        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            using (var c = _dbConnectionFactory())
            using (var cmd = c.CreateCommand())    
            {
                c.Open();
                cmd.CommandText = $"IF OBJECT_ID('dbo.{TableName}', 'U') IS NOT NULL DROP TABLE dbo.{TableName};";
                cmd.ExecuteNonQuery();
            }
        }

        private void Initialize()
        {
            using (var c = _dbConnectionFactory())
            using (var cmd = c.CreateCommand())
            {
                c.Open();
                cmd.CommandText = $"CREATE TABLE {TableName} (" +
                    "              dimension VARCHAR(20) NOT NULL," +
                    "              nextval int NOT NULL);";
                cmd.ExecuteNonQuery();
            }
        }

        private int GetDimensionNextValInDb(string dimension)
        {
            using (var c = _dbConnectionFactory())
            using (var cmd = c.CreateCommand())
            {
                c.Open();
                cmd.CommandText = $"SELECT nextval FROM dbo.{TableName} WHERE dimension=@dimension";
                var param = cmd.CreateParameter();
                param.Value = dimension;
                param.ParameterName = "@dimension";
                cmd.Parameters.Add(param);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
