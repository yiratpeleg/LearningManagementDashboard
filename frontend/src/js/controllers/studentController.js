import BaseController from "./baseController.js";
import { renderStudentRows } from "../templates/studentTemplates.js";

export default class StudentController extends BaseController {
  constructor(service, els, loaded) {
    super(els, loaded);
    this.service = service;
  }

  bind() {
    const root = this.els.studentsRoot;
    const form = root.querySelector("#student-form");

    form.addEventListener("submit", (e) => this.submit(e));
  }

  async load() {
    const list = await this.service.list();
    const tbody = this.els.studentsRoot.querySelector("#students-table tbody");

    tbody.innerHTML = renderStudentRows(list);

    this.loaded.students = true;
  }

  async submit(evt) {
    evt.preventDefault();
    const root = this.els.studentsRoot;
    const fullName = root.querySelector("#student-fullname").value.trim();
    if (!fullName) return alert("Name is required.");

    try {
      await this.service.create({ fullName });
      root.querySelector("#student-form").reset();
      await this.load();
      this.loaded.enrolments = false;
    } catch (ex) {
      alert(`Saving student failed: ${ex.message}`);
    }
  }
}
