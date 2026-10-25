document.querySelector("form").addEventListener("submit", e =>
{
    const btn = e.target.querySelector("button[type=submit]");

    btn.disabled = true;

    btn.textContent = "Uploading…";
});