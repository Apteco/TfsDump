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
    private int[] keyFieldNameIndices;
    private string tableName;
    private string collectionUrl;

    public DatabaseSink(string connectionString, string tableName, string collectionUrl)
    {
      this.connectionString = connectionString;
      this.tableName = tableName;
      this.collectionUrl = collectionUrl;
    }

    public Task InitialiseSink(string[] fieldNames, string[] keyFieldNames)
    {
      if (fieldNames == null)
        throw new ArgumentNullException(nameof(fieldNames));

      if (keyFieldNames == null)
        throw new ArgumentNullException(nameof(keyFieldNames));

      if (keyFieldNames.Length == 0)
        throw new ArgumentException("keyFieldNames can't be empty", nameof(keyFieldNames));

      if (initialised)
        throw new Exception("This sink has already been initialised");

      keyFieldNameIndices = new int[keyFieldNames.Length];
      for (int i=0; i<keyFieldNames.Length; i++)
      {
        if (!fieldNames.Contains(keyFieldNames[i]))
          throw new ArgumentException($"The keyFieldName ({keyFieldNames[i]}) isn't listed as one of the fields");

        keyFieldNameIndices[i] = Array.IndexOf(fieldNames, keyFieldNames[i]);
      }

      this.fieldNames = fieldNames;
      this.collectionUrl = collectionUrl;
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
          command.CommandText =
            $"DELETE FROM [{tableName}]" + Environment.NewLine +
            $"WHERE CollectionUrl = {AddStringParameter(command, "COLLECTIONURL", collectionUrl)}" + Environment.NewLine;

          for (int i = 0; i<keyFieldNameIndices.Length; i++)
          {
            command.CommandText += $"AND [{fieldNames[keyFieldNameIndices[i]]}] = {AddStringParameter(command, $"KEY{i}", data[keyFieldNameIndices[i]])}" + Environment.NewLine;
          }

          command.ExecuteNonQuery();
        }

        using (IDbCommand command = connection.CreateCommand())
        {
          command.CommandText =
            $"INSERT INTO [{tableName}] ({String.Join(", ", fieldNames.Select(f => $"[{f}]"))}, CollectionUrl)" + Environment.NewLine +
            "VALUES (";

          for (int i=0; i < data.Length; i++)
          {
            if (i > 0)
              command.CommandText += ", ";

            command.CommandText += AddStringParameter(command, $"FIELD{i}", data[i]);
          }

          command.CommandText += $", {AddStringParameter(command, "COLLECTIONURL", collectionUrl)})";
          command.ExecuteNonQuery();
        }
      }

      return Task.CompletedTask;
    }

    private string AddStringParameter(IDbCommand command, string fieldName, string value)
    {
      IDbDataParameter parameter = command.CreateParameter();
      parameter.ParameterName = $"@{fieldName}";
      parameter.DbType = DbType.String;
      parameter.Value = value ?? (object)DBNull.Value;
      command.Parameters.Add(parameter);
      return parameter.ParameterName;
    }
  }
}
