import { useQuery } from "@tanstack/react-query";
import {
  ResponsiveContainer,
  ScatterChart,
  Scatter,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  ZAxis,
} from "recharts";
import { getManagers } from "../api/analytics";
import { useRange } from "../hooks/useRange";

export function ManagerScatterBlock() {
  const { asRange } = useRange();
  const range = asRange();

  const { data } = useQuery({
    queryKey: ["managers", range.from, range.to, "grossProfit"],
    queryFn: () => getManagers(range, "grossProfit"),
  });

  const managers = Array.isArray(data) ? data : [];

  return (
    <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
      <h2 className="text-lg font-semibold mb-1">Матрица: объём vs маржа</h2>
      <p className="text-xs text-slate-500 mb-3">
        X — кол-во продаж, Y — маржинальность, размер точки — выручка
      </p>

      {managers.length > 0 && (
        <ResponsiveContainer width="100%" height={340}>
          <ScatterChart margin={{ top: 10, right: 20, bottom: 10, left: 0 }}>
            <CartesianGrid stroke="#232a44" strokeDasharray="3 3" />
            <XAxis
              type="number"
              dataKey="salesCount"
              name="Продаж"
              stroke="#64748b"
              fontSize={12}
            />
            <YAxis
              type="number"
              dataKey="margin"
              name="Маржа, %"
              stroke="#64748b"
              fontSize={12}
              tickFormatter={(v) => `${v}%`}
            />
            <ZAxis
              type="number"
              dataKey="revenue"
              range={[50, 500]}
              name="Выручка"
            />
            <Tooltip
              cursor={{ strokeDasharray: "3 3" }}
              content={({ payload }) => {
                if (!payload || !payload.length) return null;
                const p = payload[0].payload;
                return (
                  <div className="bg-[#151a2d] border border-[#232a44] rounded-lg p-3 text-xs">
                    <div className="font-semibold mb-1">{p.fullName}</div>
                    <div className="text-slate-400 mb-2">
                      {p.team} · {p.position}
                    </div>
                    <div>Продаж: {p.salesCount}</div>
                    <div>Маржа: {p.margin.toFixed(1)}%</div>
                    <div>Выручка: {p.revenue.toLocaleString("ru-RU")} ₽</div>
                    <div className="text-emerald-400">
                      GP: {p.grossProfit.toLocaleString("ru-RU")} ₽
                    </div>
                  </div>
                );
              }}
            />
            <Scatter
              data={managers}
              fill="#6366f1"
              fillOpacity={0.65}
              stroke="#818cf8"
            />
          </ScatterChart>
        </ResponsiveContainer>
      )}

      {managers.length === 0 && (
        <div className="h-[340px] flex items-center justify-center text-slate-400">
          Нет данных за период
        </div>
      )}
    </div>
  );
}