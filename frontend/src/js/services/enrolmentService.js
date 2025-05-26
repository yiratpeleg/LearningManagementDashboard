export default class EnrolmentService {
  static basePath = "/enrolments";

  constructor(apiClient) {
    this.api = apiClient;
  }

  list() {
    return this.api.get(EnrolmentService.basePath);
  }

  create(payload) {
    return this.api.post(EnrolmentService.basePath, payload);
  }
}
