import { useEffect, useState } from "react";
import api from "../api";

export default function Subscriptions() {
  const [subs, setSubs] = useState([]);
  const [filter, setFilter] = useState("");

  useEffect(() => {
    api.get("/subscriptions").then((res) => setSubs(res.data));
  }, []);

  const filtered = subs.filter((s) =>
    s.cliente.toLowerCase().includes(filter.toLowerCase())
  );

  return (
    <div>
      <h2 className="text-lg font-bold mb-4">Subscriptions</h2>
      <div className="flex gap-2 mb-4">
        <input
          className="border rounded px-2 py-1"
          placeholder="Filtrar por cliente..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button className="bg-green-600 text-white px-3 py-1 rounded">
          Nova Subscription
        </button>
      </div>
      <table className="w-full bg-white shadow rounded">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">Cliente</th>
            <th className="text-left p-2">Plano</th>
            <th className="text-left p-2">Valor</th>
            <th className="text-left p-2">Status</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((s) => (
            <tr key={s.id} className="border-t">
              <td className="p-2">{s.cliente}</td>
              <td className="p-2">{s.plano}</td>
              <td className="p-2">{s.valor}</td>
              <td className="p-2">{s.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}