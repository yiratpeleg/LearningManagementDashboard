import BaseController from "./baseController.js";
import { renderReportRows } from "../templates/reportTemplates.js";

export default class ReportController extends BaseController {
  constructor(enrolService, courseService, els, loaded) {
    super(els, loaded);
    this.enrolService = enrolService;
    this.courseService = courseService;
  }

  async load() {
    const [enrols, courses] = await Promise.all([
      this.enrolService.list(),
      this.courseService.list(),
    ]);

    const counts = enrols.reduce((acc, e) => {
      acc[e.courseId] = (acc[e.courseId] || 0) + 1;
      return acc;
    }, {});

    const reportData = courses.map((c) => ({
      name: c.name,
      count: counts[c.id] || 0,
    }));

    const tbody = this.els.reportRoot.querySelector("#report-table tbody");
    tbody.innerHTML = renderReportRows(reportData);

    this.loaded.report = true;
  }
}
