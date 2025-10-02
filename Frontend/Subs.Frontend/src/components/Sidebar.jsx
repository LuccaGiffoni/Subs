import { NavLink } from "react-router-dom";
import { UserGroupIcon, NewspaperIcon, MapIcon, QueueListIcon } from "@heroicons/react/24/outline";

export default function Sidebar() {
  const navItems = [
    { name: "Clients", to: "/clients", icon: UserGroupIcon },
    { name: "Subscriptions", to: "/subscriptions", icon: NewspaperIcon },
  ];

  return (
    <div className="h-screen w-64 bg-gray-900 text-white flex flex-col">
      <div className="text-2xl font-bold p-4">Subs</div>
      <nav className="flex-1">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `flex items-center gap-2 px-4 py-2 hover:bg-gray-700 ${
                isActive ? "bg-gray-800" : ""
              }`
            }
          >
            <item.icon className="h-5 w-5" />
            {item.name}
          </NavLink>
        ))}
      </nav>
    </div>
  );
}