import { type FormEvent, useState } from "react";
import { apiFetch } from "../lib/api";
import { Link } from "react-router-dom";

type CreateOrderResponse = { orderId: string };

export function OrdersPage() {
  const [customerName, setCustomerName] = useState("ACME Ltd");
  const [totalAmount, setTotalAmount] = useState("123.45");
  const [createdId, setCreatedId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const res = await apiFetch<CreateOrderResponse>("/api/Orders", {
        method: "POST",
        body: JSON.stringify({
          customerName,
          totalAmount: Number(totalAmount),
        }),
      });
      setCreatedId(res.orderId);
    } catch (err: any) {
      setError(err?.message ?? "Failed to create order");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="grid gap-6 lg:grid-cols-2">
      <div className="rounded-2xl border border-slate-800 bg-slate-900/40 p-6">
        <h2 className="text-lg font-semibold">Create order</h2>
        <p className="mt-1 text-sm text-slate-300">
          Creates an order, publishes <span className="text-slate-100">OrderCreated</span>, worker caches it in Redis.
        </p>

        {error && (
          <div className="mt-4 rounded-xl border border-red-900/60 bg-red-950/40 p-3 text-sm text-red-200">
            {error}
          </div>
        )}

        <form onSubmit={onSubmit} className="mt-6 space-y-4">
          <div>
            <label className="text-sm text-slate-300">Customer name</label>
            <input
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-700"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
            />
          </div>

          <div>
            <label className="text-sm text-slate-300">Total amount</label>
            <input
              className="mt-1 w-full rounded-xl border border-slate-800 bg-slate-950 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-700"
              value={totalAmount}
              onChange={(e) => setTotalAmount(e.target.value)}
            />
          </div>

          <button
            disabled={loading}
            className="rounded-xl bg-white px-4 py-2 font-medium text-slate-950 hover:bg-slate-200 disabled:opacity-60"
          >
            {loading ? "Creating..." : "Create"}
          </button>
        </form>
      </div>

      <div className="rounded-2xl border border-slate-800 bg-slate-900/40 p-6">
        <h2 className="text-lg font-semibold">Last created</h2>

        {createdId ? (
          <div className="mt-4 space-y-3">
            <div className="rounded-xl border border-slate-800 bg-slate-950 p-3 font-mono text-sm">
              {createdId}
            </div>
            <div className="flex gap-3">
              <Link
                to={`/app/orders/${createdId}`}
                className="rounded-xl border border-slate-800 px-3 py-2 hover:bg-slate-900"
              >
                View details
              </Link>
            </div>
          </div>
        ) : (
          <p className="mt-4 text-sm text-slate-300">Create an order to see it here.</p>
        )}
      </div>
    </div>
  );
}
