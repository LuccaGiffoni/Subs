import { useEffect, useState } from "react";
import api from "../api";
import CreateClientModal from "../modals/client/CreateClient.jsx";
import EditClientModal from "../modals/client/EditClient.jsx";
import ClientDetailsSidebar from "./details/ClientDetails.jsx";
import { useNotification } from "../services/NotificationService";
import { useAlert } from "../services/AlertService";
import ClientBusMonitor from "./monitors/ClientBusMonitor.jsx";

const PAGE_SIZE = 10;

export default function Clients() {
  const [clients, setClients] = useState([]);
  const [total, setTotal] = useState(0);
  const [filter, setFilter] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedClient, setSelectedClient] = useState(null);
  const [detailsOpen, setDetailsOpen] = useState(false);
  const [detailsClient, setDetailsClient] = useState(null);
  const [page, setPage] = useState(1);
  const [busMonitorOpen, setBusMonitorOpen] = useState(false);

  const { notify } = useNotification();
  const { showAlert } = useAlert();

  const fetchClients = async () => {
    const res = await api.get(`/clients?page=${page}&pageSize=${PAGE_SIZE}`);
    setClients(res.data.clients || res.data.subscriptions || res.data);
    setTotal(res.data.total || res.data.length || 0);
  };

  useEffect(() => {
    fetchClients();
  }, [page]);

  const handleDelete = async (id) => {
    showAlert(
      "Are you sure you want to delete this client?",
      async () => {
        try {
          await api.delete(`/clients/${id}`);
          fetchClients();
          notify("Client deleted successfully.", "success");
        } catch (error) {
          console.error(error);
          notify("Failed to delete client. Please try again.", "error");
        }
      },
      () => {
        notify("Deletion cancelled.", "info");
      }
    );
  };

  const handleEdit = (client) => {
    setSelectedClient(client);
    setEditModalOpen(true);
  };

  const handleCreate = async (data) => {
    try {
      await api.post("/clients", data);
      setPage(1);
      fetchClients();
      notify("Client created successfully!", "success");
    } catch (error) {
      console.error(error);
      showAlert(
        error?.response?.data?.message || "Failed to create client. Please try again.",
        null,
        null,
        error?.response?.data || error?.message || error
      );
    }
  };

  const handleUpdate = async (data) => {
    try {
      await api.put(`/clients/${data.id}`, data);
      fetchClients();
      notify("Client updated successfully!", "success");
    } catch (error) {
      console.error(error);
      showAlert(
        error?.response?.data?.message || "Failed to update client. Please try again.",
        null,
        null,
        error?.response?.data || error?.message || error
      );
    }
  };

  const handleDetails = (client) => {
    setDetailsClient(client);
    setDetailsOpen(true);
  };

  const filtered = clients.filter((c) =>
    `${c.firstName} ${c.lastName}`.toLowerCase().includes(filter.toLowerCase())
  );

  const totalPages = Math.ceil(total / PAGE_SIZE);

  const handlePrev = () => setPage((p) => Math.max(1, p - 1));
  const handleNext = () => setPage((p) => Math.min(totalPages, p + 1));

  useEffect(() => {
    if (page > totalPages) setPage(1);
  }, [filter, totalPages]);

  return (
    <div>
      <h2 className="text-lg font-bold mb-4">Clientes</h2>
      <div className="flex gap-2 mb-4">
        <input
          className="border rounded px-2 py-1"
          placeholder="Filter by name..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button
          className="bg-green-600 text-white px-3 py-1 rounded"
          onClick={() => setModalOpen(true)}
        >
          Create client
        </button>
        <button
          className="bg-indigo-600 text-white px-3 py-1 rounded"
          onClick={() => setBusMonitorOpen(true)}
        >
          Bus Monitor
        </button>
      </div>
      <table className="w-full bg-white shadow rounded">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">ID</th>
            <th className="text-left p-2">Name</th>
            <th className="text-left p-2">Email</th>
            <th className="text-left p-2">Phone</th>
            <th className="text-left p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((c) => (
            <tr key={c.id} className="border-t">
              <td className="p-2">{c.id}</td>
              <td className="p-2">{c.firstName} {c.lastName}</td>
              <td className="p-2">{c.email}</td>
              <td className="p-2">{c.phone}</td>
              <td className="p-2 flex gap-2">
                <button
                  className="text-indigo-600 hover:text-indigo-900"
                  title="Edit"
                  onClick={() => handleEdit(c)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 inline" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.232 5.232l3.536 3.536M9 13l6.586-6.586a2 2 0 112.828 2.828L11.828 15.828a2 2 0 01-2.828 0L9 13zm0 0V17h4" />
                  </svg>
                </button>
                <button
                  className="text-red-600 hover:text-red-900"
                  title="Delete"
                  onClick={() => handleDelete(c.id)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 inline" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
                <button
                  className="text-gray-600 hover:text-gray-900"
                  title="Details"
                  onClick={() => handleDetails(c)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 inline" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M12 20a8 8 0 100-16 8 8 0 000 16z" />
                  </svg>
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="flex justify-center items-center gap-4 mt-4">
        <button
          className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300"
          onClick={handlePrev}
          disabled={page === 1}
        >
          Previous
        </button>
        <span>
          Page {page} of {totalPages || 1}
        </span>
        <button
          className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300"
          onClick={handleNext}
          disabled={page === totalPages || totalPages === 0}
        >
          Next
        </button>
      </div>
      <CreateClientModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreate={handleCreate}
      />
      <EditClientModal
        open={editModalOpen}
        onClose={() => setEditModalOpen(false)}
        onUpdate={handleUpdate}
        client={selectedClient}
      />
      <ClientDetailsSidebar
        open={detailsOpen}
        onClose={() => setDetailsOpen(false)}
        client={detailsClient}
      />
      {busMonitorOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-30 z-50 flex items-center justify-center">
          <div className="bg-white rounded shadow-lg w-[900px] max-h-[90vh] overflow-auto relative">
            <button
              className="absolute top-2 right-2 text-gray-500 hover:text-gray-700"
              onClick={() => setBusMonitorOpen(false)}
            >
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
            <ClientBusMonitor />
          </div>
        </div>
      )}
    </div>
  );
}