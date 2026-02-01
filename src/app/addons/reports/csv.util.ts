
export function exportToCsv(filename: string, rows: any[]) {
  if (!rows || !rows.length) return;

  const keys = Array.from(new Set(rows.flatMap(r => Object.keys(r))));
  const esc = (v: any) => '"' + String(v ?? '').replace(/"/g, '""') + '"';

  const data = [keys.join(',')]
    .concat(rows.map(r => keys.map(k => esc(r[k])).join(',')))
    .join('\n');

  const blob = new Blob([data], { type: 'text/csv;charset=utf-8;' });
  const link = document.createElement('a');
  const url = URL.createObjectURL(blob);
  link.href = url;
  link.setAttribute('download', filename);
  link.click();
  URL.revokeObjectURL(url);
}
