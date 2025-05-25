export default class CourseService {
  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.request("/courses");
  }

  create({ name, description }) {
    return this.api.request("/courses", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, description }),
    });
  }

  update(id, { name, description }) {
    return this.api.request(`/courses/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, description }),
    });
  }

  delete(id) {
    return this.api.request(`/courses/${id}`, {
      method: "DELETE",
    });
  }
}
