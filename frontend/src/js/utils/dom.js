export async function fetchText(url) {
  const res = await fetch(url);
  if (!res.ok) throw new Error(`Failed to load ${url}: ${res.status}`);
  return res.text();
}

export function escapeHtml(str = "") {
  const d = document.createElement("div");
  d.textContent = str;
  return d.innerHTML;
}

export function showSection(id) {
  document
    .querySelectorAll("main section")
    .forEach((s) => s.classList.add("hidden"));

  const el = document.getElementById(id);
  if (!el) throw new Error(`No section found: ${id}`);
  el.classList.remove("hidden");

  const hash = `#${id}`;
  if (window.location.hash !== hash) {
    window.history.replaceState(null, "", hash);
  }
}
