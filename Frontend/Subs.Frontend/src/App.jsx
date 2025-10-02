import { BrowserRouter, Routes, Route } from "react-router-dom";
import Sidebar from "./components/Sidebar";
import Clients from "./pages/Clients";
import Home from "./pages/Home";
import Subscriptions from "./pages/Subscriptions";
import { NotificationProvider } from "./services/NotificationService";
import { AlertProvider } from "./services/AlertService";

export default function App() {
  return (
    <BrowserRouter>
      <div className="flex h-screen">
        <Sidebar />
        <div className="flex-1 flex flex-col">
          <main className="p-6 bg-gray-50 flex-1 overflow-auto">
              <NotificationProvider>
                <AlertProvider>
                  <Routes>
                    <Route path="/clients" element={<Clients />} />
                    <Route path="/subscriptions" element={<Subscriptions />} />
                    <Route path="*" element={<Home />} />
                  </Routes>
                </AlertProvider>
              </NotificationProvider>
          </main>
        </div>
      </div>
    </BrowserRouter>
  );
}