import { type FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { apiFetch, setToken } from "../lib/api";

type LoginResponse = { token: string };

export function LoginPage() {
  const nav = useNavigate();
  const [email, setEmail] = useState("test@example.com");
  const [password, setPassword] = useState("Test123!");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await apiFetch<LoginResponse>("/api/Auth/login", {
        method: "POST",
        body: JSON.stringify({ email, password }),
      });
      setToken(res.token);
      nav("/app/orders");
    } catch (err: any) {
      setError(err?.message ?? "Login failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-md">
      <div className="rounded-2xl border border-slate-800 bg-slate-900/40 p-6 shadow">
        <h1 className="text-xl font-semibold">Sign in</h1>
        <p className="mt-1 text-sm text-slate-300">Use your API credentials.</p>

        {error && (
          <div className="mt-4 rounded-xl border border-red-900/60 bg-red-950/40 p-3 text-sm text-red-200">
            {error}
          </div>
        )}

        <form onSubmit={onSubmit} className="mt-6 space-y-4">
          <div>
            <label className="text-sm text-slate-300">Email</label>
            <input
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 text-slate-100 outline-none focus:ring-2 focus:ring-slate-700"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="email"
            />
          </div>

          <div>
            <label className="text-sm text-slate-300">Password</label>
            <input
              type="password"
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 text-slate-100 outline-none focus:ring-2 focus:ring-slate-700"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
            />
          </div>

          <button
            disabled={loading}
            className="w-full rounded-xl bg-white px-3 py-2 font-medium text-slate-950 hover:bg-slate-200 disabled:opacity-60"
          >
            {loading ? "Signing in..." : "Sign in"}
          </button>
        </form>

        <div className="mt-4 text-sm text-slate-300">
          No account?{" "}
          <Link className="text-white underline" to="/register">
            Register
          </Link>
        </div>
      </div>
    </div>
  );
}
