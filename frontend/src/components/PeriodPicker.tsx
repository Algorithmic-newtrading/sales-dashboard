import { useRange } from "../hooks/useRange";
import type { PresetKey } from "../hooks/useRange";

const presets: { key: PresetKey; label: string }[] = [
  { key: "today", label: "Сегодня" },
  { key: "7d", label: "7 дней" },
  { key: "30d", label: "30 дней" },
  { key: "thisMonth", label: "Этот месяц" },
  { key: "lastMonth", label: "Прошлый месяц" },
];

export function PeriodPicker() {
  const { preset, setPreset, setCustom, from, to } = useRange();

  const fromStr = from.toISOString().slice(0, 10);
  const toStr = to.toISOString().slice(0, 10);

  return (
    <div className="flex items-center gap-2 flex-wrap">
      {presets.map((p) => (
        <button
          key={p.key}
          onClick={() => setPreset(p.key)}
          className={`px-3 py-1.5 text-sm rounded-lg border transition ${
            preset === p.key
              ? "bg-indigo-500 border-indigo-500 text-white"
              : "border-[#232a44] text-slate-300 hover:border-indigo-500"
          }`}
        >
          {p.label}
        </button>
      ))}

      <div className="flex items-center gap-2 ml-2">
        <input
          type="date"
          value={fromStr}
          onChange={(e) => {
            const f = new Date(e.target.value);
            if (!isNaN(f.getTime())) setCustom(f, to);
          }}
          className="bg-[#151a2d] border border-[#232a44] rounded-lg px-2 py-1 text-sm text-slate-200"
        />
        <span className="text-slate-500">→</span>
        <input
          type="date"
          value={toStr}
          onChange={(e) => {
            const t = new Date(e.target.value);
            if (!isNaN(t.getTime())) setCustom(from, t);
          }}
          className="bg-[#151a2d] border border-[#232a44] rounded-lg px-2 py-1 text-sm text-slate-200"
        />
      </div>
    </div>
  );
}