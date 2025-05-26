import BaseController from "./baseController.js";
import { showSection } from "../utils/dom.js";
import { renderCourseRows } from "../templates/courseTemplates.js";

export default class CourseController extends BaseController {
  constructor(service, els, loaded) {
    super(els, loaded);
    this.service = service;
  }

  bind() {
    this.#bindTableActions();
    this.#bindFormSubmit();
    this.#bindFormCancel();
  }

  async load() {
    const list = await this.service.list();
    const tbody = this.els.coursesRoot.querySelector("#courses-table tbody");

    tbody.innerHTML = renderCourseRows(list);

    this.loaded.courses = true;
  }

  async submit(evt) {
    evt.preventDefault();
    const form = this.#getForm();
    const data = this.#getFormData(form);

    try {
      await this.#saveCourse(data);
      this.#clearForm(form);
      await this.load();
      this.loaded.enrolments = false;
      this.loaded.report = false;
    } catch (ex) {
      alert(`Saving course failed: ${ex.message}`);
    }
  }

  populateForm(id) {
    const form = this.#getForm();
    const row = this.#getRowById(id);
    this.#setFormFields(form, row, id);
    this.#showCancel();
    showSection("courses-section");
  }

  resetForm() {
    const form = this.#getForm();
    this.#clearForm(form);
    this.#hideCancel();
  }

  async delete(id) {
    if (!confirm("Delete this course?")) return;
    try {
      await this.service.delete(id);
      await this.load();
      this.loaded.enrolments = false;
      this.loaded.report = false;
    } catch (ex) {
      alert(`Delete failed: ${ex.message}`);
    }
  }

  #bindTableActions() {
    const table = this.els.coursesRoot.querySelector("table");
    table.addEventListener("click", (e) => {
      const btn = e.target.closest("button[data-id]");
      if (!btn) return;

      const id = btn.dataset.id;
      if (btn.classList.contains("edit-course")) this.populateForm(id);
      if (btn.classList.contains("delete-course")) this.delete(id);
    });
  }

  #bindFormSubmit() {
    const form = this.els.coursesRoot.querySelector("#course-form");
    form.addEventListener("submit", (e) => this.submit(e));
  }

  #bindFormCancel() {
    const cancelBtn = this.els.coursesRoot.querySelector("#course-cancel");
    cancelBtn.addEventListener("click", () => this.resetForm());
  }

  #getForm() {
    const form = this.els.coursesRoot.querySelector("#course-form");
    if (!form) throw new Error("CourseController: form not found");
    return form;
  }

  #getFormData(form) {
    return Object.fromEntries(new FormData(form).entries());
  }

  async #saveCourse(data) {
    if (data.id) {
      return this.service.update(data.id, data);
    } else {
      return this.service.create(data);
    }
  }

  #clearForm(form) {
    form.reset();
    const idInput = form.querySelector('input[name="id"]');
    if (idInput) idInput.value = "";
  }

  #getRowById(id) {
    const btn = this.els.coursesRoot.querySelector(`button[data-id="${id}"]`);
    if (!btn) throw new Error(`Row button for id ${id} not found`);
    return btn.closest("tr");
  }

  #setFormFields(form, row, id) {
    const idInput = form.querySelector('input[name="id"]');
    const nameInput = form.querySelector('input[name="name"]');
    const descInput = form.querySelector('input[name="description"]');

    if (!idInput || !nameInput || !descInput) {
      throw new Error("CourseController: form inputs missing");
    }

    idInput.value = id;
    nameInput.value = row.cells[0].textContent.trim();
    descInput.value = row.cells[1].textContent.trim();
  }

  #showCancel() {
    const btn = this.els.coursesRoot.querySelector("#course-cancel");
    if (btn) btn.classList.remove("hidden");
  }

  #hideCancel() {
    const btn = this.els.coursesRoot.querySelector("#course-cancel");
    if (btn) btn.classList.add("hidden");
  }
}
