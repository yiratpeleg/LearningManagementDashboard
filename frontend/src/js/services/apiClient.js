export default class ApiClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
  }

  async request(path, options = {}) {
    const res = await fetch(`${this.baseUrl}${path}`, options);
    if (!res.ok) {
      const err = await res.text().catch(() => res.statusText);
      throw new Error(
        `${options.method || "GET"} ${path} failed (${res.status}): ${err}`
      );
    }
    return res.status !== 204 ? res.json() : null;
  }
}
