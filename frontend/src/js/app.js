import ApiClient from "./services/apiClient.js";
import CourseService from "./services/courseService.js";
import StudentService from "./services/studentService.js";
import EnrolmentService from "./services/enrolmentService.js";
import { fetchText, escapeHtml, showSection } from "./utils/dom.js";

class App {
  constructor() {
    const client = new ApiClient("https://localhost:5001/api");
    this.courseService = new CourseService(client);
    this.studentService = new StudentService(client);
    this.enrolmentService = new EnrolmentService(client);
    this.els = {};
    this.loaded = { courses: false, students: false, enrolments: false };
  }

  async init() {
    await this.renderLayout();
    this.cacheEls();
    this.bindNav();
    this.bindForms();
    await this.onNav("courses-section");
  }

  // Inject nav + sections
  async renderLayout() {
    this.els.navWrap = document.getElementById("nav-placeholder");
    this.els.mainWrap = document.getElementById("main-placeholder");

    this.els.navWrap.innerHTML = await fetchText("./partials/nav.html");
    const parts = await Promise.all([
      fetchText("./partials/courses.html"),
      fetchText("./partials/students.html"),
      fetchText("./partials/enrolments.html"),
    ]);
    this.els.mainWrap.innerHTML = parts.join("");
  }

  // Cache DOM nodes
  cacheEls() {
    const qs = (s) => document.querySelector(s);
    this.els.sections = document.querySelectorAll("main section");
    this.els.courseForm = qs("#course-form");
    this.els.courseCancel = qs("#course-cancel");
    this.els.enrolForm = qs("#enrolment-form");
    this.els.courseTbody = qs("#courses-table tbody");
    this.els.studentForm = qs("#student-form");
    this.els.studentFull = qs("#student-fullname");
    this.els.studentTbody = qs("#students-table tbody");
    this.els.enrolTbody = qs("#enrolments-table tbody");
    console.log("🔍 Cached Elements:", this.els);
  }

  // Nav buttons
  bindNav() {
    this.els.navWrap
      .querySelectorAll(".nav-item")
      .forEach((btn) =>
        btn.addEventListener("click", () => this.onNav(btn.dataset.target))
      );
  }

  // Form handlers
  bindForms() {
    this.els.courseForm.addEventListener("submit", (e) =>
      this.onCourseSubmit(e)
    );
    this.els.courseCancel.addEventListener("click", () =>
      this.resetCourseForm()
    );
    this.els.enrolForm.addEventListener("submit", (e) => this.onEnrolSubmit(e));
    this.els.studentForm.addEventListener("submit", (e) =>
      this.onStudentSubmit(e)
    );
  }

  // Show section + lazy-load data
  async onNav(sectionId) {
    showSection(sectionId);

    if (sectionId === "courses-section" && !this.loaded.courses) {
      await this.loadCourses();
      this.loaded.courses = true;
    }
    if (sectionId === "students-section" && !this.loaded.students) {
      await this.loadStudents();
      this.loaded.students = true;
    }
    if (sectionId === "enrolments-section" && !this.loaded.enrolments) {
      if (!this.loaded.courses) {
        await this.loadCourses();
        this.loaded.courses = true;
      }
      if (!this.loaded.students) {
        await this.loadStudents();
        this.loaded.students = true;
      }
      await this.loadEnrolments();
      this.loaded.enrolments = true;
    }
  }

  // Courses
  async loadCourses() {
    const list = await this.courseService.list();
    this.els.courseTbody.innerHTML = list
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

    this.els.courseTbody
      .querySelectorAll(".edit-course")
      .forEach((b) =>
        b.addEventListener("click", () => this.populateCourseForm(b.dataset.id))
      );
    this.els.courseTbody
      .querySelectorAll(".delete-course")
      .forEach((b) =>
        b.addEventListener("click", () => this.onCourseDelete(b.dataset.id))
      );
  }

  // Students
  async loadStudents() {
    const list = await this.studentService.list();
    this.els.studentTbody.innerHTML = list
      .map((s) => `<tr><td>${escapeHtml(s.fullName)}</td></tr>`)
      .join("");
  }

  async onStudentSubmit(evt) {
    evt.preventDefault();
    const data = { fullName: this.els.studentFull.value.trim() };
    if (!data.fullName) return alert("Name is required.");

    try {
      await this.studentService.create({ fullName });
      this.els.studentForm.reset();
      await this.loadStudents();
      this.loaded.enrolments = false;
    } catch (ex) {
      alert(`Saving student failed: ${ex.message}`);
    }
  }

  // Enrolments
  async loadEnrolments() {
    const [enrols, courses, students] = await Promise.all([
      this.enrolmentService.list(),
      this.courseService.list(),
      this.studentService.list(),
    ]);

    const cMap = Object.fromEntries(courses.map((c) => [c.id, c.name]));
    const sMap = Object.fromEntries(students.map((s) => [s.id, s.fullName]));

    const valid = enrols.filter((e) => cMap[e.courseId] && sMap[e.studentId]);

    this.els.enrolTbody.innerHTML = valid
      .map(
        (e) => `
    <tr>
      <td>${escapeHtml(cMap[e.courseId])}</td>
      <td>${escapeHtml(sMap[e.studentId])}</td>
      <td>${new Date(e.enrolledAt).toLocaleString()}</td>
    </tr>
  `
      )
      .join("");

    this.els.enrolCourseSelect.innerHTML = courses
      .map((c) => `<option value="${c.id}">${escapeHtml(c.name)}</option>`)
      .join("");

    this.els.enrolStudentSelect.innerHTML = students
      .map((s) => `<option value="${s.id}">${escapeHtml(s.fullName)}</option>`)
      .join("");
  }

  async onCourseSubmit(evt) {
    evt.preventDefault();
    const data = Object.fromEntries(
      new FormData(this.els.courseForm).entries()
    );
    try {
      if (data.id) await this.courseService.update(data.id, data);
      else await this.courseService.create(data);
      this.resetCourseForm();
      await this.loadCourses();
      this.loaded.enrolments = false;
    } catch (ex) {
      alert(`Saving course failed: ${ex.message}`);
    }
  }

  populateCourseForm(id) {
    const row = this.els.courseTbody
      .querySelector(`button[data-id="${id}"]`)
      .closest("tr");
    this.els.courseForm.id.value = id;
    this.els.courseForm.name.value = row.cells[0].textContent;
    this.els.courseForm.description.value = row.cells[1].textContent;
    this.els.courseCancel.classList.remove("hidden");
    showSection("courses-section");
  }

  resetCourseForm() {
    this.els.courseForm.reset();
    this.els.courseForm.id.value = "";
    this.els.courseCancel.classList.add("hidden");
  }

  async onCourseDelete(id) {
    if (!confirm("Delete this course?")) return;
    try {
      await this.api.deleteCourse(id);
      await this.loadCourses();
      this.loaded.enrolments = false;
    } catch (ex) {
      alert(`Delete failed: ${ex.message}`);
    }
  }

  // Enrolment form submit
  async onEnrolSubmit(evt) {
    evt.preventDefault();
    const data = Object.fromEntries(new FormData(this.els.enrolForm).entries());
    try {
      await this.enrolmentService.create({ courseId, studentId });
      await this.loadEnrolments();
    } catch (ex) {
      alert(`Enrolment failed: ${ex.message}`);
    }
  }

  // Utility escape
  escape(str = "") {
    const d = document.createElement("div");
    d.textContent = str;
    return d.innerHTML;
  }
}

// Bootstrap
document.addEventListener("DOMContentLoaded", () => {
  new App().init();
});
