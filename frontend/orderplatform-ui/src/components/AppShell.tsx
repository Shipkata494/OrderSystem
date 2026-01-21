import { Link, Outlet, useNavigate } from "react-router-dom";
import { clearToken, getToken } from "../lib/api";

export function AppShell() {
  const nav = useNavigate();
  const authed = !!getToken();

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="sticky top-0 z-10 border-b border-slate-800 bg-slate-950/80 backdrop-blur">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
          <Link to="/app/orders" className="font-semibold tracking-tight">
            OrderPlatform
          </Link>

          <div className="flex items-center gap-3 text-sm">
            <Link to="/app/orders" className="text-slate-200 hover:text-white">
              Orders
            </Link>

            {authed ? (
              <button
                className="rounded-lg border border-slate-800 px-3 py-1.5 hover:bg-slate-900"
                onClick={() => {
                  clearToken();
                  nav("/login");
                }}
              >
                Logout
              </button>
            ) : (
              <Link
                to="/login"
                className="rounded-lg border border-slate-800 px-3 py-1.5 hover:bg-slate-900"
              >
                Login
              </Link>
            )}
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  );
}
