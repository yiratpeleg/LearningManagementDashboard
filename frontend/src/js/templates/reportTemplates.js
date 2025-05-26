import { escapeHtml } from "../utils/dom.js";

export function renderReportRows(data) {
  return data
    .map(
      (item) => `
    <tr>
      <td>${escapeHtml(item.name)}</td>
      <td>${item.count}</td>
    </tr>
  `
    )
    .join("");
}
