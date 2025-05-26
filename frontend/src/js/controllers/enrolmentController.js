import BaseController from "./baseController.js";
import {
  renderEnrolmentRows,
  renderCourseOptions,
  renderStudentOptions,
} from "../templates/enrolmentTemplates.js";

export default class EnrolmentController extends BaseController {
  constructor(service, courseService, studentService, els, loaded) {
    super(els, loaded);
    this.service = service;
    this.courseService = courseService;
    this.studentService = studentService;
  }

  bind() {
    const root = this.els.enrolmentsRoot;
    root
      .querySelector("#enrolment-form")
      .addEventListener("submit", (e) => this.submit(e));
  }

  async load() {
    await this.#ensureDependenciesLoaded();
    const { enrols, courses, students } = await this.#fetchAllData();
    const { courseMap, studentMap } = this.#createLookupMaps(courses, students);

    this.#renderTable(enrols, courseMap, studentMap);
    this.#renderFormOptions(courses, students);

    this.loaded.enrolments = true;
  }

  async submit(evt) {
    evt.preventDefault();

    const root = this.els.enrolmentsRoot;
    const data = Object.fromEntries(
      new FormData(root.querySelector("#enrolment-form")).entries()
    );

    try {
      await this.service.create(data);
      await this.load();
      this.loaded.report = false;
    } catch (ex) {
      alert(`Enrolment failed: ${ex.message}`);
    }
  }

  async #ensureDependenciesLoaded() {
    if (!this.loaded.courses) {
      await this.courseService.list();
      this.loaded.courses = true;
    }
    if (!this.loaded.students) {
      await this.studentService.list();
      this.loaded.students = true;
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

  #createLookupMaps(courses, students) {
    const courseMap = Object.fromEntries(courses.map((c) => [c.id, c.name]));
    const studentMap = Object.fromEntries(
      students.map((s) => [s.id, s.fullName])
    );
    return { courseMap, studentMap };
  }

  #renderTable(enrols, courseMap, studentMap) {
    const tbody = this.els.enrolmentsRoot.querySelector(
      "#enrolments-table tbody"
    );

    tbody.innerHTML = renderEnrolmentRows(enrols, courseMap, studentMap);
  }

  #renderFormOptions(courses, students) {
    const root = this.els.enrolmentsRoot;

    root.querySelector("#enrol-course").innerHTML =
      renderCourseOptions(courses);

    root.querySelector("#enrol-student").innerHTML =
      renderStudentOptions(students);
  }
}
