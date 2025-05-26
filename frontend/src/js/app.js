import ApiClient from "./services/apiClient.js";
import CourseService from "./services/courseService.js";
import StudentService from "./services/studentService.js";
import EnrolmentService from "./services/enrolmentService.js";

import CourseController from "./controllers/courseController.js";
import StudentController from "./controllers/studentController.js";
import EnrolmentController from "./controllers/enrolmentController.js";
import ReportController from "./controllers/reportController.js";

import { fetchText, showSection } from "./utils/dom.js";

class App {
  constructor() {
    const client = new ApiClient("https://localhost:5001/api");
    this.courseService = new CourseService(client);
    this.studentService = new StudentService(client);
    this.enrolmentService = new EnrolmentService(client);

    this.els = {};
    this.loaded = {
      courses: false,
      students: false,
      enrolments: false,
      report: false,
    };

    this.controllers = {};
  }

  async init() {
    await this.renderLayout();
    this.cacheEls();

    this.controllers.courses = new CourseController(
      this.courseService,
      this.els,
      this.loaded
    );
    this.controllers.students = new StudentController(
      this.studentService,
      this.els,
      this.loaded
    );
    this.controllers.enrolments = new EnrolmentController(
      this.enrolmentService,
      this.courseService,
      this.studentService,
      this.els,
      this.loaded
    );
    this.controllers.report = new ReportController(
      this.enrolmentService,
      this.courseService,
      this.els,
      this.loaded
    );

    this.bindNav();
    this.bindForms();

    const start = window.location.hash.replace(/^#/, "") || "courses-section";
    await this.onNav(start);
  }

  async renderLayout() {
    this.els.navWrap = document.getElementById("nav-placeholder");
    this.els.mainWrap = document.getElementById("main-placeholder");

    this.els.navWrap.innerHTML = await fetchText("./partials/nav.html");
    const parts = await Promise.all([
      fetchText("./partials/courses.html"),
      fetchText("./partials/students.html"),
      fetchText("./partials/enrolments.html"),
      fetchText("./partials/report.html"),
    ]);
    this.els.mainWrap.innerHTML = parts.join("");
  }

  cacheEls() {
    this.els = {
      navWrap: document.getElementById("nav-placeholder"),
      mainWrap: document.getElementById("main-placeholder"),

      coursesRoot: document.getElementById("courses-section"),
      studentsRoot: document.getElementById("students-section"),
      enrolmentsRoot: document.getElementById("enrolments-section"),
      reportRoot: document.getElementById("report-section"),
    };
  }

  bindNav() {
    this.els.navWrap.querySelectorAll(".nav-item").forEach((btn) =>
      btn.addEventListener("click", () => {
        window.location.hash = btn.dataset.target;
        this.onNav(btn.dataset.target);
      })
    );
  }

  bindForms() {
    Object.values(this.controllers).forEach((controller) => {
      controller.bind();
    });
  }

  async onNav(sectionId) {
    showSection(sectionId);

    switch (sectionId) {
      case "courses-section":
        if (!this.loaded.courses) {
          await this.controllers.courses.load();
        }
        break;

      case "students-section":
        if (!this.loaded.students) {
          await this.controllers.students.load();
        }
        break;

      case "enrolments-section":
        if (!this.loaded.enrolments) {
          await this.controllers.enrolments.load();
        }
        break;

      case "report-section":
        if (!this.loaded.report) {
          await this.controllers.report.load();
        }
        break;
    }
  }
}

document.addEventListener("DOMContentLoaded", () => {
  new App().init();
});
