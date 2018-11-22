using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace CodeSnippets.Database
{
    /// <summary>
    /// This snippet is largely based off the implementation in https://github.com/sebastienros/yessql
    /// Literature at http://literatejava.com/hibernate/linear-block-allocator-a-superior-alternative-to-hilo/
    /// </summary>
    public class LinearBlockIdGenerator
    {
        public readonly int MaxRetries = 20;
        private readonly int _range;
        private readonly ConcurrentDictionary<string, object> _lockChest = new ConcurrentDictionary<string, object>();
        private readonly Func<DbConnection> _dbConnectionFactory;
        private bool _initialized;
        private long _start;
        private int _increment;
        private long _end;
        private string _selectSql;
        private string _updateSql;
        private string _insertSql;


        public LinearBlockIdGenerator(Func<DbConnection> dbConnectionFactory, int range = 20, string schema = "dbo", string tableName = "Identifiers")
        {
            _dbConnectionFactory = dbConnectionFactory
                ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _range = range;

            // set SQL commands. There must be a better way to apply RDBMS specific quotes without using preprocessors
#if MSSQL
            _selectSql = string.Format("SELECT [nextval] FROM [{0}].[{1}] WHERE dimension = @dimension;", schema, tableName);
            _updateSql = string.Format("UPDATE [{0}].[{1}] SET [nextval] = @new WHERE [nextval] = @previous AND [dimension] = @dimension;", schema, tableName);
            _insertSql = string.Format("INSERT INTO [{0}].[{1}] ([dimension], [nextval]) VALUES(@dimension, @nextval);", schema, tableName);
#elif MYSQL
            _selectSql = string.Format("SELECT `nextval` FROM `{0}`.`{1}` WHERE dimension = @dimension;", schema, tableName);
            _updateSql = string.Format("UPDATE `{0}`.`{1}` SET `nextval` = @new WHERE `nextval` = @previous AND `dimension` = @dimension;", schema, tableName);
            _insertSql = string.Format("INSERT INTO `{0}`.`{1}` (`dimension`, `nextval`) VALUES(@dimension, @nextval);", schema, tableName);
#elif PG
            _selectSql = string.Format("SELECT \"nextval\" FROM \"{0}\".\"{1}\" WHERE dimension = @dimension;", schema, tableName);
            _updateSql = string.Format("UPDATE \"{0}\".\"{1}\" SET \"nextval\" = @new WHERE \"nextval\" = @previous AND \"dimension\" = @dimension;", schema, tableName);
            _insertSql = string.Format("INSERT INTO \"{0}\".\"{1}\" (\"dimension\", \"nextval\") VALUES(@dimension, @nextval);", schema, tableName);
#else
            _selectSql = string.Format("SELECT nextval FROM {0}.{1} WHERE dimension = @dimension;", schema, tableName);
            _updateSql = string.Format("UPDATE {0}.{1} SET nextval = @new WHERE nextval = @previous AND dimension = @dimension;", schema, tableName);
            _insertSql = string.Format("INSERT INTO {0}.{1} (dimension, nextval) VALUES(@dimension, @nextval);", schema, tableName);
                                       
#endif
        }

        public long GetNextId(string dimension)
        {
            // Initialize the range
            if (_end == 0)
            {
                using (var dbConnection = _dbConnectionFactory())
                {
                    dbConnection.Open();
                    EnsureInitialized(dbConnection, dimension);
                    LeaseRange(dbConnection, dimension);
                }
            }

            var newIncrement = Interlocked.Increment(ref _increment);
            var nextId = newIncrement + _start;

            if (nextId > _end)
            {
                using (var dbConnection = _dbConnectionFactory())
                {
                    dbConnection.Open();
                    LeaseRange(dbConnection, dimension);
                    return GetNextId(dimension);
                }
            }

            return nextId;
        }

        private void LeaseRange(DbConnection dbConnection, string dimension)
        {
            lock (_lockChest.GetOrAdd(dimension, new object()))
            {
                var affectedRows = 0;
                long nextval;
                int retries = 0;

                using (var transaction = dbConnection.BeginTransaction())
                {
                    do
                    {
                        // Ensure we overwrite the value that has been read by this
                        // instance in case another client is trying to lease a range
                        // at the same time

                        using (var selectCommand = transaction.Connection.CreateCommand())
                        {
                            selectCommand.CommandText = _selectSql;

                            var selectDimension = selectCommand.CreateParameter();
                            selectDimension.Value = dimension;
                            selectDimension.ParameterName = "@dimension";
                            selectCommand.Parameters.Add(selectDimension);

                            selectCommand.Transaction = transaction;

                            nextval = Convert.ToInt64(selectCommand.ExecuteScalar());
                        }

                        using (var updateCommand = transaction.Connection.CreateCommand())
                        {
                            updateCommand.CommandText = _updateSql;

                            var updateDimension = updateCommand.CreateParameter();
                            updateDimension.Value = dimension;
                            updateDimension.ParameterName = "@dimension";
                            updateCommand.Parameters.Add(updateDimension);

                            var newValue = updateCommand.CreateParameter();
                            newValue.Value = nextval + _range;
                            newValue.ParameterName = "@new";
                            updateCommand.Parameters.Add(newValue);

                            var previousValue = updateCommand.CreateParameter();
                            previousValue.Value = nextval;
                            previousValue.ParameterName = "@previous";
                            updateCommand.Parameters.Add(previousValue);

                            updateCommand.Transaction = transaction;

                            affectedRows = updateCommand.ExecuteNonQuery();
                        }

                        if (retries++ > MaxRetries)
                        {
                            throw new Exception("Too many retries while trying to lease a range for: " + dimension);
                        }
                    } while (affectedRows == 0);

                    // commit transaction
                    transaction.Commit();

                }//-- end using(var transaction...)

                _increment = -1; // Start with -1 as it will be incremented
                _start = nextval;
                _end = nextval + _range - 1;
            }
        }

        private void EnsureInitialized(DbConnection connection, string dimension)
        {
            if (_initialized)
            {
                return;
            }

            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                // Does the record already exist?
                using (var selectCommand = transaction.Connection.CreateCommand())
                {
                    selectCommand.CommandText = _selectSql;

                    var selectDimension = selectCommand.CreateParameter();
                    selectDimension.Value = dimension;
                    selectDimension.ParameterName = "@dimension";
                    selectCommand.Parameters.Add(selectDimension);

                    selectCommand.Transaction = transaction;

                    var nextVal = selectCommand.ExecuteScalar();

                    if (nextVal != null)
                    {
                        return;
                    }
                }

                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = _insertSql;

                    var dimensionParameter = command.CreateParameter();
                    dimensionParameter.Value = dimension;
                    dimensionParameter.ParameterName = "@dimension";
                    command.Parameters.Add(dimensionParameter);

                    var nextValParameter = command.CreateParameter();
                    nextValParameter.Value = 1;
                    nextValParameter.ParameterName = "@nextval";
                    command.Parameters.Add(nextValParameter);
                    var x = command.Transaction == transaction;
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();
                }

                // commit transaction
                transaction.Commit();
            }

            _initialized = true;
        }
    }
}