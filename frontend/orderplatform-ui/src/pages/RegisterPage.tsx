import {type FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { apiFetch } from "../lib/api";

export function RegisterPage() {
  const nav = useNavigate();
  const [email, setEmail] = useState("test@example.com");
  const [password, setPassword] = useState("Test123!");
  const [role, setRole] = useState("User");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await apiFetch("/api/Auth/register", {
        method: "POST",
        body: JSON.stringify({ email, password, role }),
      });
      nav("/login");
    } catch (err: any) {
      setError(err?.message ?? "Register failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-md">
      <div className="rounded-2xl border border-slate-800 bg-slate-900/40 p-6 shadow">
        <h1 className="text-xl font-semibold">Create account</h1>
        <p className="mt-1 text-sm text-slate-300">Register in the API.</p>

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
            />
          </div>

          <div>
            <label className="text-sm text-slate-300">Password</label>
            <input
              type="password"
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 text-slate-100 outline-none focus:ring-2 focus:ring-slate-700"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <div>
            <label className="text-sm text-slate-300">Role</label>
            <select
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 text-slate-100 outline-none focus:ring-2 focus:ring-slate-700"
              value={role}
              onChange={(e) => setRole(e.target.value)}
            >
              <option>User</option>
              <option>Admin</option>
            </select>
          </div>

          <button
            disabled={loading}
            className="w-full rounded-xl bg-white px-3 py-2 font-medium text-slate-950 hover:bg-slate-200 disabled:opacity-60"
          >
            {loading ? "Creating..." : "Create account"}
          </button>
        </form>

        <div className="mt-4 text-sm text-slate-300">
          Have an account?{" "}
          <Link className="text-white underline" to="/login">
            Login
          </Link>
        </div>
      </div>
    </div>
  );
}
