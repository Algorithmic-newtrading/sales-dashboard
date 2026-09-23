import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { motion, AnimatePresence } from "framer-motion";
import { getManagers } from "../api/analytics";
import { useRange } from "../hooks/useRange";

const fmtMoney = (n: number) =>
  n.toLocaleString("ru-RU", { maximumFractionDigits: 0 });

export function ManagerRatingBlock() {
  const { asRange } = useRange();
  const [sortBy, setSortBy] = useState<"grossProfit" | "avgCheck">("grossProfit");
  const range = asRange();

  const { data, isLoading, isError } = useQuery({
    queryKey: ["managers", range.from, range.to, sortBy],
    queryFn: () => getManagers(range, sortBy),
  });

  const managers = Array.isArray(data) ? data : [];

  return (
    <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold">Рейтинг менеджеров</h2>
        <div className="flex gap-2">
          <button
            onClick={() => setSortBy("grossProfit")}
            className={`px-3 py-1 text-sm rounded-lg border ${
              sortBy === "grossProfit"
                ? "bg-indigo-500 border-indigo-500 text-white"
                : "border-[#232a44] text-slate-300"
            }`}
          >
            По Gross Profit
          </button>
          <button
            onClick={() => setSortBy("avgCheck")}
            className={`px-3 py-1 text-sm rounded-lg border ${
              sortBy === "avgCheck"
                ? "bg-indigo-500 border-indigo-500 text-white"
                : "border-[#232a44] text-slate-300"
            }`}
          >
            По среднему чеку
          </button>
        </div>
      </div>

      {isLoading && (
        <div className="space-y-2">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="h-14 bg-[#0f1424] rounded-xl animate-pulse" />
          ))}
        </div>
      )}

      {isError && (
        <div className="text-rose-400 text-sm">Ошибка загрузки рейтинга</div>
      )}

      {!isLoading && !isError && managers.length === 0 && (
        <div className="text-slate-400 text-sm">За выбранный период продаж нет</div>
      )}

      <div className="space-y-2">
        <AnimatePresence>
          {managers.map((m) => (
            <motion.div
              key={m.managerId}
              layout
              initial={{ opacity: 0, y: 8 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0 }}
              transition={{ duration: 0.25 }}
              className="flex items-center gap-3 p-3 rounded-xl bg-[#0f1424] border border-[#1e2540]"
            >
              <div className="w-8 text-center text-slate-400 font-semibold">
                #{m.rank}
              </div>
              <div
                className="w-9 h-9 rounded-full flex items-center justify-center text-white font-semibold text-sm"
                style={{ background: m.avatarColor }}
              >
                {m.fullName
                  .split(" ")
                  .map((s) => s[0])
                  .slice(0, 2)
                  .join("")}
              </div>
              <div className="flex-1 min-w-0">
                <div className="font-medium truncate">{m.fullName}</div>
                <div className="text-xs text-slate-500">
                  {m.team} · {m.position}
                </div>
              </div>
              <div className="text-right text-sm">
                <div>{m.salesCount} продаж</div>
                <div className="text-slate-400">{fmtMoney(m.revenue)} ₽</div>
              </div>
              <div className="text-right text-sm w-32 hidden md:block">
                <div className="text-emerald-400">{fmtMoney(m.grossProfit)} ₽</div>
                <div className="text-slate-400">Маржа {m.margin.toFixed(1)}%</div>
              </div>
              <div className="text-right text-sm w-28 hidden lg:block">
                <div>{fmtMoney(m.averageCheck)} ₽</div>
                <div className="text-slate-400">сред. чек</div>
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>
    </div>
  );
}