/**
 * Скачивает массив объектов как CSV-файл.
 * Автоматически добавляет BOM для корректной кодировки в Excel.
 */
export function downloadCsv<T extends object>(
  filename: string,
  rows: T[],
): void {
  if (!rows || rows.length === 0) return;

  const headers = Object.keys(rows[0] as Record<string, unknown>);

  const escape = (v: unknown): string => {
    const s = v == null ? "" : String(v);
    if (s.includes(",") || s.includes('"') || s.includes("\n")) {
      return `"${s.replace(/"/g, '""')}"`;
    }
    return s;
  };

  const lines = [
    headers.join(","),
    ...rows.map((r) =>
      headers.map((h) => escape((r as Record<string, unknown>)[h])).join(","),
    ),
  ];

  // BOM для Excel: иначе кириллица превратится в кракозябры
  const csv = "\uFEFF" + lines.join("\n");

  const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}