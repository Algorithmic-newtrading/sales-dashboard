import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  ResponsiveContainer,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  Legend,
} from "recharts";
import { getTimeline } from "../api/analytics";
import { useRange } from "../hooks/useRange";
import { smoothTimeline } from "../utils/smooth";

export function TimelineBlock() {
  const { asRange } = useRange();
  const range = asRange();
  const [smooth, setSmooth] = useState(false);

  const { data, isLoading, isError } = useQuery({
    queryKey: ["timeline", range.from, range.to],
    queryFn: () => getTimeline(range, "day"),
  });

  // Защита: если backend вернул не массив (вдруг обёртка), не падаем
  const safeData = Array.isArray(data) ? data : [];
  const chartData = smooth ? smoothTimeline(safeData, 0.3) : safeData;

  return (
    <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
      <div className="flex items-center justify-between mb-3 flex-wrap gap-2">
        <h2 className="text-lg font-semibold">Динамика</h2>
        <div className="flex gap-2">
          <button
            onClick={() => setSmooth(false)}
            className={`px-3 py-1 text-sm rounded-lg border ${
              !smooth
                ? "bg-indigo-500 border-indigo-500 text-white"
                : "border-[#232a44] text-slate-300"
            }`}
          >
            Сырые данные
          </button>
          <button
            onClick={() => setSmooth(true)}
            className={`px-3 py-1 text-sm rounded-lg border ${
              smooth
                ? "bg-indigo-500 border-indigo-500 text-white"
                : "border-[#232a44] text-slate-300"
            }`}
          >
            Сглаженные
          </button>
        </div>
      </div>

      {isLoading && (
        <div className="h-[280px] bg-[#0f1424] rounded-xl animate-pulse" />
      )}
      {isError && (
        <div className="h-[280px] flex items-center justify-center text-rose-400">
          Ошибка загрузки
        </div>
      )}

      {!isLoading && !isError && chartData.length === 0 && (
        <div className="h-[280px] flex items-center justify-center text-slate-400">
          Нет данных за период
        </div>
      )}

      {chartData.length > 0 && (
        <ResponsiveContainer width="100%" height={280}>
          <AreaChart data={chartData}>
            <defs>
              <linearGradient id="rev" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#6366f1" stopOpacity={0.7} />
                <stop offset="100%" stopColor="#6366f1" stopOpacity={0} />
              </linearGradient>
              <linearGradient id="gp" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stopColor="#10b981" stopOpacity={0.7} />
                <stop offset="100%" stopColor="#10b981" stopOpacity={0} />
              </linearGradient>
            </defs>
            <CartesianGrid stroke="#232a44" strokeDasharray="3 3" />
            <XAxis
              dataKey="date"
              stroke="#64748b"
              fontSize={12}
              tickFormatter={(v) => new Date(String(v)).toLocaleDateString("ru-RU")}
            />
            <YAxis
              stroke="#64748b"
              fontSize={12}
              tickFormatter={(v) => `${Math.round(Number(v) / 1000)}k`}
            />
            <Tooltip
              contentStyle={{
                background: "#151a2d",
                border: "1px solid #232a44",
                borderRadius: 12,
                color: "#e5e7eb",
              }}
              labelFormatter={(v) => {
                if (v == null) return "";
                return new Date(String(v)).toLocaleDateString("ru-RU");
              }}
              formatter={(value) => Number(value).toLocaleString("ru-RU")}
            />
            <Legend />
            <Area
              dataKey="revenue"
              name="Выручка"
              stroke="#6366f1"
              fill="url(#rev)"
              strokeWidth={2}
            />
            <Area
              dataKey="grossProfit"
              name="Валовая прибыль"
              stroke="#10b981"
              fill="url(#gp)"
              strokeWidth={2}
            />
          </AreaChart>
        </ResponsiveContainer>
      )}
    </div>
  );
}