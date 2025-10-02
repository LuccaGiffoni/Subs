import { useState, useEffect, useRef } from "react";
import api from "../../api";

const paymentMethods = [
  { value: "Credit", label: "Credit Card" },
  { value: "Debit", label: "Debit Card" },
  { value: "BankSlip", label: "Bank Slip" },
  { value: "PIX", label: "PIX" },
];

const paymentFrequencies = [
  { value: "Bullet", label: "Bullet" },
  { value: "Monthly", label: "Monthly" },
  { value: "Bimonthly", label: "Bimonthly" },
  { value: "Quarterly", label: "Quarterly" },
  { value: "Semiannual", label: "Semiannual" },
  { value: "Annual", label: "Annual" },
];

const discountTypes = [
  { value: "Absolute", label: "Absolute (fixed amount)" },
  { value: "Percentage", label: "Percentage (%)" },
];

const CLIENT_PAGE_SIZE = 10;

export default function SubscriptionModal({ open, onClose, onCreate }) {
  const [form, setForm] = useState({
    createdAt: "",
    updatedAt: "",
    paymentMethod: "",
    paymentFrequency: "",
    amount: "",
    currency: "Real",
    clientId: "",
    productId: "",
    discountType: "",
    discountValue: "",
  });

  const [showDiscount, setShowDiscount] = useState(false);
  const [clientSearch, setClientSearch] = useState("");
  const [clientPage, setClientPage] = useState(1);
  const [clients, setClients] = useState([]);
  const [clientTotal, setClientTotal] = useState(0);
  const [filteredClients, setFilteredClients] = useState([]);
  const debounceRef = useRef();

  const fetchClients = async (search = "", page = 1) => {
    const res = await api.get(`/clients?page=${page}&pageSize=${CLIENT_PAGE_SIZE}&search=${encodeURIComponent(search)}`);
    if (res.data.clients) {
      setClients(res.data.clients);
      setClientTotal(res.data.total);
    } else if (Array.isArray(res.data)) {
      setClients(res.data);
      setClientTotal(res.data.length);
    }
  };

  useEffect(() => {
    if (!open) return;
    if (debounceRef.current) clearTimeout(debounceRef.current);

    const handler = (e) => {
      if (e.key === "Tab") {
        fetchClients(clientSearch, 1);
      }
    };

    window.addEventListener("keydown", handler);

    debounceRef.current = setTimeout(() => {
      fetchClients(clientSearch, 1);
    }, 2000);

    return () => {
      window.removeEventListener("keydown", handler);
      if (debounceRef.current) clearTimeout(debounceRef.current);
    };
  }, [clientSearch, open]);

  useEffect(() => {
    if (open) fetchClients(clientSearch, clientPage);
  }, [clientPage, open]);

  useEffect(() => {
    setFilteredClients(clients);
  }, [clients]);


  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const payment = {
      method: form.paymentMethod,
      frequency: form.paymentFrequency,
      amount: parseFloat(form.amount),
      currency: { type: form.currency,  rate: 1 },
    };

    if (showDiscount && form.discountType && form.discountValue) {
      payment.discount = {
        type: form.discountType,
        value: parseFloat(form.discountValue) || 0,
      };
    }

    const subscription = {
      status: "Draft",
      payment,
      clientId: form.clientId,
      productId: form.productId,
    };

    onCreate(subscription);
    onClose();
  };

  const clientTotalPages = Math.ceil(clientTotal / CLIENT_PAGE_SIZE);

  const handleClientPrev = () => setClientPage((p) => Math.max(1, p - 1));
  const handleClientNext = () => setClientPage((p) => Math.min(clientTotalPages, p + 1));

  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-2xl">
        <h2 className="text-xl font-bold mb-6">New Subscription</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium">Client</label>
              <input
                type="text"
                value={clientSearch}
                onChange={e => setClientSearch(e.target.value)}
                className="w-full border rounded px-3 py-2 mb-2"
                placeholder="Search by name (tab to query)..."
              />
              <div className="max-h-32 overflow-y-auto border rounded bg-white shadow">
                {filteredClients.map(c => (
                  <div
                    key={c.id}
                    className={`px-3 py-2 cursor-pointer hover:bg-indigo-100 ${form.clientId === c.id ? "bg-indigo-200" : ""}`}
                    onClick={() => {
                      setForm({ ...form, clientId: c.id });
                      setClientSearch(`${c.firstName} ${c.lastName}`);
                    }}
                  >
                    {c.firstName} {c.lastName} ({c.email})
                  </div>
                ))}
                {filteredClients.length === 0 && (
                  <div className="px-3 py-2 text-gray-400">No clients found</div>
                )}
              </div>
              <div className="flex justify-center items-center gap-2 mt-2">
                <button
                  type="button"
                  className="px-2 py-1 rounded bg-gray-200 hover:bg-gray-300 text-xs"
                  onClick={handleClientPrev}
                  disabled={clientPage === 1}
                >
                  Previous
                </button>
                <span className="text-xs">
                  Page {clientPage} of {clientTotalPages || 1}
                </span>
                <button
                  type="button"
                  className="px-2 py-1 rounded bg-gray-200 hover:bg-gray-300 text-xs"
                  onClick={handleClientNext}
                  disabled={clientPage === clientTotalPages || clientTotalPages === 0}
                >
                  Next
                </button>
              </div>
              <input type="hidden" name="clientId" value={form.clientId} />
            </div>
            <div>
              <label className="block text-sm font-medium">Product ID</label>
              <input
                name="productId"
                value={form.productId}
                onChange={handleChange}
                className="w-full border rounded px-3 py-2"
                placeholder="ID do Produto"
                required
              />
            </div>
          </div>
          <div className="pt-4 border-t">
            <h3 className="font-semibold mb-2">Payment</h3>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium">Payment method</label>
                <select
                  name="paymentMethod"
                  value={form.paymentMethod}
                  onChange={handleChange}
                  className="w-full border rounded px-3 py-2"
                  required
                >
                  <option value="">Select...</option>
                  {paymentMethods.map((pm) => (
                    <option key={pm.value} value={pm.value}>{pm.label}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium">Payment frequency</label>
                <select
                  name="paymentFrequency"
                  value={form.paymentFrequency}
                  onChange={handleChange}
                  className="w-full border rounded px-3 py-2"
                  required
                >
                  <option value="">Select...</option>
                  {paymentFrequencies.map((pf) => (
                    <option key={pf.value} value={pf.value}>{pf.label}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium">Amount</label>
                <input
                  name="amount"
                  type="number"
                  value={form.amount}
                  onChange={handleChange}
                  className="w-full border rounded px-3 py-2"
                  placeholder="Valor"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Currency</label>
                <input
                  name="currency"
                  value={form.currency}
                  onChange={handleChange}
                  className="w-full border rounded px-3 py-2"
                  placeholder="BRL"
                  required
                />
              </div>
            </div>
            <div className="mt-4">
              {!showDiscount ? (
                <button
                  type="button"
                  className="px-3 py-1 rounded bg-gray-100 hover:bg-gray-200 text-sm"
                  onClick={() => setShowDiscount(true)}
                >
                  Add Discount
                </button>
              ) : (
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium">Discount type</label>
                    <select
                      name="discountType"
                      value={form.discountType}
                      onChange={handleChange}
                      className="w-full border rounded px-3 py-2"
                    >
                      <option value="">Select...</option>
                      {discountTypes.map((dt) => (
                        <option key={dt.value} value={dt.value}>{dt.label}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium">Discount value</label>
                    <input
                      name="discountValue"
                      type="number"
                      value={form.discountValue}
                      onChange={handleChange}
                      className="w-full border rounded px-3 py-2"
                      placeholder="0"
                    />
                  </div>
                  <div className="col-span-2 flex justify-end">
                    <button
                      type="button"
                      className="px-3 py-1 rounded bg-gray-100 hover:bg-gray-200 text-sm mt-2"
                      onClick={() => {
                        setShowDiscount(false);
                        setForm({ ...form, discountType: "", discountValue: "" });
                      }}
                    >
                      Remove Discount
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
          <div className="flex justify-end gap-2 mt-6">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded bg-gray-200 hover:bg-gray-300"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 rounded bg-indigo-600 text-white hover:bg-indigo-700"
            >
              Create
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}