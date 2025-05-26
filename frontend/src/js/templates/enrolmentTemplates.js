import { escapeHtml } from "../utils/dom.js";

export function renderEnrolmentRows(enrols, courseMap, studentMap) {
  return enrols
    .filter((e) => courseMap[e.courseId] && studentMap[e.studentId])
    .map(
      (e) => `
      <tr>
        <td>${escapeHtml(courseMap[e.courseId])}</td>
        <td>${escapeHtml(studentMap[e.studentId])}</td>
        <td>${new Date(e.enrolledAt).toLocaleString()}</td>
      </tr>
    `
    )
    .join("");
}

export function renderCourseOptions(courses) {
  return courses
    .map((c) => `<option value="${c.id}">${escapeHtml(c.name)}</option>`)
    .join("");
}

export function renderStudentOptions(students) {
  return students
    .map((s) => `<option value="${s.id}">${escapeHtml(s.fullName)}</option>`)
    .join("");
}
