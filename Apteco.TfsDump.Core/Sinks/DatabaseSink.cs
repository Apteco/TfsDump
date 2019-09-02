using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Apteco.TfsDump.Core.Sinks
{
  public class DatabaseSink : ISink
  {
    private string connectionString;
    private bool initialised;
    private string[] fieldNames;
    private int keyFieldNameIndex;
    private string tableName;

    public DatabaseSink(string connectionString, string tableName)
    {
      this.connectionString = connectionString;
      this.tableName = tableName;
    }

    public Task InitialiseSink(string[] fieldNames, string keyFieldName)
    {
      if (fieldNames == null)
        throw new ArgumentNullException(nameof(fieldNames));

      if (initialised)
        throw new Exception("This sink has already been initialised");

      if (!fieldNames.Contains(keyFieldName))
        throw new ArgumentException($"The keyFieldName ({keyFieldName}) isn't listed as one of the fields");

      this.fieldNames = fieldNames;
      keyFieldNameIndex = Array.IndexOf(fieldNames, keyFieldName);
      initialised = true;

      return Task.CompletedTask;
    }

    public Task Write(string[] data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof(data));

      if (data.Length != fieldNames.Length)
        throw new Exception($"Can't write data with {data.Length} fields when the sink was initialised with {fieldNames.Length} fields");

      using (IDbConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        using (IDbCommand command = connection.CreateCommand())
        {
          IDbDataParameter parameter = command.CreateParameter();
          parameter.ParameterName = "@KEY";
          parameter.DbType = DbType.String;
          parameter.Value = data[keyFieldNameIndex];
          command.Parameters.Add(parameter);

          command.CommandText =
            $"DELETE FROM [{tableName}]" + Environment.NewLine +
            $"WHERE [{fieldNames[keyFieldNameIndex]}] = @KEY";

          command.ExecuteNonQuery();
        }

        using (IDbCommand command = connection.CreateCommand())
        {
          command.CommandText =
            $"INSERT INTO [{tableName}] ({String.Join(", ", fieldNames.Select(f => $"[{f}]"))})" + Environment.NewLine +
            "VALUES (";

          for (int i=0; i < data.Length; i++)
          {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = $"@FIELD{i}";
            parameter.DbType = DbType.String;
            parameter.Value = data[i] ?? (object)DBNull.Value;
            command.Parameters.Add(parameter);

            if (i > 0)
              command.CommandText += ", ";

            command.CommandText += parameter.ParameterName;
          }

          command.CommandText += ")";
          command.ExecuteNonQuery();
        }
      }

      return Task.CompletedTask;
    }
  }
}
