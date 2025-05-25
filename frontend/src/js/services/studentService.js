export default class StudentService {
  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.request("/students");
  }

  create({ fullName }) {
    return this.api.request("/students", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ fullName }),
    });
  }
}
