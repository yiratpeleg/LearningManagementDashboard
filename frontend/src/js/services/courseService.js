export default class CourseService {
  static basePath = "/courses";

  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.get(CourseService.basePath);
  }

  create(payload) {
    return this.api.post(CourseService.basePath, payload);
  }

  update(id, payload) {
    return this.api.put(`${CourseService.basePath}/${id}`, payload);
  }

  delete(id) {
    return this.api.delete(`${CourseService.basePath}/${id}`);
  }
}
