export default function Header({ title }) {
  return (
    <header className="bg-white shadow p-4">
      <h1 className="text-xl font-semibold text-gray-800">{title}</h1>
    </header>
  );
}