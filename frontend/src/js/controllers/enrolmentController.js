import BaseController from "./baseController.js";
import {
  renderCourseOptions,
  renderStudentOptions,
  renderEnrolmentList,
} from "../templates/enrolmentTemplates.js";

export default class EnrolmentController extends BaseController {
  constructor(service, courseService, studentService, els, loaded) {
    super(els, loaded);
    this.service = service;
    this.courseService = courseService;
    this.studentService = studentService;
    this._cache = null;
  }

  bind() {
    const root = this.els.enrolmentsRoot;
    root
      .querySelector("#enrolment-form")
      .addEventListener("submit", (e) => this.submit(e));
    root
      .querySelector("#enrol-student-select")
      .addEventListener("change", (e) => this.onStudentChange(e.target.value));
  }

  async load() {
    await this.#ensureDependenciesLoaded();

    const { enrols, courses, students } = await this.#fetchAllData();
    this._cache = { enrols, courses, students };

    const root = this.els.enrolmentsRoot;
    root.querySelector("#enrol-course").innerHTML =
      renderCourseOptions(courses);
    root.querySelector("#enrol-student-select").innerHTML =
      renderStudentOptions(students);

    this.#renderList([]);

    this.loaded.enrolments = true;
  }

  async submit(evt) {
    evt.preventDefault();

    const root = this.els.enrolmentsRoot;
    const form = root.querySelector("#enrolment-form");
    const studentSel = root.querySelector("#enrol-student-select");
    const studentId = studentSel.value;
    const formData = new FormData(form);
    const courseId = formData.get("courseId");

    if (!studentId || !courseId) {
      return alert("Please select both a student and a course.");
    }

    const data = { studentId, courseId };

    try {
      await this.service.create(data);

      await this.load();
      this.loaded.report = false;

      studentSel.value = studentId;
      this.onStudentChange(studentId);
    } catch (ex) {
      alert(`Enrolment failed: ${ex.message}`);
    }
  }

  onStudentChange(studentId) {
    if (!this._cache) return;
    const { enrols, courses } = this._cache;
    const courseMap = Object.fromEntries(courses.map((c) => [c.id, c.name]));
    const names = enrols
      .filter((e) => e.studentId === studentId)
      .map((e) => courseMap[e.courseId]);
    this.#renderList(names);
  }

  async #ensureDependenciesLoaded() {
    if (!this.loaded.courses) {
      await this.courseService.list();
    }
    if (!this.loaded.students) {
      await this.studentService.list();
    }
  }

  async #fetchAllData() {
    const [enrols, courses, students] = await Promise.all([
      this.service.list(),
      this.courseService.list(),
      this.studentService.list(),
    ]);
    return { enrols, courses, students };
  }

  #renderList(courseNames) {
    const list = this.els.enrolmentsRoot.querySelector("#enrolments-list");
    list.innerHTML = renderEnrolmentList(courseNames);
  }
}
