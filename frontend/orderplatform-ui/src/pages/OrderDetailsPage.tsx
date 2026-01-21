import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { createOrdersHub } from "../lib/signalr";

type OrderStatusChangedMsg = {
  orderId: string;
  status: string;
  changedAtUtc: string;
};

export function OrderDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const orderId = id;

  const [status, setStatus] = useState<string>("(unknown)");

  const baseUrl = "http://localhost:5264";
  const token = localStorage.getItem("token") ?? "";

  const conn = useMemo(() => createOrdersHub(baseUrl, token), [baseUrl, token]);

  useEffect(() => {
    if (!orderId) return;

    fetch(`${baseUrl}/api/orders/${orderId}`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then(async (r) => {
        if (!r.ok) throw new Error(await r.text());
        return r.json();
      })
      .then((order) => setStatus(order.status))
      .catch(() => setStatus("(not found)"));
  }, [orderId, token, baseUrl]);

  useEffect(() => {
    if (!orderId) return;

    let cancelled = false;

    const handler = (msg: OrderStatusChangedMsg) => {
      if (msg.orderId?.toLowerCase() !== orderId.toLowerCase()) return;
      setStatus(msg.status);
    };

    (async () => {
      try {
        conn.on("orderStatusChanged", handler);

        await conn.start();
        if (cancelled) return;

        await conn.invoke("JoinOrder", orderId);
      } catch (e) {
        console.error("SignalR error:", e);
      }
    })();

    return () => {
      cancelled = true;
      conn.off("orderStatusChanged", handler);
      conn.stop().catch(() => {});
    };
  }, [conn, orderId]);

  return (
    <div className="p-6">
      <div className="text-xl font-semibold">Order {orderId}</div>
      <div className="mt-2">
        Live status: <b>{status}</b>
      </div>
    </div>
  );
}
