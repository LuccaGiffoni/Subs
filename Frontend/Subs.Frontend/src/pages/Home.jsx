export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-700 via-purple-600 to-pink-500 flex flex-col items-center justify-center text-white relative overflow-hidden">
      <div className="absolute inset-0 pointer-events-none z-0">
        <div className="absolute top-10 left-1/4 w-72 h-72 bg-pink-400 opacity-30 rounded-full blur-3xl animate-spin-slow"></div>
        <div className="absolute bottom-20 right-1/3 w-96 h-96 bg-indigo-400 opacity-20 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute top-1/2 left-2/3 w-40 h-40 bg-purple-400 opacity-30 rounded-full blur-2xl animate-bounce"></div>
      </div>
      <div className="relative z-10 bg-white bg-opacity-10 backdrop-blur-2xl rounded-3xl shadow-2xl p-12 flex flex-col items-center border border-white/20">
        <h1 className="text-6xl font-extrabold mb-4 drop-shadow-2xl animate-gradient-x bg-gradient-to-r from-pink-400 via-purple-400 to-indigo-400 bg-clip-text text-transparent">
          Subs Dashboard
        </h1>
        <p className="text-2xl mb-8 text-center max-w-2xl font-light">
          Welcome to your <span className="font-bold text-pink-200">crazy beautiful</span> subscription management platform.<br />
          <span className="font-bold text-indigo-200">Track</span>, <span className="font-bold text-purple-200">analyze</span>, and <span className="font-bold text-pink-200">monitor</span> all your clients and subscriptions in real time.<br />
          <span className="italic text-lg text-white/80">Modern. Fast. Magical.</span>
        </p>
        <div className="flex gap-8 mb-8">
          <a
            href="/clients"
            className="px-8 py-4 rounded-2xl bg-indigo-700 hover:bg-indigo-800 transition shadow-xl font-semibold text-xl hover:scale-105 transform duration-200"
          >
            Clients
          </a>
          <a
            href="/subscriptions"
            className="px-8 py-4 rounded-2xl bg-purple-700 hover:bg-purple-800 transition shadow-xl font-semibold text-xl hover:scale-105 transform duration-200"
          >
            Subscriptions
          </a>
        </div>
        <div className="mt-8 text-lg text-white/80 flex items-center gap-2">
          <span className="animate-bounce text-3xl">✨</span>
          <span>
            Powered by <span className="font-bold text-indigo-200">Azure</span>, <span className="font-bold text-purple-200">React</span>, and <span className="font-bold text-pink-200">your imagination</span>!
          </span>
        </div>
      </div>
      <style>
        {`
          .animate-gradient-x {
            background-size: 200% 200%;
            animation: gradient-x 3s ease-in-out infinite;
          }
          @keyframes gradient-x {
            0%, 100% { background-position: left center; }
            50% { background-position: right center; }
          }
          .animate-spin-slow {
            animation: spin 12s linear infinite;
          }
          @keyframes spin {
            100% { transform: rotate(360deg); }
          }
        `}
      </style>
    </div>
  );
}