import { createContext, useContext, useState } from "react";

const AlertContext = createContext();

export function AlertProvider({ children }) {
  const [alert, setAlert] = useState(null);
  const [showDetails, setShowDetails] = useState(false);

  const showAlert = (message, onConfirm, onCancel, errorDetails = null) => {
    setAlert({ message, onConfirm, onCancel, errorDetails });
    setShowDetails(false);
  };

  const handleConfirm = () => {
    if (alert?.onConfirm) alert.onConfirm();
    setAlert(null);
    setShowDetails(false);
  };

  const handleCancel = () => {
    if (alert?.onCancel) alert.onCancel();
    setAlert(null);
    setShowDetails(false);
  };

  const handleCopy = () => {
    if (alert?.errorDetails) {
      navigator.clipboard.writeText(
        typeof alert.errorDetails === "string"
          ? alert.errorDetails
          : JSON.stringify(alert.errorDetails, null, 2)
      );
    }
  };

  return (
    <AlertContext.Provider value={{ showAlert }}>
      {children}
      {alert && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-lg p-6 w-full max-w-sm">
            <div className="mb-4 text-lg">{alert.message}</div>
            {alert.errorDetails && (
              <div className="mb-2">
                <button
                  className="text-xs text-blue-600 underline mr-2"
                  onClick={() => setShowDetails((v) => !v)}
                >
                  {showDetails ? "Hide Details" : "Show Details"}
                </button>
                <button
                  className="text-xs text-gray-600 underline"
                  onClick={handleCopy}
                >
                  Copy
                </button>
                {showDetails && (
                  <pre className="mt-2 p-2 bg-gray-100 rounded text-xs max-h-40 overflow-auto">
                    {typeof alert.errorDetails === "string"
                      ? alert.errorDetails
                      : JSON.stringify(alert.errorDetails, null, 2)}
                  </pre>
                )}
              </div>
            )}
            <div className="flex justify-end gap-2 mt-2">
              <button
                className="px-4 py-2 rounded bg-gray-200 hover:bg-gray-300"
                onClick={handleCancel}
              >
                Cancel
              </button>
              {alert.onConfirm && (
                <button
                  className="px-4 py-2 rounded bg-red-600 text-white hover:bg-red-700"
                  onClick={handleConfirm}
                >
                  Confirm
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </AlertContext.Provider>
  );
}

export function useAlert() {
  return useContext(AlertContext);
}