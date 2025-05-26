import { escapeHtml } from "../utils/dom.js";

export function renderCourseRows(courses) {
  return courses
    .map(
      (c) => `
    <tr>
      <td>${escapeHtml(c.name)}</td>
      <td>${escapeHtml(c.description)}</td>
      <td>
        <button data-id="${
          c.id
        }" class="btn btn-secondary edit-course">Edit</button>
        <button data-id="${
          c.id
        }" class="btn btn-secondary delete-course">Delete</button>
      </td>
    </tr>
  `
    )
    .join("");
}
