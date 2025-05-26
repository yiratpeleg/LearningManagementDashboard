export default class StudentService {
  static basePath = "/students";

  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.get(StudentService.basePath);
  }

  create(payload) {
    return this.api.post(StudentService.basePath, payload);
  }
}
