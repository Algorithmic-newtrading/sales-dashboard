import { create } from "zustand";
import { toDateOnly } from "../api/client";

export type PresetKey = "today" | "7d" | "30d" | "thisMonth" | "lastMonth" | "custom";

interface RangeState {
  preset: PresetKey;
  from: Date;
  to: Date;
  setPreset: (p: PresetKey) => void;
  setCustom: (from: Date, to: Date) => void;
  asRange: () => { from: string; to: string };
}

function computeRange(p: PresetKey): { from: Date; to: Date } {
  const now = new Date();
  const end = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);

  switch (p) {
    case "today": {
      const s = new Date(now.getFullYear(), now.getMonth(), now.getDate());
      return { from: s, to: end };
    }
    case "7d": {
      const s = new Date(now);
      s.setDate(s.getDate() - 6);
      s.setHours(0, 0, 0, 0);
      return { from: s, to: end };
    }
    case "30d": {
      const s = new Date(now);
      s.setDate(s.getDate() - 29);
      s.setHours(0, 0, 0, 0);
      return { from: s, to: end };
    }
    case "thisMonth": {
      const s = new Date(now.getFullYear(), now.getMonth(), 1);
      return { from: s, to: end };
    }
    case "lastMonth": {
      const s = new Date(now.getFullYear(), now.getMonth() - 1, 1);
      const e = new Date(now.getFullYear(), now.getMonth(), 1);
      return { from: s, to: e };
    }
    default: {
      const s = new Date(now);
      s.setDate(s.getDate() - 29);
      s.setHours(0, 0, 0, 0);
      return { from: s, to: end };
    }
  }
}

const initial = computeRange("30d");

export const useRange = create<RangeState>((set, get) => ({
  preset: "30d",
  from: initial.from,
  to: initial.to,

  setPreset: (p) => {
    const r = computeRange(p);
    set({ preset: p, from: r.from, to: r.to });
  },

  setCustom: (from, to) => {
    set({ preset: "custom", from, to });
  },

  asRange: () => {
    const { from, to } = get();
    return { from: toDateOnly(from), to: toDateOnly(to) };
  },
}));