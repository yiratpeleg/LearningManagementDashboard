import { escapeHtml } from "../utils/dom.js";

export function renderStudentRows(students) {
  return students
    .map(
      (s) => `
      <tr>
        <td>${escapeHtml(s.fullName)}</td>
      </tr>
    `
    )
    .join("");
}
