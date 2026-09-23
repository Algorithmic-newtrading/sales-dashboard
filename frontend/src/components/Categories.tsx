import { useQuery } from "@tanstack/react-query";
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
} from "recharts";
import { getCategories, getTopProducts } from "../api/analytics";
import { useRange } from "../hooks/useRange";

const fmtMoney = (n: number) =>
  n.toLocaleString("ru-RU", { maximumFractionDigits: 0 });

export function CategoriesBlock() {
  const { asRange } = useRange();
  const range = asRange();

  const cats = useQuery({
    queryKey: ["categories", range.from, range.to],
    queryFn: () => getCategories(range),
  });

  const top = useQuery({
    queryKey: ["top-products", range.from, range.to],
    queryFn: () => getTopProducts(range, 5),
  });

  // Защита от не-массивов
  const catsData = Array.isArray(cats.data) ? cats.data : [];
  const topData = Array.isArray(top.data) ? top.data : [];

  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
      {/* Категории */}
      <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
        <h2 className="text-lg font-semibold mb-3">Категории</h2>

        {cats.isLoading && (
          <div className="h-[280px] bg-[#0f1424] rounded-xl animate-pulse" />
        )}
        {cats.isError && (
          <div className="h-[280px] flex items-center justify-center text-rose-400">
            Ошибка
          </div>
        )}

        {catsData.length > 0 && (
          <ResponsiveContainer width="100%" height={280}>
            <BarChart data={catsData}>
              <CartesianGrid stroke="#232a44" strokeDasharray="3 3" />
              <XAxis dataKey="categoryName" stroke="#64748b" fontSize={11} />
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
                formatter={(value) => Number(value).toLocaleString("ru-RU")}
              />
              <Bar dataKey="revenue" name="Выручка" fill="#6366f1" radius={[6, 6, 0, 0]} />
              <Bar dataKey="grossProfit" name="Прибыль" fill="#10b981" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        )}

        {!cats.isLoading && !cats.isError && catsData.length === 0 && (
          <div className="h-[280px] flex items-center justify-center text-slate-400">
            Нет данных
          </div>
        )}
      </div>

      {/* Топ-5 продуктов */}
      <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
        <h2 className="text-lg font-semibold mb-3">Топ-5 продуктов</h2>

        {top.isLoading && (
          <div className="space-y-2">
            {[1, 2, 3, 4, 5].map((i) => (
              <div key={i} className="h-10 bg-[#0f1424] rounded-lg animate-pulse" />
            ))}
          </div>
        )}

        {top.isError && <div className="text-rose-400 text-sm">Ошибка</div>}

        {topData.length > 0 && (
          <div className="space-y-2">
            {topData.map((p, i) => (
              <div
                key={p.productId}
                className="flex items-center gap-3 p-2 rounded-lg hover:bg-[#0f1424]"
              >
                <div className="w-6 text-slate-500 text-sm">#{i + 1}</div>
                <div className="flex-1 min-w-0">
                  <div className="text-sm font-medium truncate">{p.productName}</div>
                  <div className="text-xs text-slate-500">{p.categoryName}</div>
                </div>
                <div className="text-right text-sm">
                  <div className="text-emerald-400">{fmtMoney(p.revenue)} ₽</div>
                  <div className="text-xs text-slate-500">{p.quantity} шт</div>
                </div>
              </div>
            ))}
          </div>
        )}

        {!top.isLoading && !top.isError && topData.length === 0 && (
          <div className="text-slate-400 text-sm">Нет данных</div>
        )}
      </div>
    </div>
  );
}