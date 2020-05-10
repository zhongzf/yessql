using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace YesSql.Provider.Cosmos
{
    public class CosmosDialect : BaseDialect
    {
        private static Dictionary<DbType, string> ColumnTypes = new Dictionary<DbType, string>
        {
            {DbType.Guid, "UNIQUEIDENTIFIER"},
            {DbType.Binary, "VARBINARY(8000)"},
            {DbType.Time, "DATETIME"},
            {DbType.Date, "DATETIME"},
            {DbType.DateTime, "DATETIME" },
            {DbType.DateTime2, "DATETIME2" },
            {DbType.DateTimeOffset, "datetimeoffset" },
            {DbType.Boolean, "BIT"},
            {DbType.Byte, "TINYINT"},
            {DbType.Currency, "MONEY"},
            {DbType.Decimal, "DECIMAL(19,5)"},
            {DbType.Double, "FLOAT(53)"},
            {DbType.Int16, "SMALLINT"},
            {DbType.UInt16, "SMALLINT"},
            {DbType.Int32, "INT"},
            {DbType.UInt32, "BIGINT"},
            {DbType.Int64, "BIGINT"},
            {DbType.UInt64, "NUMERIC(20)"},
            {DbType.Single, "REAL"},
            {DbType.AnsiStringFixedLength, "CHAR(255)"},
            {DbType.AnsiString, "VARCHAR(255)"},
            {DbType.StringFixedLength, "NCHAR(255)"},
            {DbType.String, "NVARCHAR(255)"},
        };

        public CosmosDialect()
        {
        }

        public override string Name => "Cosmos";
        public override string IdentitySelectString => "; select SCOPE_IDENTITY()";

        public override string GetTypeName(DbType dbType, int? length, byte precision, byte scale)
        {
            if (length.HasValue)
            {
                if (length.Value > 4000)
                {
                    if (dbType == DbType.String)
                    {
                        return "NVARCHAR(max)";
                    }

                    if (dbType == DbType.AnsiString)
                    {
                        return "VARCHAR(max)";
                    }

                    if (dbType == DbType.Binary)
                    {
                        return "VARBINARY(max)";
                    }
                }
                else
                {
                    if (dbType == DbType.String)
                    {
                        return "NVARCHAR(" + length + ")";
                    }

                    if (dbType == DbType.AnsiString)
                    {
                        return "VARCHAR(" + length + ")";
                    }

                    if (dbType == DbType.Binary)
                    {
                        return "VARBINARY(" + length + ")";
                    }
                }
            }

            if (ColumnTypes.TryGetValue(dbType, out string value))
            {
                return value;
            }

            throw new Exception("DbType not found for: " + dbType);
        }

        public override void Page(ISqlBuilder sqlBuilder, string offset, string limit)
        {
            if (offset != null)
            {
                sqlBuilder.Trail(" OFFSET ");
                sqlBuilder.Trail(offset);
                sqlBuilder.Trail(" ROWS");

                if (limit != null)
                {
                    sqlBuilder.Trail(" FETCH NEXT ");
                    sqlBuilder.Trail(limit);
                    sqlBuilder.Trail(" ROWS ONLY");
                }
            }
            else if (limit != null)
            {
                // Insert LIMIT clause after the select with brackets for parameters
                sqlBuilder.InsertSelector(" ");
                sqlBuilder.InsertSelector("(" + limit + ")");
                sqlBuilder.InsertSelector("TOP ");
            }
        }

        public override string GetDropIndexString(string indexName, string tableName)
        {
            return "drop index if exists " + QuoteForColumnName(indexName) + " on " + QuoteForTableName(tableName);
        }

        public override string QuoteForColumnName(string columnName)
        {
            return "[" + columnName + "]";
        }

        public override string QuoteForTableName(string tableName)
        {
            return "[" + tableName + "]";
        }

        public override void Concat(StringBuilder builder, params Action<StringBuilder>[] generators)
        {
            builder.Append("(");

            for (var i = 0; i < generators.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" + ");
                }

                generators[i](builder);
            }

            builder.Append(")");
        }
    }
}
