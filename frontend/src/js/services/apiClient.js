const METHODS = {
  GET: "GET",
  POST: "POST",
  PUT: "PUT",
  DELETE: "DELETE",
};

const HEADER_JSON = "application/json";
const HEADERS = { "Content-Type": HEADER_JSON };
const STATUS_NO_CONTENT = 204;

export default class ApiClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  async request(path, options = {}) {
    const res = await fetch(`${this.baseUrl}${path}`, options);
    if (!res.ok) {
      const errText = await res.text().catch(() => res.statusText);
      throw new Error(
        `${options.method || METHODS.GET} ${path} failed (${
          res.status
        }): ${errText}`
      );
    }
    return res.status !== STATUS_NO_CONTENT ? res.json() : null;
  }

  get(path) {
    return this.request(path, {
      method: METHODS.GET,
    });
  }

  post(path, data) {
    return this.request(path, {
      method: METHODS.POST,
      headers: HEADERS,
      body: JSON.stringify(data),
    });
  }

  put(path, data) {
    return this.request(path, {
      method: METHODS.PUT,
      headers: HEADERS,
      body: JSON.stringify(data),
    });
  }

  delete(path) {
    return this.request(path, {
      method: METHODS.DELETE,
    });
  }
}
