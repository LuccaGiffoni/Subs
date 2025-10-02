import { useState, useEffect } from "react";

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

export default function EditSubscriptionModal({ open, onClose, onUpdate, subscription }) {
  const [form, setForm] = useState({
    createdAt: "",
    updatedAt: "",
    paymentMethod: "",
    paymentFrequency: "",
    amount: "",
    currency: "BRL",
    clientId: "",
    productId: "",
    discountType: "",
    discountValue: "",
  });
  const [showDiscount, setShowDiscount] = useState(false);

  useEffect(() => {
    if (subscription) {
      setForm({
        createdAt: subscription.createdAt?.slice(0, 16) || "",
        updatedAt: subscription.updatedAt?.slice(0, 16) || "",
        paymentMethod: subscription.payment?.method || "",
        paymentFrequency: subscription.payment?.frequency || "",
        amount: subscription.payment?.amount || "",
        currency: subscription.payment?.currency?.code || "BRL",
        clientId: subscription.client?.id || "",
        productId: subscription.productId || "",
        discountType: subscription.payment?.discount?.type || "",
        discountValue: subscription.payment?.discount?.value || "",
      });
      setShowDiscount(!!subscription.payment?.discount);
    }
  }, [subscription]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const payment = {
      method: form.paymentMethod,
      frequency: form.paymentFrequency,
      amount: parseFloat(form.amount),
      currency: { code: form.currency },
    };

    if (showDiscount && form.discountType && form.discountValue) {
      payment.discount = {
        type: form.discountType,
        value: parseFloat(form.discountValue) || 0,
      };
    }

    const updatedSubscription = {
      ...subscription,
      createdAt: form.createdAt,
      updatedAt: form.updatedAt,
      payment,
      client: {
        id: form.clientId,
      },
      productId: form.productId,
    };

    onUpdate(updatedSubscription);
    onClose();
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-2xl">
        <h2 className="text-xl font-bold mb-6">Edit Subscription</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium">Client ID</label>
              <input
                name="clientId"
                value={form.clientId}
                onChange={handleChange}
                className="w-full border rounded px-3 py-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium">Product ID</label>
              <input
                name="productId"
                value={form.productId}
                onChange={handleChange}
                className="w-full border rounded px-3 py-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium">Created At</label>
              <input
                name="createdAt"
                type="datetime-local"
                value={form.createdAt}
                onChange={handleChange}
                className="w-full border rounded px-3 py-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium">Updated At</label>
              <input
                name="updatedAt"
                type="datetime-local"
                value={form.updatedAt}
                onChange={handleChange}
                className="w-full border rounded px-3 py-2"
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
              Save
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}