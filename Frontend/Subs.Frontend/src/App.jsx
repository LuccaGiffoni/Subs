import { BrowserRouter, Routes, Route } from "react-router-dom";
import Sidebar from "./components/Sidebar";
import Header from "./components/Header";
import Clients from "./pages/Clients";
import Subscriptions from "./pages/Subscriptions";

export default function App() {
  return (
    <BrowserRouter>
      <div className="flex h-screen">
        <Sidebar />
        <div className="flex-1 flex flex-col">
          <Header title="Subs Dashboard" />
          <main className="p-6 bg-gray-50 flex-1 overflow-auto">
            <Routes>
              <Route path="/clients" element={<Clients />} />
              <Route path="/subscriptions" element={<Subscriptions />} />
              <Route path="*" element={<div>Selecione uma opção no menu</div>} />
            </Routes>
          </main>
        </div>
      </div>
    </BrowserRouter>
  );
}