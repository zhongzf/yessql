using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace YesSql.Provider.Cosmos.Client
{
    public class CosmosParameterCollection : DbParameterCollection
    {
        private List<CosmosParameter> _items;

        override public int Count
        {
            get
            {
                return ((null != _items) ? _items.Count : 0);
            }
        }

        private List<CosmosParameter> InnerList
        {
            get
            {
                List<CosmosParameter> items = _items;

                if (null == items)
                {
                    items = new List<CosmosParameter>();
                    _items = items;
                }
                return items;
            }
        }


        override public object SyncRoot
        {
            get
            {
                return ((System.Collections.ICollection)InnerList).SyncRoot;
            }
        }

        override public int Add(object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, value);
            InnerList.Add((CosmosParameter)value);
            return Count - 1;
        }

        override public void AddRange(System.Array values)
        {
            OnChange();
            if (null == values)
            {
                throw new ArgumentNullException(nameof(values));
            }
            foreach (object value in values)
            {
                ValidateType(value);
            }
            foreach (CosmosParameter value in values)
            {
                Validate(-1, value);
                InnerList.Add((CosmosParameter)value);
            }
        }

        private int CheckName(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameterName));
            }
            return index;
        }

        override public void Clear()
        {
            OnChange();
            List<CosmosParameter> items = InnerList;

            if (null != items)
            {
                foreach (CosmosParameter item in items)
                {
                    item.ResetParent();
                }
                items.Clear();
            }
        }

        override public bool Contains(object value)
        {
            return (-1 != IndexOf(value));
        }

        override public void CopyTo(Array array, int index)
        {
            ((System.Collections.ICollection)InnerList).CopyTo(array, index);
        }

        override public System.Collections.IEnumerator GetEnumerator()
        {
            return ((System.Collections.ICollection)InnerList).GetEnumerator();
        }

        override protected DbParameter GetParameter(int index)
        {
            RangeCheck(index);
            return InnerList[index];
        }

        override protected DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameterName));
            }
            return InnerList[index];
        }

        private static int IndexOf(System.Collections.IEnumerable items, string parameterName)
        {
            if (null != items)
            {
                int i = 0;

                foreach (CosmosParameter parameter in items)
                {
                    if (parameterName == parameter.ParameterName)
                    {
                        return i;
                    }
                    ++i;
                }
                i = 0;

                foreach (CosmosParameter parameter in items)
                {
                    if (0 == string.Compare(parameterName, parameter.ParameterName))
                    {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        override public int IndexOf(string parameterName)
        {
            return IndexOf(InnerList, parameterName);
        }

        override public int IndexOf(object value)
        {
            if (null != value)
            {
                ValidateType(value);

                List<CosmosParameter> items = InnerList;

                if (null != items)
                {
                    int count = items.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (value == items[i])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        override public void Insert(int index, object value)
        {
            OnChange();
            ValidateType(value);
            Validate(-1, (CosmosParameter)value);
            InnerList.Insert(index, (CosmosParameter)value);
        }

        private void RangeCheck(int index)
        {
            if ((index < 0) || (Count <= index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        override public void Remove(object value)
        {
            OnChange();
            ValidateType(value);
            int index = IndexOf(value);
            if (-1 != index)
            {
                RemoveIndex(index);
            }
            else if (this != ((CosmosParameter)value).CompareExchangeParent(null, this))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        override public void RemoveAt(int index)
        {
            OnChange();
            RangeCheck(index);
            RemoveIndex(index);
        }

        override public void RemoveAt(string parameterName)
        {
            OnChange();
            int index = CheckName(parameterName);
            RemoveIndex(index);
        }

        private void RemoveIndex(int index)
        {
            List<CosmosParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "RemoveIndex, invalid");
            CosmosParameter item = items[index];
            items.RemoveAt(index);
            item.ResetParent();
        }

        private void Replace(int index, object newValue)
        {
            List<CosmosParameter> items = InnerList;
            Debug.Assert((null != items) && (0 <= index) && (index < Count), "Replace Index invalid");
            ValidateType(newValue);
            Validate(index, newValue);
            CosmosParameter item = items[index];
            items[index] = (CosmosParameter)newValue;
            item.ResetParent();
        }

        override protected void SetParameter(int index, DbParameter value)
        {
            OnChange();
            RangeCheck(index);
            Replace(index, value);
        }

        override protected void SetParameter(string parameterName, DbParameter value)
        {
            OnChange();
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameterName));
            }
            Replace(index, value);
        }

        private void Validate(int index, object value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            object parent = ((CosmosParameter)value).CompareExchangeParent(this, null);
            if (null != parent)
            {
                if (this != parent)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (index != IndexOf(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            string name = ((CosmosParameter)value).ParameterName;
            if (0 == name.Length)
            {
                index = 1;
                do
                {
                    name = "Parameter" + index.ToString(CultureInfo.CurrentCulture);
                    index++;
                } while (-1 != IndexOf(name));
                ((CosmosParameter)value).ParameterName = name;
            }
        }

        private void ValidateType(object value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }
            else if (!s_itemType.IsInstanceOfType(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        private bool _isDirty;
        private static Type s_itemType = typeof(CosmosParameter);

        public CosmosParameterCollection() : base()
        {
        }

        internal bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }
        public override bool IsFixedSize => ((System.Collections.IList)InnerList).IsFixedSize;
        public override bool IsReadOnly => ((System.Collections.IList)InnerList).IsReadOnly;
        new public CosmosParameter this[int index]
        {
            get
            {
                return (CosmosParameter)GetParameter(index);
            }
            set
            {
                SetParameter(index, value);
            }
        }

        new public CosmosParameter this[string parameterName]
        {
            get
            {
                return (CosmosParameter)GetParameter(parameterName);
            }
            set
            {
                SetParameter(parameterName, value);
            }
        }

        public CosmosParameter Add(CosmosParameter value)
        {
            Add((object)value);
            return value;
        }

        public CosmosParameter AddWithValue(string parameterName, object value)
        { // 79027
            return Add(new CosmosParameter(parameterName, value));
        }

        public void AddRange(CosmosParameter[] values)
        {
            AddRange((Array)values);
        }

        override public bool Contains(string value)
        { 
            return (-1 != IndexOf(value));
        }

        public bool Contains(CosmosParameter value)
        {
            return (-1 != IndexOf(value));
        }

        public void CopyTo(CosmosParameter[] array, int index)
        {
            CopyTo((Array)array, index);
        }

        public int IndexOf(CosmosParameter value)
        {
            return IndexOf((object)value);
        }

        public void Insert(int index, CosmosParameter value)
        {
            Insert(index, (object)value);
        }

        private void OnChange()
        {
            IsDirty = true;
        }

        public void Remove(CosmosParameter value)
        {
            Remove((object)value);
        }
    }
}
