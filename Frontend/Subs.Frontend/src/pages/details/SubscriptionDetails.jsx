import { useEffect, useState } from "react";
import api from "../../api";

function formatCurrency(amount, currency = "BRL") {
  if (amount == null) return "";
  return amount.toLocaleString("pt-BR", {
    style: "currency",
    currency,
    minimumFractionDigits: 2,
  });
}

export default function SubscriptionDetailsSidebar({ open, onClose, subscription }) {
  const [history, setHistory] = useState([]);
  const [loadingHistory, setLoadingHistory] = useState(false);

  useEffect(() => {
    if (open && subscription?.id) {
      setLoadingHistory(true);
      api
        .get(`/subscriptions/${subscription.id}/history`)
        .then((res) => {
          const history = Array.isArray(res.data.history)
            ? res.data.history.map(event => ({
                ...event,
                representsRollbackEvent: event.rollbackId && event.rollbackId !== "00000000-0000-0000-0000-000000000000"
              }))
            : [];
          setHistory(history);
        })
        .catch(() => setHistory([]))
        .finally(() => setLoadingHistory(false));
    } else {
      setHistory([]);
    }
  }, [open, subscription?.id]);

  if (!open || !subscription) return null;

  return (
    <div className="fixed top-0 right-0 h-full w-[600px] bg-white shadow-2xl z-50 flex flex-col transition-transform duration-300"
         style={{ transform: open ? "translateX(0)" : "translateX(100%)" }}>
      <div className="flex justify-between items-center p-6 border-b">
        <h2 className="text-2xl font-bold">Subscription Details</h2>
        <button
          className="text-gray-500 hover:text-gray-700"
          onClick={onClose}
        >
          <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
      <div className="p-6 overflow-y-auto flex-1">
        <div className="mb-4">
          <span className="font-semibold">ID:</span> {subscription.id}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Status:</span> {subscription.status}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Created At:</span> {subscription.createdAt}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Updated At:</span> {subscription.updatedAt}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Client:</span> {subscription.client?.firstName} {subscription.client?.lastName}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Product ID:</span> {subscription.productId}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Payment Method:</span> {subscription.payment?.method}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Payment Frequency:</span> {subscription.payment?.frequency}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Amount:</span> {formatCurrency(subscription.payment?.amount, subscription.payment?.currency?.code || "BRL")}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Currency:</span> {subscription.payment?.currency?.code}
        </div>
        <div className="mb-4">
          <span className="font-semibold">Total Amount:</span> {formatCurrency(subscription.payment?.totalAmount, subscription.payment?.currency?.code || "BRL")}
        </div>
        {subscription.payment?.discount && (
          <div className="mb-4">
            <span className="font-semibold">Discount:</span> {subscription.payment.discount.type} - {subscription.payment.discount.value}
          </div>
        )}

        <hr className="my-6 border-gray-300" />

        <h3 className="text-xl font-bold mb-4">Event History</h3>
        <div className="bg-gray-50 rounded-lg shadow-inner p-4 max-h-72 overflow-y-auto">
          {loadingHistory ? (
            <div className="text-gray-500">Loading history...</div>
          ) : history.length === 0 ? (
            <div className="text-gray-400">No event history found for this subscription.</div>
          ) : (
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-gray-100">
                  <th className="p-2 text-left">Date</th>
                  <th className="p-2 text-left">Status</th>
                  <th className="p-2 text-left">Note</th>
                  <th className="p-2 text-left">Rollback</th>
                </tr>
              </thead>
              <tbody>
                {history.map((event) => (
                  <tr key={event.id} className="border-t">
                    <td className="p-2">{new Date(event.createdAt).toLocaleString()}</td>
                    <td className="p-2">{event.operation}</td>
                    <td className="p-2">{event.note || <span className="text-gray-400">—</span>}</td>
                    <td className="p-2">
                      {event.representsRollbackEvent ? (
                        <span className="px-2 py-1 rounded bg-yellow-200 text-yellow-800 text-xs font-semibold">Rollback</span>
                      ) : (
                        <span className="text-gray-400 text-xs">No</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}