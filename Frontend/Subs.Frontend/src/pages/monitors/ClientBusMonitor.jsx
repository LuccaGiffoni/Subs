import { useEffect, useState } from "react";
import api from "../../api";

function getStatusColor(status) {
  switch (status) {
    case "Processed":
      return "bg-green-100 text-green-800";
    case "Failed":
      return "bg-red-100 text-red-800";
    case "Processing":
      return "bg-yellow-100 text-yellow-800";
    case "Received":
      return "bg-gray-200 text-gray-700";
    default:
      return "bg-gray-100 text-gray-500";
  }
}

function formatUtcDate(dateStr) {
  if (!dateStr) return "";
  const date = new Date(dateStr);
  return date.toLocaleString("en-US", {
    timeZone: "UTC",
    year: "numeric",
    month: "short",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  }) + " UTC";
}

export default function ClientBusMonitor() {
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const fetchQueue = async () => {
    setLoading(true);
    try {
      const res = await api.get("/bus/clients");
      if (Array.isArray(res.data.items)) {
        setMessages(res.data.items);
      } else {
        setMessages([]);
      }
    } catch (err) {
      setMessages([]);
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchQueue();
  }, []);

  const filteredMessages = messages.filter(
    (msg) =>
      !filter ||
      (msg.client?.id &&
        msg.client.id.toString().toLowerCase().includes(filter.toLowerCase()))
  );

  return (
    <div className="p-8">
      <h2 className="text-2xl font-bold mb-6">Client Bus Queue Monitor</h2>
      <div className="flex gap-2 mb-4">
        <input
          className="border rounded px-2 py-1"
          placeholder="Filter by Client ID..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button
          className="px-4 py-2 bg-blue-600 text-white rounded"
          onClick={fetchQueue}
          disabled={loading}
        >
          Refresh
        </button>
      </div>
      {loading ? (
        <div>Loading...</div>
      ) : (
        <table className="w-full bg-white shadow rounded">
          <thead>
            <tr className="bg-gray-100">
              <th className="p-2 text-left">Client ID</th>
              <th className="p-2 text-left">Status</th>
              <th className="p-2 text-left">Created At</th>
              <th className="p-2 text-left">Operation</th>
            </tr>
          </thead>
          <tbody>
            {filteredMessages.length === 0 ? (
              <tr>
                <td colSpan={4} className="p-4 text-center text-gray-400">
                  No messages in queue.
                </td>
              </tr>
            ) : (
              filteredMessages.map((msg, idx) => (
                <tr key={idx} className="border-t">
                  <td className="p-2">{msg.clientId}</td>
                  <td className="p-2">
                    <span className={`px-2 py-1 rounded font-semibold text-xs ${getStatusColor(msg.status)}`}>
                      {msg.status || "—"}
                    </span>
                  </td>
                  <td className="p-2">{formatUtcDate(msg.createdAt)}</td>
                  <td className="p-2">{msg.operation}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}