export default class BaseController {
  constructor(els, loaded) {
    this.els = els;
    this.loaded = loaded;
  }

  bind() {}

  async load() {
    throw new Error(`${this.constructor.name} must implement load()`);
  }

  async submit(evt) {}
}
