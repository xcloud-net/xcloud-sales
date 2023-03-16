using System.Collections.Concurrent;
using System.Collections.Generic;

namespace XCloud.Platform.Application.Messenger.Connection;

public class ConnectionManager : IDisposable
{
    private readonly ConcurrentDictionary<string, IConnection> _connections =
        new ConcurrentDictionary<string, IConnection>();

    private string BuildKey(IConnection connection)
    {
        return $"{connection.ClientIdentity.ConnectionId}@{connection.ClientIdentity.SubjectId}";
    }

    public IReadOnlyList<IConnection> AsReadOnlyList()
    {
        return this._connections.Values.ToArray();
    }

    public void AddConnection(IConnection con)
    {
        if (con == null)
            throw new ArgumentNullException(nameof(con));

        this._connections[BuildKey(con)] = con;
    }

    public void RemoveConnection(IConnection con)
    {
        this._connections.TryRemove(BuildKey(con), out var _);

        var items = this._connections.Where(x => x.Value == con).ToArray();
        foreach (var m in items)
        {
            this._connections.TryRemove(m.Key, out var _);
        }
    }

    public void Dispose()
    {
        foreach (var m in this._connections.Values)
        {
            m.Dispose();
        }

        this._connections.Clear();
    }
}