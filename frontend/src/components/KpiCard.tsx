import { motion } from "framer-motion";

interface Props {
  title: string;
  value: string;
  delta?: number;
  hint?: string;
  accent?: string;
}

export function KpiCard({ title, value, delta, hint, accent = "#6366f1" }: Props) {
  const hasDelta = typeof delta === "number";
  const positive = (delta ?? 0) >= 0;

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.35, ease: "easeOut" }}
      className="rounded-2xl p-5 border border-[#232a44] bg-[#151a2d] flex flex-col gap-2 min-h-[130px]"
    >
      <div className="text-xs uppercase tracking-wider text-slate-400">{title}</div>
      <div className="text-2xl font-semibold" style={{ color: accent }}>
        {value}
      </div>
      {hasDelta && (
        <div className={`text-xs ${positive ? "text-emerald-400" : "text-rose-400"}`}>
          {positive ? "▲" : "▼"} {Math.abs(delta!).toFixed(1)}% к пред. периоду
        </div>
      )}
      {hint && <div className="text-xs text-slate-500 mt-auto">{hint}</div>}
    </motion.div>
  );
}