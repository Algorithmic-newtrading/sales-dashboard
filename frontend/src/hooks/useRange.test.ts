import { describe, it, expect, beforeEach } from "vitest";
import { useRange } from "./useRange";

describe("useRange store", () => {
  beforeEach(() => {
    useRange.getState().setPreset("30d");
  });

  it("preset '7d' — диапазон 7 дней", () => {
    useRange.getState().setPreset("7d");
    const { from, to } = useRange.getState();

    const diffDays = Math.round(
      (to.getTime() - from.getTime()) / (1000 * 60 * 60 * 24)
    );
    expect(diffDays).toBe(7);
  });

  it("preset '30d' — диапазон 30 дней", () => {
    useRange.getState().setPreset("30d");
    const { from, to } = useRange.getState();

    const diffDays = Math.round(
      (to.getTime() - from.getTime()) / (1000 * 60 * 60 * 24)
    );
    expect(diffDays).toBe(30);
  });

  it("preset 'today' — from = начало сегодня, to = завтра", () => {
    useRange.getState().setPreset("today");
    const { from, to } = useRange.getState();

    const now = new Date();
    expect(from.getDate()).toBe(now.getDate());
    expect(from.getMonth()).toBe(now.getMonth());
    expect(from.getFullYear()).toBe(now.getFullYear());
    expect(from.getHours()).toBe(0);
    expect(from.getMinutes()).toBe(0);

    const diffDays = Math.round(
      (to.getTime() - from.getTime()) / (1000 * 60 * 60 * 24)
    );
    expect(diffDays).toBe(1);
  });

  it("setCustom — устанавливает произвольный диапазон", () => {
    const f = new Date("2026-01-01");
    const t = new Date("2026-02-01");
    useRange.getState().setCustom(f, t);

    const { from, to, preset } = useRange.getState();
    expect(preset).toBe("custom");
    expect(from.toISOString()).toBe(f.toISOString());
    expect(to.toISOString()).toBe(t.toISOString());
  });

  it("asRange — возвращает строки YYYY-MM-DD", () => {
    useRange
      .getState()
      .setCustom(new Date("2026-01-15"), new Date("2026-02-20"));
    const range = useRange.getState().asRange();

    expect(range.from).toBe("2026-01-15");
    expect(range.to).toBe("2026-02-20");
  });
});