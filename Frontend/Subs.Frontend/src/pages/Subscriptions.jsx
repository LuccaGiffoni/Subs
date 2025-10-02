import { useEffect, useState } from "react";
import api from "../api";
import SubscriptionModal from "../modals/subscription/CreateSubscription";
import EditSubscriptionModal from "../modals/subscription/EditSubscription";
import SubscriptionDetailsSidebar from "./details/SubscriptionDetails.jsx";
import { useNotification } from "../services/NotificationService";
import { useAlert } from "../services/AlertService";
import SubscriptionBusMonitor from "./monitors/SubscriptionBusMonitor";

const statusColors = {
  Draft: "bg-gray-300 text-gray-800",
  Pending: "bg-yellow-200 text-yellow-800",
  Active: "bg-green-200 text-green-800",
  Canceled: "bg-red-200 text-red-800",
  Expired: "bg-purple-200 text-purple-800",
  Suspended: "bg-orange-200 text-orange-800",
};

const PAGE_SIZE = 10;

export default function Subscriptions() {
  const [subs, setSubs] = useState([]);
  const [total, setTotal] = useState(0);
  const [filter, setFilter] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedSub, setSelectedSub] = useState(null);
  const [detailsOpen, setDetailsOpen] = useState(false);
  const [detailsSub, setDetailsSub] = useState(null);
  const [page, setPage] = useState(1);
  const [busMonitorOpen, setBusMonitorOpen] = useState(false);

  const { notify } = useNotification();
  const { showAlert } = useAlert();

  const fetchSubscriptions = async () => {
    const res = await api.get(`/subscriptions?page=${page}&pageSize=${PAGE_SIZE}`);
    setSubs(res.data.subscriptions);
    setTotal(res.data.total);
  };

  useEffect(() => {
    fetchSubscriptions();
  }, [page]);

  const handleEdit = (sub) => {
    setSelectedSub(sub);
    setEditModalOpen(true);
  };

  const handleCreate = async (data) => {
    try {
      await api.post("/subscriptions", data);
      setPage(1);
      fetchSubscriptions();
      notify("Subscription created successfully!", "success");
    } catch (error) {
      console.error(error);
      showAlert(
        error?.response?.data?.message || "Failed to create subscription. Please try again.",
        null,
        null,
        error?.response?.data || error?.message || error
      );
    }
  };

  const handleUpdate = async (data) => {
    try {
      await api.put(`/subscriptions/${data.id}`, data);
      fetchSubscriptions();
      notify("Subscription updated successfully!", "success");
    } catch (error) {
      console.error(error);
      showAlert(
        error?.response?.data?.message || "Failed to update subscription. Please try again.",
        null,
        null,
        error?.response?.data || error?.message || error
      );
    }
  };

  const handleDelete = async (id) => {
    showAlert(
      "Are you sure you want to delete this subscription?",
      async () => {
        try {
          await api.delete(`/subscriptions/${id}`);
          fetchSubscriptions();
          notify("Subscription deleted successfully.", "success");
        } catch (error) {
          console.error(error);
          notify("Failed to delete subscription. Please try again.", "error");
        }
      },
      () => {
        notify("Deletion cancelled.", "info");
      }
    );
  };

  const handleDetails = (sub) => {
    setDetailsSub(sub);
    setDetailsOpen(true);
  };

  const filtered = subs.filter((s) =>
    s.clientId.toLowerCase().includes(filter.toLowerCase())
  );

  const totalPages = Math.ceil(total / PAGE_SIZE);

  const handlePrev = () => setPage((p) => Math.max(1, p - 1));
  const handleNext = () => setPage((p) => Math.min(totalPages, p + 1));

  useEffect(() => {
    if (page > totalPages) setPage(1);
  }, [filter, totalPages]);

  return (
    <div>
      <h2 className="text-lg font-bold mb-4">Subscriptions</h2>
      <div className="flex gap-2 mb-4">
        <input
          className="border rounded px-2 py-1"
          placeholder="Filter by ID..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button
          className="bg-green-600 text-white px-3 py-1 rounded"
          onClick={() => setModalOpen(true)}
        >
          Create subscription
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
            <th className="text-left p-2">Status</th>
            <th className="text-left p-2">Client</th>
            <th className="text-left p-2">Product</th>
            <th className="text-left p-2">Amount</th>
            <th className="text-left p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((s) => (
            <tr key={s.id} className="border-t">
              <td className="p-2">{s.id}</td>
              <td className="p-2">
                <span
                  className={`px-3 py-1 rounded-full text-xs font-semibold inline-block ${
                    statusColors[s.status] || "bg-gray-200 text-gray-800"
                  }`}
                >
                  {s.status}
                </span>
              </td>
              <td className="p-2">{s.client.firstName} {s.client.lastName}</td>
              <td className="p-2">{s.productId}</td>
              <td className="p-2">{s.payment.totalAmount}</td>
              <td className="p-2 flex gap-2">
                <button
                  className="text-indigo-600 hover:text-indigo-900"
                  title="Edit"
                  onClick={() => handleEdit(s)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 inline" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.232 5.232l3.536 3.536M9 13l6.586-6.586a2 2 0 112.828 2.828L11.828 15.828a2 2 0 01-2.828 0L9 13zm0 0V17h4" />
                  </svg>
                </button>
                <button
                  className="text-red-600 hover:text-red-900"
                  title="Delete"
                  onClick={() => handleDelete(s.id)}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 inline" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
                <button
                  className="text-gray-600 hover:text-gray-900"
                  title="Details"
                  onClick={() => handleDetails(s)}
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
      <SubscriptionModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreate={handleCreate}
      />
      <EditSubscriptionModal
        open={editModalOpen}
        onClose={() => setEditModalOpen(false)}
        onUpdate={handleUpdate}
        subscription={selectedSub}
      />
      <SubscriptionDetailsSidebar
        open={detailsOpen}
        onClose={() => setDetailsOpen(false)}
        subscription={detailsSub}
      />
      {busMonitorOpen && (
        <SubscriptionBusMonitor open={busMonitorOpen} onClose={() => setBusMonitorOpen(false)} />
      )}
    </div>
  );
}