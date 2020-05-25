using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using YesSql.Provider.Cosmos.Helpers;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosDataReader : DbDataReader
    {
        public FeedIteratorReader FeedIteratorReader { get; set; }

        public object CurrentObject { get; set; }

        public override object this[int ordinal] => GetObjectValue(ordinal);

        public override object this[string name] => GetObjectValue(name);

        public object GetObjectValue(int ordinal)
        {
            // TODO:
            return null;
        }

        public object GetObjectValue(string name)
        {
            // TODO:
            return null;
        }

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount
        {
            get
            {
                // TODO:
                return 1;
            }
        }

        public override bool HasRows => FeedIteratorReader.FeedIterator.HasMoreResults;

        public override bool IsClosed => false;

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(this[ordinal]);
        }

        public override byte GetByte(int ordinal)
        {
            return Convert.ToByte(this[ordinal]);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return Convert.ToChar(this[ordinal]);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Convert.ToDateTime(this[ordinal]);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Convert.ToDecimal(this[ordinal]);
        }

        public override double GetDouble(int ordinal)
        {
            return Convert.ToDouble(this[ordinal]);
        }

        public override Type GetFieldType(int ordinal)
        {
            // TODO:
            return typeof(string);
        }

        public override float GetFloat(int ordinal)
        {
            return Convert.ToSingle(this[ordinal]);
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            return Convert.ToInt16(this[ordinal]);
        }

        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(this[ordinal]);
        }

        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(this[ordinal]);
        }

        public override string GetString(int ordinal)
        {
            return Convert.ToString(this[ordinal]);
        }

        public override object GetValue(int ordinal)
        {
            return this[ordinal];
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            return this[ordinal] == null;
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            return HasRows;
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            // TODO:
            return ordinal.ToString();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            try
            {
                CurrentObject = FeedIteratorReader.ReadNext().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(FeedIteratorReader.CommandText);
                Debug.WriteLine(FeedIteratorReader.QueryText);
                Debug.WriteLine(ex);
            }
            return CurrentObject != null;
        }
    }
}
