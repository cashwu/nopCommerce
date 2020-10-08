using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;

namespace Nop.Data.DataProviders
{
    //TODO: IDisposeAsync https://docs.microsoft.com/ru-ru/dotnet/standard/garbage-collection/implementing-disposeasync
    public class TempDataStorage<T> : IQueryable<T>, IDisposable where T : class
    {
        private readonly IDisposable _disposableFactory;
        private readonly IDisposable _disposableResource;

        public TempDataStorage(string storageName, IQueryable<T> query, Func<DataConnection> dataConnectionFactory)
        {
            if (dataConnectionFactory is null)
                throw new ArgumentNullException(nameof(dataConnectionFactory));

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var dataConnection = dataConnectionFactory();

            var tmpTable = dataConnection.CreateTempTable<T>(storageName, query);

            ElementType = tmpTable.AsQueryable().ElementType;
            Expression = tmpTable.AsQueryable().Expression;
            Provider = tmpTable.AsQueryable().Provider;

            _disposableResource = tmpTable;
            _disposableFactory = dataConnection;
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public void Dispose()
        {
            _disposableResource.Dispose();
            _disposableFactory.Dispose();
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}