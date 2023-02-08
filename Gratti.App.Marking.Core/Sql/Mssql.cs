using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Gratti.App.Marking.Core.Sql
{
    public static class Mssql
    {
        public static void CreateCommand(string connectionString, string commandText, Action<SqlConnection, SqlCommand> action)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    if (connection.State != System.Data.ConnectionState.Open) connection.Open();
                    action?.Invoke(connection, command);
                }
            }
        }

        public static void AddParameters(this SqlCommand command, SqlParameter[] sqlParameters = null)
        {
            if (sqlParameters != null && sqlParameters.Length > 0)
                foreach (var p in sqlParameters)
                {
                    if (p.Value == null) p.Value = DBNull.Value;
                    Type t = p.Value.GetType();
                    if (t.IsArray && !(p.Value is byte[]))
                    {
                        if (p.Value is int[])
                            command.AddParameterArrayValues(p.ParameterName, (int[])p.Value);
                        else if (p.Value is string[])
                            command.AddParameterArrayValues(p.ParameterName, (string[])p.Value);
                        else if (p.Value is decimal[])
                            command.AddParameterArrayValues(p.ParameterName, (decimal[])p.Value);
                        else if (p.Value is DateTime[])
                            command.AddParameterArrayValues(p.ParameterName, (DateTime[])p.Value);
                        else
                            command.AddParameterArrayValues(p.ParameterName, (object[])p.Value);
                    }
                    else
                        command.Parameters.Add(p);
                }
        }

        public static void AddParameterArrayValues<T>(this SqlCommand command, string parameterName, T[] values)
        {
            List<string> parameterNames = new List<string>();

            for (int i = 0; values != null && i < values.Length; i++)
            {
                string paramName = parameterName + i;
                command.Parameters.AddWithValue(parameterName + i, values[i]);
                parameterNames.Add(paramName);
            }
            command.CommandText = command.CommandText.Replace(parameterName, string.Join(",", parameterNames));
        }

        public static void ExecuteNonQuery(string connectionString, string commandText, SqlParameter[] sqlParameters = null)
        {
            CreateCommand(connectionString, commandText,
                 (connection, command) =>
                 {
                     if (sqlParameters != null && sqlParameters.Length > 0)
                         command.AddParameters(sqlParameters);

                     command.ExecuteNonQuery();
                 }
            );
        }


        public static void ExecuteQuery(string connectionString, string commandText, SqlParameter[] sqlParameters, Action<SqlDataReader> onExecute, Action<object[]> action)
        {
            if (action == null)
                return;

            CreateCommand(connectionString, commandText,
                 (connection, command) =>
                 {
                     if (sqlParameters != null && sqlParameters.Length > 0)
                         command.AddParameters(sqlParameters);

                     using (SqlDataReader reader = command.ExecuteReader())
                     {
                         onExecute?.Invoke(reader);
                         object[] values = new object[reader.FieldCount];
                         while (reader.Read())
                         {
                             reader.GetValues(values);
                             action(values);
                         }
                     }
                 }
            );
        }
    }
}
