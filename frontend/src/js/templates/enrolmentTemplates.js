import { escapeHtml } from "../utils/dom.js";

export function renderCourseOptions(courses) {
  return courses
    .map((c) => `<option value="${c.id}">${escapeHtml(c.name)}</option>`)
    .join("");
}

export function renderStudentOptions(students) {
  return [
    `<option value="">-- choose a student --</option>`,
    ...students.map(
      (s) => `<option value="${s.id}">${escapeHtml(s.fullName)}</option>`
    ),
  ].join("");
}

export function renderEnrolmentList(courseNames) {
  if (!courseNames.length) {
    return `<li class="empty">No enrollments yet</li>`;
  }
  return courseNames.map((name) => `<li>${escapeHtml(name)}</li>`).join("");
}
