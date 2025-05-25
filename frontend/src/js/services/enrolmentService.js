export default class EnrolmentService {
  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.request("/enrolments");
  }

  create({ courseId, studentId }) {
    return this.api.request("/enrolments", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ courseId, studentId }),
    });
  }
}
