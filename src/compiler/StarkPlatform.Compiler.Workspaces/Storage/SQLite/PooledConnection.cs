// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using StarkPlatform.Compiler.Host;
using StarkPlatform.Compiler.SQLite.Interop;

namespace StarkPlatform.Compiler.SQLite
{
    internal partial class SQLitePersistentStorage : AbstractPersistentStorage
    {
        private struct PooledConnection : IDisposable
        {
            private readonly SQLitePersistentStorage sqlitePersistentStorage;
            public readonly SqlConnection Connection;

            public PooledConnection(SQLitePersistentStorage sqlitePersistentStorage, SqlConnection sqlConnection)
            {
                this.sqlitePersistentStorage = sqlitePersistentStorage;
                Connection = sqlConnection;
            }

            public void Dispose()
            {
                sqlitePersistentStorage.ReleaseConnection(Connection);
            }
        }
    }
}
