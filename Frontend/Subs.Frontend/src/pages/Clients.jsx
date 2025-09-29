import { useEffect, useState } from "react";
import api from "../api";

export default function Clients() {
  const [clients, setClients] = useState([]);
  const [filter, setFilter] = useState("");

  useEffect(() => {
    api.get("/clients").then(res => setClients(res.data));
  }, []);

  const filtered = clients.filter((c) =>
    `${c.firstName} ${c.lastName}`.toLowerCase().includes(filter.toLowerCase())
  );

  return (
    <div>
      <h2 className="text-lg font-bold mb-4">Clientes</h2>
      <div className="flex gap-2 mb-4">
        <input
          className="border rounded px-2 py-1"
          placeholder="Filtrar por nome..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button className="bg-blue-600 text-white px-3 py-1 rounded">
          Novo Cliente
        </button>
      </div>
      <table className="w-full bg-white shadow rounded">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">Nome</th>
            <th className="text-left p-2">Email</th>
            <th className="text-left p-2">Telefone</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((c) => (
            <tr key={c.id} className="border-t">
              <td className="p-2">{c.firstName} {c.lastName}</td>
              <td className="p-2">{c.email}</td>
              <td className="p-2">{c.phone}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}